using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC2019Test
{
    public partial class Day14
    {
        [Test]
        public void Part1()
        {
            var lines = File.ReadAllLines("day14.input");
            var ore = CalcOre(lines, 1);
            Assert.IsTrue(ore < 401671);
        }

        [Test]
        public void Part2()
        {
            var lines = File.ReadAllLines("day14.input");

            long targetOre = 1000000000000;
            long fuel = 1;
            long ore;
            long factor;
            do
            {
                ore = CalcOre(lines, fuel);

                var delta = targetOre - ore;
                factor = (ore / fuel);
                var inc = (delta / factor);
                if (inc == 0)
                {
                    inc = Math.Sign(delta);
                }

                fuel += inc;
            }
            while (ore <= targetOre);
            ore = CalcOre(lines, fuel);
            Console.WriteLine($"DONE ORE={ore} => FUEL={fuel}");
            Assert.AreEqual(3126714, fuel);
        }

        private long CalcOre(string[] lines, long amountOfFuel)
        {
            var formulas = ReadFormulas(lines);

            var leftovers = new Dictionary<string, long>();
            var needs = new Dictionary<string, long>();
            var step = 0;

            needs["FUEL"] = amountOfFuel;
            while (!needs.All(n => n.Key == "ORE"))
            {
                var need = needs.First(n => n.Key != "ORE");
                var form = formulas[need.Key];
                var o = form.Out.FirstOrDefault(o => o.Item2 == need.Key);
                var formulaQuantity = o.Item1;
                var neededAmount = need.Value;
                var leftOver = leftovers.GetValueOrDefault(o.Item2, 0);
                var leftOverOrig = leftOver;
                if (leftOver > 0)
                {
                    var delta = Math.Min(neededAmount, leftOver);
                    neededAmount -= delta;
                    leftOver -= delta;
                    leftovers[o.Item2] = leftOver;

                }
                if (neededAmount > 0)
                {
                    var steps = neededAmount / formulaQuantity;
                    var extra = (neededAmount % formulaQuantity);
                    if (extra > 0)
                    {
                        steps++;
                    }

                    if (!leftovers.ContainsKey(o.Item2)) leftovers[o.Item2] = 0;
                    leftovers[o.Item2] += steps*formulaQuantity-neededAmount;


                    foreach (var i in form.In)
                    {
                        if (!needs.ContainsKey(i.Item2)) needs[i.Item2] = 0;
                        var produce = i.Item1 * steps;
                        needs[i.Item2] += produce;
                    }
                }
                needs.Remove(o.Item2);
                step++;
            }
            return needs["ORE"];
        }

        Regex regex = new Regex(@"((?<innr>\d+) (?<inelm>\w+),? ?)+=> (?<outnr>\d+) (?<outelm>\w+)");
        public Dictionary<string, Formula> ReadFormulas(string[] lines)
        {
            var formulas = new Dictionary<string, Formula>();
            foreach (var line in lines)
            {
                var m = regex.Match(line);

                var ins = m.Groups["innr"].Captures.Select(v => long.Parse(v.Value)).Zip(m.Groups["inelm"].Captures.Select(v => v.Value));
                var outs = m.Groups["outnr"].Captures.Select(v => long.Parse(v.Value)).Zip(m.Groups["outelm"].Captures.Select(v => v.Value));
                formulas.Add(outs.First().Item2, new Formula { In = ins.ToList(), Out = outs.ToList() });
            }

            return formulas;
        }

        public class Formula
        {
            public List<(long, string)> In;
            public List<(long, string)> Out;

            public override string ToString()
            {
                return In.Aggregate("", (a, i) => a + $"{i.Item1} {i.Item2},") + $"=> " + Out.Aggregate("", (a, i) => a + $"{ i.Item1} { i.Item2},");
            }
        }


        [Test]
        public void Part1t0()
        {
            var lines = File.ReadAllLines("day14.test0.input");
            var ore = CalcOre(lines, 1);
            Assert.AreEqual(31, ore);
        }
        [Test]
        public void Part1t1()
        {
            var lines = File.ReadAllLines("day14.test1.input");
            var ore = CalcOre(lines, 1);
            Assert.AreEqual(165, ore);
        }
        [Test]
        public void Part1t2()
        {
            var lines = File.ReadAllLines("day14.test2.input");
            var ore = CalcOre(lines, 1);
            Assert.AreEqual(13312, ore);
        }

        [Test]
        public void Part1t3()
        {
            var lines = File.ReadAllLines("day14.test3.input");
            var ore = CalcOre(lines, 1);
            Assert.AreEqual(180697, ore);
        }

        [Test]
        public void Part1t4()
        {
            var lines = File.ReadAllLines("day14.test4.input");
            var ore = CalcOre(lines, 1);
            Assert.AreEqual(2210736, ore);
        }
    }
}
