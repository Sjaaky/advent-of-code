using FluentAssertions.Execution;
using System.Text.RegularExpressions;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;
using System.Linq;
using System.Threading.Tasks;

namespace AoC2022;

public class Day16
{
    //[Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(16);
    }

    [SetUp]
    public void setup()
    {
        var lines = File.ReadAllLines("day16.input");
        r.Match("x");
        Node.sid = 0;
        Node.smask = 1;
    }

    record Node(string Label, int Flow, List<Node> Neighbours)
    {
        public override string ToString()
         => $"{Label} {Flow} {string.Join("|", Neighbours?.Select(n => n.Label))}";

        private int[] _release
         = Enumerable.Range(0, 30).Select(r => (30-r) * Flow).ToArray();

        public int? release(int time)
        {
            if (time < 30) return _release[time];
            return null;
        }

        public long mask = smask <<= 1;
        public static long smask = 1;

        public int id = sid++;
        public static int sid = 0;
    }

    Regex r = new Regex(@"Valve (\w{2}) has flow rate=(\d+); tunnels? leads? to valves? (?:(\w{2})(?:, )?)*", RegexOptions.Compiled);

    [TestCase("day16.input", ExpectedResult = 1862)] //1606 too low, !1885 
    [TestCase("day16example1.input",  ExpectedResult = 1651)]
    public int Part1(string input)
    {
        var lines = File.ReadAllLines(input);
        var maxtime = 30;
        var dict = lines.Select(ToNode).ToDictionary(n => n.Label);

        foreach (var line in lines)
        {
            var m = r.Match(line);
            if (m.Success)
            {
                var src = m.Groups[1].Value;
                var dest = m.Groups[3].Captures.Select(c => dict[c.Value]).ToList();
                dict[src].Neighbours.AddRange(dest);
            }
            else
            {
                throw new Exception(line);
            }
        }
        var shortestpath = AllPairsShortestPath(dict);
        int size = dict.Count();
        string[] keys = dict.Keys.ToArray();
        long allMask = dict.Aggregate(0L, (a, b) => a |= b.Value.mask);
        long valveMask = dict.Aggregate(0L, (a, b) => a |= (b.Value.Flow > 0) ? b.Value.mask : 0);
        int result = 0;
        Node[] valves = dict.Where(k => k.Value.Flow > 0).Select(k => k.Value).ToArray();
        PriorityQueue<S, int> q = new();
        q.Enqueue(new S(dict["AA"], dict["AA"].mask, 0, 0), int.MaxValue);
        while (q.TryDequeue(out var c, out int prio))
        {
            result = Math.Max(result, c.released);
            var now = c.time;
            if ((c.visited & valveMask) == valveMask) continue;

            var unvisited = Unvisited(valves, c.visited)
                .Select(u => (node: u, heuristic: Heuristic(u, shortestpath, c.node, now, maxtime)))
                .Where(u => u.heuristic.HasValue)
                .OrderByDescending(u => u.heuristic.Value).ToArray();

            var left = unvisited
                .Select((h, i) => (h: h, time: i * 2))
                .Where(u => u.h.heuristic.HasValue && now + u.time < maxtime)
                .Sum(u => u.h.heuristic ?? 0);

            if (now < 30 && c.released + left > result)
            {
                foreach (var unvis in unvisited)
                {
                    var walkdist = shortestpath[c.node.id, unvis.node.id];
                    var dt = walkdist + 1;
                    var next = now + dt;
                    var node = unvis.node;
                    var mask = c.visited | node.mask;
                    var heur = unvis.heuristic;
                    var score = c.released + node.release(next);
                    if (score.HasValue)
                    {
                        var state = new S(node, mask, next, score.Value);
                        q.Enqueue(state, -score.Value);
                    }
                }
            }
        }
        return result;
    }

    [TestCase("day16.input", ExpectedResult = 2422)] //2033,2079 too low,2328 incorrect,  //2185 not correct
    [TestCase("day16example1.input", ExpectedResult = 1707)]
    public int Part2(string input)
    {
        var lines = File.ReadAllLines(input);
        var maxtime = 30;
        var dict = lines.Select(ToNode).ToDictionary(n => n.Label);

        foreach (var line in lines)
        {
            var m = r.Match(line);
            if (m.Success)
            {
                var src = m.Groups[1].Value;
                var dest = m.Groups[3].Captures.Select(c => dict[c.Value]).ToList();
                dict[src].Neighbours.AddRange(dest);
            }
            else
            {
                throw new Exception(line);
            }
        }
        var shortestpath = AllPairsShortestPath(dict);
        int size = dict.Count();
        string[] keys = dict.Keys.ToArray();
        long allMask = dict.Aggregate(0L, (a, b) => a |= b.Value.mask);
        long valveMask = dict.Aggregate(0L, (a, b) => a |= (b.Value.Flow > 0) ? b.Value.mask : 0);
        int result = 0;
        Node[] valves = dict.Where(k => k.Value.Flow > 0).Select(k => k.Value).ToArray();

        PriorityQueue<S2, int> q = new();
        q.Enqueue(new S2(new P(dict["AA"], 4), new P(dict["AA"], 4), dict["AA"].mask, 0), int.MaxValue);
        while (q.TryDequeue(out var c, out int prio))
        {
            result = Math.Max(result, c.released);
            int i = 0;
            if ((c.visited & valveMask) == valveMask) continue;

            var unvisited = Unvisited(valves, c.visited)
                .Select(u => Heuristic(u, shortestpath, c, maxtime))
                .Where(u => u.score > 0)
                .OrderByDescending(u => u.score.Value)
                .ToArray();

            var left = unvisited
                .Select((h, i) => (h: h, time:(i*2)))
                .Where(u => u.h.score.HasValue && c.p1.time + u.time < maxtime)
                .Sum(u => u.h.score ?? 0);

            if (c.released + left > result)
            {
                foreach (var unvis in unvisited)
                {
                    var next = unvis.time;
                    var node = unvis.u;
                    var released = unvis.score.Value;

                    var newreleased = c.released + released;
                    var mask = c.visited | node.mask;
                    var newplace = new P(node, next);
                    var other = c.p2;
                    if (next > other.time)
                    {
                        var state = new S2(other, newplace, mask, newreleased);
                        q.Enqueue(state, - (newreleased));
                    }
                    else
                    {
                        var state = new S2(newplace, other, mask, newreleased);
                        q.Enqueue(state, - (newreleased));
                    }
                }
            }
            i++;
        }
        return result;
    }

    private static int? Heuristic(Node u, int[,] shortestpath, Node c, int now, int maxtime)
    {
        var distance = shortestpath[c.id, u.id];
        var r = u.release(now + distance +1);
        return r;
    }

    record Heuristics(Node u, int idx, P p, int? score, int time);
    private static Heuristics Heuristic(Node u, int[,] shortestpath, S2 ps, int maxtime)
    {
        var p = ps.p1;
        var now = p.time;
        var distance = shortestpath[p.node.id, u.id];
        var next = now + distance + 1;
        if (next < maxtime)
        {
            var r = u.release(next);
            return new(u, -1, p, r, next);
        }
        return new(u, -1, p, (int?)null, 40);
    }

    private IEnumerable<Node> Unvisited(Node[] valves, long visited)
    {
        foreach (var node in valves)
        {
            if ((node.mask & visited) == 0)
            {
                yield return node;
            }
        }
    }

    record S(Node node, long visited, int time, int released)
    {

    }

    record P(Node node, int time);
    record S1(P p1, long visited, int released)
    {
        public P Get(int i) => p1;
        public IEnumerable<P> All()
        {
            yield return p1;
        }
    }

    record S2(P p1, P p2, long visited, int released)
    {
        public P Get(int i) => i switch { 0 => p1, 1 => p2 };
        public IEnumerable<P> All()
        {
            yield return p1;
            yield return p2;
        }
    }

    private int[,] AllPairsShortestPath(Dictionary<string, Node> dict)
    {
        var result = new int[dict.Count(), dict.Count()].Init(int.MaxValue);
        foreach (var src in dict)
        {
            ShortestPathToAll(result, src.Value);
        }
        return result;
    }

    private void ShortestPathToAll(int[,] result, Node src)
    {
        result[src.id, src.id] = 0;
        Queue<Node> q = new Queue<Node>(new[] { src });
        while (q.TryDequeue(out var current))
        {
            var d = result[src.id, current.id] + 1;
            foreach (var n in current.Neighbours)
            {
                if (result[src.id, n.id] > d)
                {
                    result[src.id, n.id] = d;
                    q.Enqueue(n);
                }
            }
        }
    }

    private Node ToNode(string line)
    {
        var m = r.Match(line);
        if (m.Success)
        {
            var src = m.Groups[1].Value;
            var flow = int.Parse(m.Groups[2].Value);
            var n = new Node(src, flow, new List<Node>());
            return n;
        }
        else
        {
            throw new Exception(line);
        }
    }
}
