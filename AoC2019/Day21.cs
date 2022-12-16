using LanguageExt;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static AoC2019Test.Util;

namespace AoC2019Test
{
    public class Day21
    {
        [Test]
        public void Part1()
        {
            var program = File.ReadAllLines("day21.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();

            var conds = new System.Collections.Generic.HashSet<string>();

            string[] inputs =
            {
                "WALK\n"
            ,
                "NOT C J\n" +
                "WALK\n"
            ,
                "NOT C J\n" +
                "AND D J\n" +
                "WALK\n"
            ,
                "NOT C J\n" +
                "AND D J\n" +
                "NOT A T\n" +
                "OR T J\n" +
                "WALK\n"
            };

            foreach (var input in inputs)
            {
                var (d, f) = Run(program, input);
                if (d == 0)
                {
                    conds.Add(f);
                }
            }

            Console.WriteLine("conds:");
            foreach (var c in conds)
            {
                Console.WriteLine(c);
            }
        }

        private static (bigint damage, string failure) Run(bigint[] program, string input)
        {
            var c = new IntCodeComputer(program, false);
            c.Execute(input.Select(c => (bigint)c).ToList());

            Console.WriteLine($"{c.InstructionsExecuted}");

            int ptr = 0;
            string failure = null;
            bigint damage = 0;
            var sb = new StringBuilder();
            foreach (var ch in c.Output)
            {
                if (ch > 128)
                {
                    sb.AppendLine();
                    sb.AppendLine($"DAMAGE {ch}");
                    damage = ch;
                }
                else
                {
                    sb.Append((char)ch);
                    if ((char)ch == '#' && failure == null)
                    {
                        failure = new string(c.Output.Skip(ptr).Take(17).Select(i => (char)i).ToArray());
                    }
                }
                ptr++;

            }
            File.AppendAllText(".\\day21part2.log", "\"" + input.Replace("\n", "\\n\" +\r\n\"") + "\r\n" + sb.ToString() + "\r\n" + "===================\r\n");

            return (damage, failure);
        }

        [Test]
        public void Part2()
        {
            var program = File.ReadAllLines("day21.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();

            var conds = new System.Collections.Generic.HashSet<string>();


            string[] inputs =
            {
                "NOT C J\n" +
                "RUN\n"
            ,
                "NOT C J\n" +
                "AND D J\n" +
                "NOT A T\n" +
                "OR T J\n" +
                "RUN\n"
            ,
                "NOT C J\n" +

                "NOT E T\n" +
                "AND A T\n" +
                "AND B T\n" +
                "AND C T\n" +
                "AND H T\n" +
                "OR T J\n" +

                "AND D J\n" +
                "NOT A T\n" +
                "OR T J\n" +

                "RUN\n"
            ,
                "NOT C J\n" +

                "NOT E T\n" +
                "AND A T\n" +
                "AND B T\n" +
                "AND C T\n" +
                "AND H T\n" +
                "OR T J\n" +

                "NOT A T\n" +
                "OR B T\n" +
                "OR E T\n" +
                "NOT T T\n" +
                "OR T J\n" +

                "NOT A T\n" +
                "OR T J\n" +

                "RUN\n"
            ,
                "NOT C J\n" +

                "NOT E T\n" +
                "AND A T\n" +
                "AND B T\n" +
                "AND C T\n" +
                "AND H T\n" +
                "OR T J\n" +

                "AND J T\n" +
                "OR B T\n" +
                "OR E T\n" +
                "NOT T T\n" +
                "OR T J\n" +

                "NOT A T\n" +
                "OR T J\n" +

                "RUN\n"
            ,

                "NOT J J\n" + // j = true
                "AND A J\n" +
                "AND B J\n" +
                "AND C J\n" +
                "NOT J J\n" + 
                "NOT T T\n" + // t = true
                "AND E T\n" + 
                "AND F T\n" +
                "AND I T\n" +
                "NOT T T\n" +
                "AND T J\n" +
                "AND D J\n" +
                "AND H J\n" +

                "NOT A T\n" +
                "OR T J\n" +

                "RUN\n"

            };
            foreach (var input in inputs)
            {
                var (d, f) = Run(program, input);
                if (d == 0)
                {
                    conds.Add(f);
                }
            }

            Console.WriteLine("conds:");
            foreach (var c in conds)
            {
                Console.WriteLine("__ABCDEFGH");
                Console.WriteLine(c);
            }

        }

        [Test]
        public void T1()
        {
            //                           __ABCDEFGH
            Assert.AreEqual(false, Jump("#####...#########"));
        }
        [Test]
        public void T2()
        {

            //                           __ABCDEFGH
            Assert.AreEqual(false, Jump("####...##########"));
        }
        [Test]
        public void T3()
        {
            //                          __ABCDEFGH
            Assert.AreEqual(false, Jump("###...###########"));
        }
        [Test]
        public void T4()
        {
            //                          __ABCDEFGH
            Assert.AreEqual(true, Jump("##...############"));
        }

        [Test]
        public void Ta1()
        {
            //                           __ABCDEFGH
            Assert.AreEqual(false, Jump("#####.#.##..#####"));
        }
        [Test]
        public void Ta2()
        {
            //                           __ABCDEFGH
            Assert.AreEqual(false, Jump("####.#.##..######"));
        }
        [Test]
        public void Ta3()
        {
            //                           __ABCDEFGH
            Assert.AreEqual(false, Jump("###.#.##..#######"));
        }
        [Test]
        public void Ta4()
        {
            //                          __ABCDEFGH
            Assert.AreEqual(true, Jump("##.#.##..#########"));
        }
        [Test]
        [Ignore("true and false both valid")]
        public void Ta5()
        {
            //                          __ABCDEFGH
            Assert.AreEqual(true, Jump(".##..#############"));
        }
        [Test]
        public void Ta6()
        {
            //                          __ABCDEFGH
            Assert.AreEqual(true, Jump("##..##############"));
        }

        [Test]
        public void Tb1()
        {
            //                           __ABCDEFGH
            Assert.AreEqual(false, Jump("#####.##.##...###"));
        }
        [Test]
        public void Tb2()
        {
            //                          __ABCDEFGH
            Assert.AreEqual(true, Jump("####.##.##...###"));
        }
        [Test]
        public void Tb3()
        {
            //                           __ABCDEFGH
            Assert.AreEqual(false, Jump("###.##.##...###"));
        }
        [Test]
        [Ignore("too late")]
        public void Tb4()
        {
            //                           __ABCDEFGH
            Assert.AreEqual(false, Jump("##.##.##...###"));
        }
        [Test]
        [Ignore("too late")]
        public void Tb5()
        {
            //                          __ABCDEFGH
            Assert.AreEqual(true, Jump("#.##.##...###"));
        }
        [Test]
        public void Tb6()
        {
            //                          __ABCDEFGH
            Assert.AreEqual(true, Jump(".##.##...###"));
        }

        [Test]
        public void Tc1()
        {
            //                          __ABCDEFGH
            Assert.AreEqual(false, Jump("######..##..#.####"));
        }

        [Test]
        public void Tc2()
        {
            //                          __ABCDEFGHI
            Assert.AreEqual(true, Jump("###.####.#..##"));
        }

        private bool Jump(string x)
        {
            var a = !C('A', x);
            var b = (!(C('A', x) && C('B', x) && C('C', x)) && !(C('E', x) && C('F', x) && C('I', x)) && C('D', x) && C('H', x));
            return (a || b);
        }

        private bool C(char c, string x)
        {
            var i = c - 'A' + 2;
            return x[i] == '#';
        }
    }
}
