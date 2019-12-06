using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2019Test
{
    public partial class Day5
    {
        [Test]
        public void Part1()
        {
            var program = File.ReadAllLines("day5.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(int.Parse)
                .ToArray();
            var p = new IntCodeComputer(program);
            var result = p.Execute(new[] { 1 });

            Console.WriteLine(result);
            Console.WriteLine($"output = {string.Join(',', p.Output)}");

            Assert.AreEqual(7265618, p.Output.Last());
        }

        [Test]
        public void Part2()
        {
            var program = File.ReadAllLines("day5.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(int.Parse)
                .ToArray();
            var p = new IntCodeComputer(program);
            var result = p.Execute(new[] { 5 });

            Console.WriteLine(result);
            Console.WriteLine($"output = {string.Join(',', p.Output)}");
        
            Assert.AreEqual(7731427, p.Output[0]);
        }
    }
}
