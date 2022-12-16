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
    public class Day22part1
    {
        [Test]
        public void Part1()
        {
            var lines = File.ReadAllLines("day22.input");
            long nrOfCardsStdSpcDeck = 10007;

            var pos = TrackPosition(lines, 4649);
            Console.WriteLine(pos);

            pos = TrackPosition(lines, 2019);
            Console.WriteLine(pos);
        }

        [Test]
        public void Test1parseinc7()
        {
            var deck = StandardDeck(10);
            string[] lines =
            {
                "deal with increment 7",
            };
            nrOfCardsStdSpcDeck = 10;
            for (int i = 0; i < 10; i++)
            {
                var r = TrackPosition(lines, i);
                Console.WriteLine($"{i} => {r} ");
            }
        }

        [Test]
        public void Test1parseinc3()
        {
            var deck = StandardDeck(10);
            string[] lines =
            {
                "deal with increment 3",
            };
            nrOfCardsStdSpcDeck = 10;
            for (int i = 0; i < 10; i++)
            {
                var r = TrackPosition(lines, i);
                Console.WriteLine($"{i} => {r} ");
            }
        }

        [Test]
        public void Test1parsedealIntoNewDeck()
        {
            var deck = StandardDeck(10);
            string[] lines =
            {
                "deal into new stack",
            };
            nrOfCardsStdSpcDeck = 10;
            for (int i = 0; i < 10; i++)
            {
                var r = TrackPosition(lines, i);
                Console.WriteLine($"{i} => {r} ");
            }
        }

        [Test]
        public void Test1parseCut3()
        {
            var deck = StandardDeck(10);
            string[] lines =
            {
                "cut 3",
            };
            nrOfCardsStdSpcDeck = 10;
            for (int i = 0; i < 10; i++)
            {
                var r = TrackPosition(lines, i);
                Console.WriteLine($"{i} => {r} ");
            }
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
            nrOfCardsStdSpcDeck = 10;
            for (int i = 0; i < 10; i++)
            {
                var r = TrackPosition(lines, i);
                Console.WriteLine($"{i} => {r} ");
            }
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
            nrOfCardsStdSpcDeck = 10;
            for (int i = 0; i < 10; i++)
            {
                var r = TrackPosition(lines, i);
                Console.WriteLine($"{i} => {r} ");
            }
        }

        [Test]
        public void Test3parse()
        {
            string[] lines =
            {
                "deal with increment 7",
                "deal with increment 9",
                "cut -2",
            };
            nrOfCardsStdSpcDeck = 10;
            for (int i = 0; i < 10; i++)
            {
                var r = TrackPosition(lines, i);
                Console.WriteLine($"{i} => {r} ");
            }
        }


        private long TrackPosition(string[] lines, long position)
        {
            var instructions = new List<(string prefix, Func<long, int, long> function)>
            {
                ("deal with increment ", IncNRev),
                ("cut ", CutNRev),
                ("deal into new stack", NewStackRev),

            };
            var oldposition = position;
            for (int r = 0; r < 1; r++)
            {
                foreach (var line in lines.Reverse())
                {
                    foreach (var instr in instructions)
                    {
                        if (line.StartsWith(instr.prefix))
                        {
                            if (!int.TryParse(line.Substring(instr.prefix.Length), out var nr))
                            {
                                nr = 0;
                            }
                            position = instr.function(position, nr);
                        }
                    }
                }
                oldposition = position;
            }

            return position;
        }

         long nrOfCardsStdSpcDeck = 10007;

        public long NewStackRev(long orig, int dummy=0)
        {
            return nrOfCardsStdSpcDeck - orig - 1;
        }

        public long CutNRev(long orig, int n)
        {
            return (orig + n + nrOfCardsStdSpcDeck) % nrOfCardsStdSpcDeck;
        }

        //public Dictionary<int, int[]> revs = new Dictionary<int, int[]>();

        //private GetRev(long orig, int n)
        //{
        //    if (!revs.ContainsKey(n))
        //    {
        //        var arr = new int[n];
        //        for (int i = 0; i < arr.Length; i++)
        //        {
        //            arr[(i * n) % arr.Length] = i;
        //        }
        //        revs[n] = arr;
        //    }
        //}

        public long IncNRev(long orig, int n)
        {
            var b = (orig / n);
            var c = (orig % n);
            var d = n - c;
            d = ((nrOfCardsStdSpcDeck%n) - c);

            var e = nrOfCardsStdSpcDeck / n;
            var f = nrOfCardsStdSpcDeck % n;

            return ((e * d + b) + (e * f + c) / n) %  nrOfCardsStdSpcDeck;
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

        public int[] IncNRev2(int[] orig, int n)
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
        public void Test1NewStack()
        {
            var d1 = NewStackRev(3);
            var check = nrOfCardsStdSpcDeck - d1 - 1;
            Assert.AreEqual(3, check);
        }

        [Test]
        public void Test1Cut3()
        {
            var d1 = CutNRev(2020, 3);
            var check = (d1 - 3 + nrOfCardsStdSpcDeck) % nrOfCardsStdSpcDeck;
            Assert.AreEqual(2020, check);
        }

        [Test]
        public void Test2_IncRev0()
        {
            nrOfCardsStdSpcDeck = 10;
            Assert.AreEqual(0, IncNRev(0, 3));
        }

        [Test]
        public void Test2_IncRev1()
        {
            nrOfCardsStdSpcDeck = 10;
            Assert.AreEqual(7, IncNRev(1, 3));
        }

        [Test]
        public void Test2_IncRev2()
        {
            nrOfCardsStdSpcDeck = 10;
            Assert.AreEqual(4, IncNRev(2, 3));
        }

        [Test]
        public void Test2_IncRev3()
        {
            nrOfCardsStdSpcDeck = 10;
            Assert.AreEqual(1, IncNRev(3, 3));
        }

        [Test]
        public void Test2_IncRev4()
        {
            nrOfCardsStdSpcDeck = 10;
            Assert.AreEqual(8, IncNRev(4, 3));
        }

        [Test]
        public void Test2_IncRev5()
        {
            nrOfCardsStdSpcDeck = 10;
            Assert.AreEqual(5, IncNRev(5, 3));
        }

        [Test]
        public void Test2_IncRev6()
        {
            nrOfCardsStdSpcDeck = 10;
            Assert.AreEqual(2, IncNRev(6, 3));
        }

        [Test]
        public void Test2_IncRev7()
        {
            nrOfCardsStdSpcDeck = 10;
            Assert.AreEqual(9, IncNRev(7, 3));
        }

        [Test]
        public void Test2_IncRev8()
        {
            nrOfCardsStdSpcDeck = 10;
            Assert.AreEqual(6, IncNRev(8, 3));
        }

        [Test]
        public void Test2_IncRev9()
        {
            nrOfCardsStdSpcDeck = 10;
            Assert.AreEqual(3, IncNRev(9, 3));
        }

        [Test]
        public void Test1_0Inc3()
        {
            var d1 = IncNRev(0, 3);
            Assert.AreEqual(0, d1);
        }

        [Test]
        public void Test1_1Inc3()
        {
            var d1 = IncNRev(1, 3);
            Assert.AreEqual(7, d1);
        }

        [Test]
        public void Test1_2Inc3()
        {
            var d1 = IncNRev(2, 3);
            Assert.AreEqual(4, d1);
        }

        [Test]
        public void Test1()
        {
            var deck = StandardDeck(10);
            var d1 = IncN(deck, 7);
        }

        [Test]
        public void Test2()
        {
            var deck = StandardDeck(10);
        }


    }
}
