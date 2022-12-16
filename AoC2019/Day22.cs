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
    public class Day22
    {
        [Test]
        public void Part1()
        {
            var lines = File.ReadAllLines("day22.input");

            var deck = StandardDeck(nrOfCardsStdSpcDeck);
            deck = ExecuteDealInstructions(lines, deck);

            Console.WriteLine(Array.IndexOf(deck, 2019));
        }

        private int[] ExecuteDealInstructions(string[] lines, int[] deck)
        {
            var instructions = new List<(string prefix, Func<int[], int, int[]> function)>
            {
                ("deal with increment ", IncN),
                ("cut ", CutN),
                ("deal into new stack", NewStack),

            };
            foreach (var line in lines)
            {
                foreach (var instr in instructions)
                {
                    if (line.StartsWith(instr.prefix))
                    {
                        if (!int.TryParse(line.Substring(instr.prefix.Length), out var nr))
                        {
                            nr = 0;
                        }
                        deck = instr.function(deck, nr);
                    }
                }
            }

            return deck;
        }

        const int nrOfCardsStdSpcDeck = 10007;

        public int[] NewStack(int[] orig, int dummy=0)
        {
            var arr = new int[orig.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = orig[orig.Length - i - 1];
            }
            return arr;
        }

        public int[] CutN(int[] orig, int n)
        {
            var arr = new int[orig.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = orig[(i + n + orig.Length) % orig.Length];
            }
            return arr;
        }

        public int[] IncN(int[] orig, int n)
        {
            var arr = new int[orig.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[(i * n) % orig.Length] = orig[i];
            }
            return arr;
        }

        public int[] IncNRev(int[] orig, int n)
        {
            var arr = new int[orig.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = orig[(i * n) % orig.Length];
            }
            return arr;
        }

        public int[] StandardDeck(int nrOfCards)
        {
            var arr = new int[nrOfCards];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = i;
            }
            return arr;
        }

        [Test]
        public void Test1std()
        {
            var deck = StandardDeck(10);
            Console.WriteLine(string.Join(",", deck));
        }

        [Test]
        public void Test1NewStack()
        {
            var deck = StandardDeck(10);
            var d1 = NewStack(deck);
            Console.WriteLine(string.Join(",", d1));
        }

        [Test]
        public void Test1Cut3()
        {
            var deck = StandardDeck(10);
            var d1 = CutN(deck, 3);
            Console.WriteLine(string.Join(",", d1));
        }

        [Test]
        public void Test1Cutm4()
        {
            var deck = StandardDeck(10);
            var d1 = CutN(deck, -4);
            Console.WriteLine(string.Join(",", d1));
        }

        [Test]
        public void Test1Inc3()
        {
            var deck = StandardDeck(10);
            var d1 = IncN(deck, 3);
            Console.WriteLine(string.Join(",", d1));
        }

        [Test]
        public void Test1()
        {
            var deck = StandardDeck(10);
            var d1 = IncN(deck, 7);
            var d2 = NewStack(d1);
            var d3 = NewStack(d2);
            Console.WriteLine(string.Join(",", d3));
        }

        [Test]
        public void Test2()
        {
            var deck = StandardDeck(10);
            var d1 = CutN(deck, 6);
            var d2 = IncN(d1, 7);
            var d3 = NewStack(d2);
            Console.WriteLine(string.Join(",", d3));
        }

        [Test]
        public void Test3()
        {
            var deck = StandardDeck(10);
            var d1 = IncN(deck, 7);
            var d2 = IncN(d1, 9);
            var d3 = CutN(d2, -2);
            Console.WriteLine(string.Join(",", d3));
        }

        [Test]
        public void Test1parse()
        {
            var deck = StandardDeck(10);
            string[] lines =
            {
                "deal with increment 7",
                "deal into new stack",
                "deal into new stack",
            };
            deck = ExecuteDealInstructions(lines, deck);
            Console.WriteLine(string.Join(",", deck));
        }

        [Test]
        public void Test2parse()
        {
            var deck = StandardDeck(10);
            string[] lines =
            {
                "cut 6",
                "deal with increment 7",
                "deal into new stack",
            };
            deck = ExecuteDealInstructions(lines, deck);
            Console.WriteLine(string.Join(",", deck));
        }

        [Test]
        public void Test3parse()
        {
            var deck = StandardDeck(10);
            string[] lines =
            {
                "deal with increment 7",
                "deal with increment 9",
                "cut -2",
            };
            deck = ExecuteDealInstructions(lines, deck);
            Console.WriteLine(string.Join(",", deck));
        }

        [Test]
        public void TestMult()
        {
            var d0 = StandardDeck(10007);
            var d = d0;
            d = CutN(d, 1);
            d = IncN(d, 7);
            d = CutN(d, 13);
            d = IncN(d, 9);
            d = CutN(d, 19);

            var e0 = StandardDeck(10007);
            var e = e0;
            e = IncN(e, 7*9);
            e = CutN(e, 1*7*9 + 13*9 + 19);

            Console.WriteLine(string.Join(",", d));
            Console.WriteLine(string.Join(",", e));
            Assert.AreEqual(d, e);
        }

        [Test]
        public void TestMultNewStack()
        {
            var d0 = StandardDeck(10007);
            var d = d0;
            d = CutN(d, 1);
            d = IncN(d, 7);
            d = CutN(d, 13);
            d = NewStack(d);
            d = IncN(d, 11);
           // d = CutN(d, 19);

            var e0 = StandardDeck(10007);
            var e = e0;
            e = IncN(e, 7 * 11);
            e = CutN(e, 1 * 7 * 11 + 13 * 11 - 10 ); // 10  = 11 - 1 ??
            e = NewStack(e);

            Console.WriteLine(string.Join(",", d));
            Console.WriteLine(string.Join(",", e));
            Assert.AreEqual(d, e);
        }

        [Test]
        public void TestMult2NewStack()
        {
            var d0 = StandardDeck(10007);
            var d = d0;
            d = CutN(d, 1);
            d = NewStack(d);
            d = IncN(d, 7);
            d = CutN(d, 13);
            d = NewStack(d);
            d = IncN(d, 11);
            // d = CutN(d, 19);

            var e0 = StandardDeck(10007);
            var e = e0;
            e = IncN(e, 7 * 11);
            e = CutN(e, 1 * 7 * 11 + 13 * 11 - 10); // 10  = 11 - 1 ??
            e = NewStack(e);

            Console.WriteLine(string.Join(",", d));
            Console.WriteLine(string.Join(",", e));
            Assert.AreEqual(d, e);
        }
    }
}
