using FluentAssertions.Execution;
using System.Text.RegularExpressions;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;
using System.Diagnostics;

namespace AoC2022;

public class Day19
{
    [Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(19);
    }

    [SetUp]
    public void setup()
    {
        var lines = File.ReadAllLines("day19.input");
        r.Match("x");
    }
    public enum Resource {
        Ore,
        Clay,
        Obsidian,
        Geode
    }
    public record Resources(int ore, int clay, int obsidian, int geode)
    {
        public int ore { get; set; } = ore;
        public int clay { get; set; } = clay;
        public int obsidian { get; set; } = obsidian;
        public int geode { get; set; } = geode;

        public Resources AddOreRobot() => new Resources(ore+1, clay, obsidian, geode);
        public Resources AddClayRobot() => new Resources(ore, clay+1, obsidian, geode);
        public Resources AddObsidianRobot() => new Resources(ore, clay, obsidian+1, geode);
        public Resources AddGeodeRobot() => new Resources(ore, clay, obsidian, geode+1);

        internal Resources Harvest(Resources producers)
         => new Resources(ore + producers.ore, clay + producers.clay, obsidian + producers.obsidian, geode + producers.geode);
    }
    public record Robot(Resource type, Resources resourcesToBuild)
    {
        public bool CanBeBuild(Resources r)
         => (r.ore >= resourcesToBuild.ore &&
                r.clay >= resourcesToBuild.clay &&
                r.obsidian >= resourcesToBuild.obsidian &&
                r.geode >= resourcesToBuild.geode);

        public Resources Build(Resources r)
         => new Resources(
             r.ore - resourcesToBuild.ore,
             r.clay - resourcesToBuild.clay,
             r.obsidian - resourcesToBuild.obsidian,
             r.geode - resourcesToBuild.geode);
    }
    public class Blueprint
    {
        internal int id { get; init; }
        internal Robot ore { get; init; }
        internal Robot clay { get; init; }
        internal Robot obsidian { get; init; }
        internal Robot geode { get; init; }

        public Blueprint(int id, Robot ore, Robot clay, Robot obsidian, Robot geode)
        {
            this.id = id;
            this.ore = ore;
            this.clay = clay;
            this.obsidian = obsidian;
            this.geode = geode;

            oreValue = 1;
            clayValue = clay.resourcesToBuild.ore * oreValue;
            obsidianValue = obsidian.resourcesToBuild.ore * oreValue + obsidian.resourcesToBuild.clay * clayValue;
            geodeValue = geode.resourcesToBuild.ore * oreValue + geode.resourcesToBuild.obsidian * obsidianValue;
        }

        public int stockScore(Resources stock)
         => stock.ore * oreValue + stock.clay * clayValue * 2 + stock.obsidian * obsidianValue * 3 + stock.geode * geodeValue * 4;

        public int produceScore(Resources producer, int timeleft)
         => (producer.ore * oreValue * timeleft)
            + (producer.clay * clayValue * timeleft) * 2
            + (producer.obsidian * obsidianValue * timeleft) * 3
            + (producer.geode * geodeValue * timeleft) * 4;

        public int oreValue { get; init; }
        public int clayValue { get; init; }
        public int obsidianValue { get; init; }
        public int geodeValue { get; init; }

        public int maxToProduce(State s, int maxtime)
        {
            int amount_ore = s.stock.ore;
            int amount_clay = s.stock.clay;
            int amount_obsidian = s.stock.obsidian;
            int amount_geode = s.stock.geode;
            int harvester_ore = s.harvesters.ore;
            int harvester_clay = s.harvesters.clay;
            int harvester_obsidian = s.harvesters.obsidian;
            int harvester_geode = s.harvesters.geode;
            for (int t = s.time; t < maxtime; t++)
            {
                var new_harvester_geode = Math.Min(amount_ore / geode.resourcesToBuild.ore, amount_obsidian / geode.resourcesToBuild.obsidian);
                if (new_harvester_geode > 0)
                {
                    amount_ore -= new_harvester_geode * geode.resourcesToBuild.ore;
                    amount_obsidian -= new_harvester_geode * geode.resourcesToBuild.obsidian;
                }
                amount_geode += harvester_geode;
                harvester_geode += new_harvester_geode;
                
                var new_harvester_obsidian = Math.Min(amount_ore / obsidian.resourcesToBuild.ore, amount_clay / obsidian.resourcesToBuild.clay);
                if (new_harvester_obsidian > 0)
                {
                    amount_ore -= new_harvester_obsidian * obsidian.resourcesToBuild.ore;
                    amount_clay -= new_harvester_obsidian * obsidian.resourcesToBuild.clay;
                }
                amount_obsidian += harvester_obsidian;
                harvester_obsidian += new_harvester_obsidian;

                var new_harvester_clay = amount_ore / clay.resourcesToBuild.ore;
                if (new_harvester_clay > 0)
                {
                    //amount_ore -= new_harvester_clay * clay.resourcesToBuild.ore; ;
                }
                amount_clay += harvester_clay;
                harvester_clay += new_harvester_clay;

                var new_harvester_ore = amount_ore / ore.resourcesToBuild.ore;
                if (new_harvester_ore > 0)
                {
                    //amount_ore -= newore * ore.resourcesToBuild.ore;
                }
                amount_ore += harvester_ore;
                harvester_ore += new_harvester_ore;
            }
            return amount_geode;
        }
    }

    Regex r = new Regex(@"Blueprint (?<id>[0-9]+): Each ore robot costs (?<ore_ore>[0-9]+) ore. Each clay robot costs (?<clay_ore>[0-9]+) ore. Each obsidian robot costs (?<obs_ore>[0-9]+) ore and (?<obs_clay>[0-9]+) clay. Each geode robot costs (?<geo_ore>[0-9]+) ore and (?<geo_obs>[0-9]+) obsidian.", RegexOptions.Compiled);
    [TestCase("day19.input", ExpectedResult = 1958)]
    [TestCase("day19example1.input", ExpectedResult = 33)]
    public int Part1(string input)
    {
        var lines = File.ReadAllLines(input);
        var blueprints = new List<Blueprint>();
        foreach (var line in lines)
        {
            var m = r.Match(line);
            if (m.Success)
            {
                var bp = new Blueprint(AsInt(m, "id"),
                    new Robot(Resource.Ore, new Resources(AsInt(m, "ore_ore"), 0, 0, 0)),
                    new Robot(Resource.Clay, new Resources(AsInt(m, "clay_ore"), 0, 0, 0)),
                    new Robot(Resource.Obsidian, new Resources(AsInt(m, "obs_ore"), AsInt(m, "obs_clay"), 0, 0)),
                    new Robot(Resource.Geode, new Resources(AsInt(m, "geo_ore"), 0, AsInt(m, "geo_obs"), 0))
                    );
                blueprints.Add(bp);
            }
            else
            {
                throw new Exception(line);
            }
        }
        var timelimit = 24;
        var quality = 0;
        foreach (var bp in blueprints)
        {
            var stock = new Resources(0, 0, 0, 0);
            var producers = new Resources(1, 0, 0, 0);

            var initialstate = new State(0, stock, producers);
            PriorityQueue<State, int> q = new();
            q.Enqueue(initialstate, 0);
            var geodeCracked = 0;
            int runs = 10_000_000;
            int run = 0;
            while (q.TryDequeue(out var s, out int prio))
            {
                if (run++ >= runs) break;
                if (geodeCracked < s.stock.geode)
                {
                    geodeCracked = s.stock.geode;
                    Console.WriteLine($"time {s.time} stock {s.stock} producers {s.harvesters} run {run}");
                    run = 0;
                }
                if (s.time < timelimit)
                {
                    var max = bp.maxToProduce(s, timelimit);
                    if (max == 0 || max < geodeCracked) continue;
                    foreach (var next in s.Next(bp))
                    {
                        var score = (s.stock.geode * bp.geodeValue * 10) + bp.stockScore(next.stock) + bp.produceScore(next.harvesters, timelimit - s.time) * 3;
                        q.Enqueue(next, -score);
                    }
                }
            }
            Console.WriteLine($"bp {bp.id}: cracked {geodeCracked} runs {run}");
            quality += bp.id * geodeCracked;
        }
        return quality;
    }

    [TestCase("day19.input", 3, ExpectedResult = 5525990)] // 3.267 too low
    [TestCase("day19example1.input", 1, ExpectedResult = 56)]
    [TestCase("day19example1.input", 2, ExpectedResult = 56*62)]
    public int Part2(string input, int take)
    {
        var lines = File.ReadAllLines(input);
        var blueprints = new List<Blueprint>();
        foreach (var line in lines)
        {
            var m = r.Match(line);
            if (m.Success)
            {
                var bp = new Blueprint(AsInt(m, "id"),
                    new Robot(Resource.Ore, new Resources(AsInt(m, "ore_ore"), 0, 0, 0)),
                    new Robot(Resource.Clay, new Resources(AsInt(m, "clay_ore"), 0, 0, 0)),
                    new Robot(Resource.Obsidian, new Resources(AsInt(m, "obs_ore"), AsInt(m, "obs_clay"), 0, 0)),
                    new Robot(Resource.Geode, new Resources(AsInt(m, "geo_ore"), 0, AsInt(m, "geo_obs"), 0))
                    );
                blueprints.Add(bp);
            }
            else
            {
                throw new Exception(line);
            }
        }
        var timelimit = 32;
        var quality = 1;
        foreach (var bp in blueprints.Take(3))
        {
            var stock = new Resources(0, 0, 0, 0);
            var producers = new Resources(1, 0, 0, 0);

            var initialstate = new State(0, stock, producers);
            PriorityQueue<State, int> q = new();
            q.Enqueue(initialstate, 0);
            var geodeCracked = 1;
            int runs = 100_000_000;
            int run = 0;
            while (q.TryDequeue(out var s, out int prio))
            {
                if (run++ >= runs) break;
                if (geodeCracked < s.stock.geode)
                {
                    geodeCracked = s.stock.geode;
                    Console.WriteLine($"time {s.time} stock {s.stock} producers {s.harvesters} run {run}");
                    run = 0;
                }
                if (s.time < timelimit)
                {
                    var max = bp.maxToProduce(s, timelimit);
                    if (max == 0 || (max < geodeCracked)) continue;
                    foreach (var next in s.Next(bp))
                    {
                        var score = (s.stock.geode * bp.geodeValue) + bp.stockScore(next.stock) + bp.produceScore(next.harvesters, timelimit - s.time);
                        q.Enqueue(next, -score);
                    }
                }
            }
            if (q.Count == 0) Console.WriteLine("queue empty");
            Console.WriteLine($"bp {bp.id}: cracked {geodeCracked} runs {run}");
            quality *= geodeCracked;
        }
        return quality;
    }
    public record State(int time, Resources stock, Resources harvesters)
    {
        public IEnumerable<State> Next(Blueprint bp)
        {
            if (bp.ore.CanBeBuild(stock))
            {
                yield return new State(time + 1, bp.ore.Build(stock).Harvest(harvesters), harvesters.AddOreRobot());
            }
            if (bp.clay.CanBeBuild(stock))
            {
                yield return new State(time + 1, bp.clay.Build(stock).Harvest(harvesters), harvesters.AddClayRobot());
            }
            if (bp.obsidian.CanBeBuild(stock))
            {
                yield return new State(time + 1, bp.obsidian.Build(stock).Harvest(harvesters), harvesters.AddObsidianRobot());
            }
            if (bp.geode.CanBeBuild(stock))
            {
                yield return new State(time + 1, bp.geode.Build(stock).Harvest(harvesters), harvesters.AddGeodeRobot());
            }
            yield return new State(time + 1, stock.Harvest(harvesters), harvesters);
        }

    }

    int AsInt(Match m, int pos)
     => int.Parse(m.Groups[pos].Value);
    int AsInt(Match m, string ng)
     => int.Parse(m.Groups[ng].Value);
}
