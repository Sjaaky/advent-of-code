using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AoC2019Test.Util;

namespace AoC2019Test
{
    public partial class Day7
    {
        [Test]
        public void Part1()
        {
            var program = File.ReadAllLines("day7.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(int.Parse)
                .ToArray();

            var maxThrust = AllPhaseSettings(0, 5).Max(ps => Amplifiers(program, ps));

            Console.WriteLine($"{maxThrust}");
            Assert.AreEqual(19650, maxThrust);
        }

        public int Amplifiers(int[] program, int[] phaseSettings)
        {
            var signal = 0;
            for (int i = 0; i < phaseSettings.Length; i++)
            {
                var p = new IntCodeComputer(program);
                p.Execute(new[] { phaseSettings[i], signal }.ToList());
                Console.WriteLine($"Step {i} {phaseSettings[i]} {signal} => {string.Join(",", p.Output)}");
                signal = p.Output.First();
            }
            Console.WriteLine($"Phasesetting {string.Join(",", phaseSettings)} => {signal}");
            return signal;
        }

        [Test]
        public void Part2()
        {
            var program = File.ReadAllLines("day7.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(int.Parse)
                .ToArray();

            var maxThrust = AllPhaseSettings(5, 10).Max(ps => AmplifiersWithFeedBack(program, ps));

            Assert.AreEqual(35961106, maxThrust);
            Console.WriteLine($"{maxThrust}");
        }

        public int AmplifiersWithFeedBack(int[] program, int[] phaseSettings)
        {
            IntCodeComputer[] p = Range(0, 4).Select(_ => new IntCodeComputer(program, true)).ToArray();
            List<int>[] inputs = Range(0, 4).Select(i => new List<int>() { phaseSettings[i] }).ToArray();

            var signal = 0;
            int iter = 0;
            while (!p[4].IsHalted)
            {
                for (int i = 0; i < phaseSettings.Length; i++)
                {
                    inputs[i].Add(signal);
                    p[i].Execute(inputs[i]);
                    Console.WriteLine($"Step {iter} {i} {phaseSettings[i]} {signal} => {string.Join(",", p[i].Output)}");
                    signal = p[i].Output.Last();
                }
                iter++;
            }
            Console.WriteLine($"Phasesetting iters{iter} {string.Join(",", phaseSettings)} => {signal}");
            return signal;
        }

        private IEnumerable<int[]> AllPhaseSettings(int min, int max)
        {
            int[] ps = new int[5];
            for (ps[0] = min; ps[0] < max; ps[0]++)
                for (ps[1] = min; ps[1] < max; ps[1]++)
                    for (ps[2] = min; ps[2] < max; ps[2]++)
                        for (ps[3] = min; ps[3] < max; ps[3]++)
                            for (ps[4] = min; ps[4] < max; ps[4]++)
                            {
                                if (ps.GroupBy(p => p).All(g => g.Count() <= 1))
                                {
                                    yield return ps;
                                }
                            }
        }

        [Test]
        public void Part1Example1()
        {
            var x = Amplifiers(new[] { 3, 15, 3, 16, 1002, 16, 10, 16, 1, 16, 15, 15, 4, 15, 99, 0, 0 }, new[] { 4, 3, 2, 1, 0 });
            Assert.AreEqual(43210, x);
        }

        [Test]
        public void Part1Example2()
        {
            var x = Amplifiers(new[] { 3, 23, 3, 24, 1002, 24, 10, 24, 1002, 23, -1, 23, 101, 5, 23, 23, 1, 24, 23, 23, 4, 23, 99, 0, 0 }, new[] { 0, 1, 2, 3, 4 });
            Assert.AreEqual(54321, x);
            Console.WriteLine(x);
        }

        [Test]
        public void Part1Example3()
        {
            var x = Amplifiers(new[] { 3, 31, 3, 32, 1002, 32, 10, 32, 1001, 31, -2, 31, 1007, 31, 0, 33, 1002, 33, 7, 33, 1, 33, 31, 31, 1, 32, 31, 31, 4, 31, 99, 0, 0, 0 }, new[] { 1, 0, 4, 3, 2 });
            Assert.AreEqual(65210, x);
            Console.WriteLine(x);
        }

        [Test]
        public void Part2Example1()
        {
            var x = AmplifiersWithFeedBack(new[] { 3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5 }, 
                new[] { 9, 8, 7, 6, 5 });
            Assert.AreEqual(139629729, x);
            Console.WriteLine(x);
        }

        [Test]
        public void Part2Example2()
        {
            var x = AmplifiersWithFeedBack(new[] {3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,
                        -5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,
                        53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10},
                new[] { 9, 7, 8, 5, 6 });
            Assert.AreEqual(18216, x);
            Console.WriteLine(x);
        }
    }
}
