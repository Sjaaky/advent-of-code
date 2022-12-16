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
    public class Day22part2
    {
        [Test]
        public void Part2()
        {
            var lines = File.ReadAllLines("day22.input");

            var pos = TrackPosition(lines, 2020);

            Console.WriteLine(pos);
        }

        private long TrackPosition(string[] lines, long position)
        {
            var instructions = new List<(string prefix, Func<long, int, long, long> function)>
            {
                ("deal with increment ", IncNRev),
                ("cut ", CutNRev),
                ("deal into new stack", NewStackRev),

            };
            long nrOfCardsStdSpcDeck = 119315717514047;
            var oldposition = position;
            var repeat = new System.Collections.Generic.HashSet<long>();

            var functions = new List<Func<long, long, long>>();
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
                        position = instr.function(position, nr, nrOfCardsStdSpcDeck);
                        functions.Add((a, b) => instr.function(a, nr, b));
                    }
                }
            }

            for (long r = 0; r < 101741582076661; r++)
            {
                //foreach (var line in lines.Reverse())
                //{
                //    foreach (var instr in instructions)
                //    {
                //        if (line.StartsWith(instr.prefix))
                //        {
                //            if (!int.TryParse(line.Substring(instr.prefix.Length), out var nr))
                //            {
                //                nr = 0;
                //            }
                //            position = instr.function(position, nr, nrOfCardsStdSpcDeck);
                //        }
                //    }
                //}
                foreach (var f in functions)
                {
                    position = f(position, nrOfCardsStdSpcDeck);
                }

                var newValue = repeat.Add(position);
                if (!newValue)
                {
                    Console.WriteLine($"Repeat after {r} steps");
                    return -1;
                }
                if (position == 2020)
                {
                    Console.WriteLine($"{r} {position} delta = {position - oldposition} delta2 = {(position - oldposition + nrOfCardsStdSpcDeck) % nrOfCardsStdSpcDeck}");

                }
                oldposition = position;
            }

            return position;
        }

        //const long nrOfCardsStdSpcDeck = 119315717514047;

        public long NewStackRev(long orig, int dummy, long nrOfCards)
        {
            return nrOfCards - orig - 1;
        }

        public long CutNRev(long orig, int n, long nrOfCards)
        {
            return (orig + n + nrOfCards) % nrOfCards;
        }

        public long IncNRev(long position, int n, long nrOfCards)
        {
            long nrs = -1;
            long delta = nrOfCards % n;
            long offset = 0;
            long mod = 0;
            while (true)
            {
                mod = ((position + n - offset) % n);
                if (mod == 0) break;
                
                nrs += (nrOfCards / n) + 1;
                if (offset > delta)
                {
                    nrs--;
                }
                offset += n - delta;
                offset %= n;
            }

            return nrs + ((position + (n - offset)) / n);
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
            long nrOfCards = 10;
            var d1 = NewStackRev(3, 0, nrOfCards);
            var check = nrOfCards - d1 - 1;
            Assert.AreEqual(3, check);
        }

        [Test]
        public void Test1Cut3()
        {
            long nrOfCards = 10;
            var d1 = CutNRev(2020, 3, nrOfCards);
            var check = (d1 - 3 + nrOfCards) % nrOfCards;
            Assert.AreEqual(2020, check);
        }

        [Test]
        public void Test2_IncRev()
        {
            long nrOfCards = 10;
            Assert.AreEqual(0, IncNRev(0, 3, nrOfCards));
        }

        [Test]
        public void Test2_IncRev1()
        {
            long nrOfCards = 10;
            Assert.AreEqual(7, IncNRev(1, 3, nrOfCards));
        }

        [Test]
        public void Test2_IncRev2()
        {
            long nrOfCards = 10;
            Assert.AreEqual(4, IncNRev(2, 3, nrOfCards));
        }

        [Test]
        public void Test2_IncRev3()
        {
            long nrOfCards = 10;
            Assert.AreEqual(1, IncNRev(3, 3, nrOfCards));
        }

        [Test]
        public void Test2_IncRev4()
        {
            long nrOfCards = 10;
            Assert.AreEqual(8, IncNRev(4, 3, nrOfCards));
        }

        [Test]
        public void Test2_IncRev5()
        {
            long nrOfCards = 10;
            Assert.AreEqual(5, IncNRev(5, 3, nrOfCards));
        }

        [Test]
        public void Test2_IncRev6()
        {
            long nrOfCards = 10;
            Assert.AreEqual(2, IncNRev(6, 3, nrOfCards));
        }

        [Test]
        public void Test2_IncRev7()
        {
            long nrOfCards = 10;
            Assert.AreEqual(9, IncNRev(7, 3, nrOfCards));
        }

        [Test]
        public void Test2_IncRev8()
        {
            long nrOfCards = 10;
            Assert.AreEqual(6, IncNRev(8, 3, nrOfCards));
        }

        [Test]
        public void Test2_IncRev9()
        {
            long nrOfCards = 10;
            Assert.AreEqual(3, IncNRev(9, 3, nrOfCards));
        }

        [Test]
        public void Test3_IncRev0()
        {
            long nrOfCards = 11;
            Assert.AreEqual(0, IncNRev(0, 3, nrOfCards));
        }

        [Test]
        public void Test3_IncRev4()
        {
            long nrOfCards = 11;
            Assert.AreEqual(3, IncNRev(4, 3, nrOfCards));
        }

        [Test]
        public void Test3_IncRev5()
        {
            long nrOfCards = 11;
            Assert.AreEqual(5, IncNRev(5, 3, nrOfCards));
        }

        [Test]
        public void Test3_IncRev6()
        {
            long nrOfCards = 10;
            Assert.AreEqual(0, IncNRev(0, 3, nrOfCards));
            Assert.AreEqual(1, IncNRev(3, 3, nrOfCards));
            Assert.AreEqual(2, IncNRev(6, 3, nrOfCards));
            Assert.AreEqual(3, IncNRev(9, 3, nrOfCards));
            Assert.AreEqual(4, IncNRev(2, 3, nrOfCards));
            Assert.AreEqual(5, IncNRev(5, 3, nrOfCards));
            Assert.AreEqual(6, IncNRev(8, 3, nrOfCards));
            Assert.AreEqual(7, IncNRev(1, 3, nrOfCards));
            Assert.AreEqual(8, IncNRev(4, 3, nrOfCards));
            Assert.AreEqual(9, IncNRev(7, 3, nrOfCards));

            nrOfCards = 11;
            Assert.AreEqual(0, IncNRev(0, 3, nrOfCards));
            Assert.AreEqual(1, IncNRev(3, 3, nrOfCards));
            Assert.AreEqual(2, IncNRev(6, 3, nrOfCards));
            Assert.AreEqual(3, IncNRev(9, 3, nrOfCards));
            Assert.AreEqual(4, IncNRev(1, 3, nrOfCards));
            Assert.AreEqual(5, IncNRev(4, 3, nrOfCards));
            Assert.AreEqual(6, IncNRev(7, 3, nrOfCards));
            Assert.AreEqual(7, IncNRev(10, 3, nrOfCards));
            Assert.AreEqual(8, IncNRev(2, 3, nrOfCards));
            Assert.AreEqual(9, IncNRev(5, 3, nrOfCards));

        }

        [Test]
        public void Test7_IncRev6()
        {
            long nrOfCards = 10;
            Assert.AreEqual(2, IncNRev(4, 7, nrOfCards));
            Assert.AreEqual(0, IncNRev(0, 7, nrOfCards));
            Assert.AreEqual(1, IncNRev(7, 7, nrOfCards));
            Assert.AreEqual(3, IncNRev(1, 7, nrOfCards));
            Assert.AreEqual(4, IncNRev(8, 7, nrOfCards));
            Assert.AreEqual(5, IncNRev(5, 7, nrOfCards));
            Assert.AreEqual(6, IncNRev(2, 7, nrOfCards));
            Assert.AreEqual(7, IncNRev(9, 7, nrOfCards));
            Assert.AreEqual(8, IncNRev(6, 7, nrOfCards));
            Assert.AreEqual(9, IncNRev(3, 7, nrOfCards));

            nrOfCards = 11;
            Assert.AreEqual(0, IncNRev(0, 3, nrOfCards));
            Assert.AreEqual(1, IncNRev(3, 3, nrOfCards));
            Assert.AreEqual(2, IncNRev(6, 3, nrOfCards));
            Assert.AreEqual(3, IncNRev(9, 3, nrOfCards));
            Assert.AreEqual(4, IncNRev(1, 3, nrOfCards));
            Assert.AreEqual(5, IncNRev(4, 3, nrOfCards));
            Assert.AreEqual(6, IncNRev(7, 3, nrOfCards));
            Assert.AreEqual(7, IncNRev(10, 3, nrOfCards));
            Assert.AreEqual(8, IncNRev(2, 3, nrOfCards));
            Assert.AreEqual(9, IncNRev(5, 3, nrOfCards));

        }

        [Test]
        public void Test3_IncRev7()
        {
            long nrOfCards = 11;
            Assert.AreEqual(9, IncNRev(7, 3, nrOfCards));
        }


        [Test]
        public void Test1_0Inc3()
        {
            long nrOfCards = 10;
            var d1 = IncNRev(0, 3, nrOfCards);
            Assert.AreEqual(0, d1);
        }

        [Test]
        public void Test1_1Inc3()
        {
            long nrOfCards = 10;
            var d1 = IncNRev(1, 3, nrOfCards);
            Assert.AreEqual(7, d1);
        }

        [Test]
        public void Test1_2Inc3()
        {
            long nrOfCards = 10;
            var d1 = IncNRev(2, 3, nrOfCards);
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
