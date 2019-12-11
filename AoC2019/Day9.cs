using LanguageExt;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AoC2019Test.Util;

namespace AoC2019Test
{
    public partial class Day9
    {
        [Test]
        public void Part1()
        {
            var program = File.ReadAllLines("day9.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();

            var p = new IntCodeComputer(program);
            p.Execute(new bigint[] { 1 }.ToList());
            var diagCode = p.Output.Last();

            Console.WriteLine($"{string.Join(",", p.Output)}");
            Assert.AreEqual(1, p.Output.Count);
            Assert.AreEqual(2316632620, (long)p.Output.First());
        }

        [Test]
        public void Part2()
        {
            var program = File.ReadAllLines("day9.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();

            var p = new IntCodeComputer(program);
            p.Execute(new bigint[] { 2 }.ToList());
            var diagCode = p.Output.Last();

            Console.WriteLine($"{string.Join(",", p.Output)}");
            Assert.AreEqual(1, p.Output.Count);
            Assert.AreEqual(78869, (int)p.Output.First());
        }

        [Test]
        public void Part1Example1()
        {
            var c = new IntCodeComputer(new bigint[] { 109, 1, 204, -1, 1001, 100, 1, 100, 1008, 100, 16, 101, 1006, 101, 0, 99 });
            var x = c.Execute( new List<bigint> { });
            Console.WriteLine(string.Join(",", c.Output));
        }

        [Test]
        public void Part1Example2()
        {
            var c = new IntCodeComputer(new bigint[] { 1102, 34915192, 34915192, 7, 4, 7, 99, 0 });
            var x = c.Execute(new List<bigint> { });
            Console.WriteLine(string.Join(",", c.Output));
        }

        [Test]
        public void Part1Example3()
        {
            var c = new IntCodeComputer(new bigint[] { 104, 1125899906842624, 99 });
            var x = c.Execute(new List<bigint> { });
            Console.WriteLine(string.Join(",", c.Output));
        }
    }
}
