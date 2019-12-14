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
            var ore = CalcOre(lines);
            Assert.IsTrue(ore < 401671);
        }

        [Test]
        public void Part1t0()
        {
            var lines = File.ReadAllLines("day14.test0.input");
            var ore = CalcOre(lines);
            Assert.AreEqual(31, ore);
        }
        [Test]
        public void Part1t1()
        {
            var lines = File.ReadAllLines("day14.test1.input");
            var ore = CalcOre(lines);
            Assert.AreEqual(165, ore);
        }
        [Test]
        public void Part1t2()
        {
            var lines = File.ReadAllLines("day14.test2.input");
            var ore = CalcOre(lines);
            Assert.AreEqual(13312, ore);
        }

        [Test]
        public void Part1t3()
        {
            var lines = File.ReadAllLines("day14.test3.input");
            var ore = CalcOre(lines);
            Assert.AreEqual(180697, ore);
        }

        [Test]
        public void Part1t4()
        {
            var lines = File.ReadAllLines("day14.test4.input");
            var ore = CalcOre(lines);
            Assert.AreEqual(2210736, ore);
        }

        //[Test]
        //public void Part1t5()
        //{
        //    var lines = File.ReadAllLines("day14.test5.input");
        //    var ore = CalcOre(lines);
        //    Assert.AreEqual(180697, ore);
        //}

        private int CalcOre(string[] lines)
        {
            var formulas = ReadFormulas(lines);

            var fuel = formulas["FUEL"];
            var leftovers = new Dictionary<string, int>();
            var needs = new Dictionary<string, int>();
            needs["FUEL"] = 1;

            var step = 0;
            while (!needs.All(n => n.Key == "ORE"))
            {
                Console.WriteLine($"=== step {step} ====");
                foreach (var n in needs)
                {
                    Console.WriteLine($"Need {n.Value} {n.Key}");
                }
                foreach (var l in leftovers)
                {
                    Console.WriteLine($"LeftOver {l.Value} {l.Key}");
                }
                //Console.WriteLine($"=^= step {step} =^=");

                var need = needs.First(n => n.Key != "ORE");
                var form = formulas[need.Key];
                Console.WriteLine(form);
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
                    //if (neededAmount == 0)
                    {
                        var lo1 = leftovers[o.Item2];
                        leftovers[o.Item2] = leftOver;
                        var lo2 = leftOver;

                        //Console.WriteLine($"leftOver {o.Item2} {lo1} => {lo2}");
                    }
                    Assert.IsTrue(leftovers[o.Item2] >= 0);
                    Assert.IsTrue(delta >= 0);
                    Assert.IsTrue(neededAmount >= 0);
                    Assert.IsTrue(leftOver >= 0);

                }
                Console.WriteLine($"need {need.Value} of {need.Key}. {leftOverOrig} leftovers, stillneeded={neededAmount}, leftOVer{leftOver} ");
                if (neededAmount > 0)
                {
                    var steps = neededAmount / formulaQuantity;
                    var extra = (neededAmount % formulaQuantity);
                    if (extra > 0)
                    {
                        steps++;
                    }

                    if (!leftovers.ContainsKey(o.Item2)) leftovers[o.Item2] = 0;
                    var lo1 = leftovers[o.Item2];
                    leftovers[o.Item2] += steps*formulaQuantity-neededAmount;

                    Assert.IsTrue(leftovers[o.Item2] >= 0);
                    Console.WriteLine($"leftOver {o.Item2}. {lo1} || {steps * formulaQuantity} - {neededAmount} => {leftovers[o.Item2]}");
                        
                    Console.WriteLine($" create {neededAmount} of {need.Key}. steps={steps} x amount={formulaQuantity}  {extra} ");

                    foreach (var i in form.In)
                    {
                        if (!needs.ContainsKey(i.Item2)) needs[i.Item2] = 0;
                        var produce = i.Item1 * steps;
                        needs[i.Item2] += produce;
                        //Console.WriteLine($"  - create {produce} of {i.Item2}. => {needs[i.Item2]}");
                    }
                }
                needs.Remove(o.Item2);

                //Console.WriteLine("---");
                foreach (var n in needs)
                {
                    //Console.WriteLine($" > Need {n.Value} {n.Key}");
                }
                foreach (var l in leftovers)
                {
                    //Console.WriteLine($" > LeftOver {l.Value} {l.Key}");
                }
                step++;

            }
            Console.WriteLine(needs["ORE"]);
            return needs["ORE"];
        }

        [Test]
        public void Part2()
        {
            var lines = File.ReadAllLines("day14.input");
         
        }

        Regex regex = new Regex(@"((?<innr>\d+) (?<inelm>\w+),? ?)+=> (?<outnr>\d+) (?<outelm>\w+)");
        public Dictionary<string, Formula> ReadFormulas(string[] lines)
        {
            var objects = new Dictionary<string, Formula>();
            var formulas = new Dictionary<string, Formula>();
            foreach (var line in lines)
            {
                var m = regex.Match(line);

                var ins = m.Groups["innr"].Captures.Select(v => int.Parse(v.Value)).Zip(m.Groups["inelm"].Captures.Select(v => v.Value));//.ToArray<Capture>().Select(c => (int.Parse(c.Groups["nr"]), c["elm"]));
                if (ins.Count() != line.Count(c => c == ',') +1)
                {
                    throw new Exception();
                }
                var outs = m.Groups["outnr"].Captures.Select(v => int.Parse(v.Value)).Zip(m.Groups["outelm"].Captures.Select(v => v.Value));//.ToArray<Capture>().Select(c => (int.Parse(c.Groups["nr"]), c["elm"]));
                formulas.Add(outs.First().Item2, new Formula { In = ins.ToList(), Out = outs.ToList() });
            }

            return formulas;
        }

        [Test]
        public void Example1()
        {
          
        }

        public class Formula
        {
            public List<(int, string)> In;
            public List<(int, string)> Out;

            public override string ToString()
            {
                return In.Aggregate("", (a, i) => a + $"{i.Item1} {i.Item2},") + $"=> " + Out.Aggregate("", (a, i) => a + $"{ i.Item1} { i.Item2},");
            }
        }
    }
}
