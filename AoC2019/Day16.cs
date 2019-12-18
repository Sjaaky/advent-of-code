using LanguageExt;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static AoC2019Test.Util;

namespace AoC2019Test
{
    public class Day16
    {
        [Test]
        public void Part1()
        {
            var input = File.ReadAllText("day16.input");
            List<int> arr = new List<int>();
            List<int> arr2 = new List<int>();
            foreach (char c in input)
            {
                if (c < '0' || c > '9') break;
                arr.Add((c - '0'));
            }

            var res = FFT(100, arr, arr2);
            Assert.AreEqual(new int[] { 7, 7, 0, 3, 8, 8, 3, 0 }, res);
        }

        [Test]
        public void Part2()
        {
            var input = File.ReadAllText("day16.input");

            var res = string.Join("", FFTpart2(input));
            Assert.AreEqual("28135104", res);
        }

        [Test]
        public void Part2t331()
        {
            var input = "03036732577212944063491565474664";

            var res = string.Join("", FFTpart2(input));
            Assert.AreEqual("84462026", res);
        }

        [Test]
        public void Part2t332()
        {
            var input = "02935109699940807407585447034323";

            var res = string.Join("", FFTpart2(input));
            Assert.AreEqual("78725270", res);
        }

        [Test]
        public void Part2t333()
        {
            var input = "03081770884921959731165446850517";

            var res = string.Join("", FFTpart2(input));
            Assert.AreEqual("53553731", res);
        }


        private static int[] FFTpart2(string input)
        {
            int skip = 1;
            List<int> arr = new List<int>();
            int toskip = int.Parse(input.Substring(0, 7));

            for (int i = 0; i < 10000; i++)
            {
                foreach (char c in input)
                {
                    if (c < '0' || c > '9') continue;
                    if (skip > toskip)
                    {
                        arr.Add((c - '0'));
                    }
                    skip++;
                }
            }

            for (int i = 0; i < 100; i++)
            {
                for (int j = arr.Count - 2; j >= 0; j--)
                {
                    arr[j] = (arr[j + 1] + arr[j]) % 10;
                }
            }
            return arr.Take(8).ToArray();
        }
    
        [Test]
        public void Part1t1()
        {
            var input = "80871224585914546619083218645595";
            List<int> arr = new List<int>();
            List<int> arr2 = new List<int>();
            foreach (char c in input)
            {
                arr.Add((c - '0'));
            }

            var res = FFT(100, arr, arr2);
            Assert.AreEqual(new int[] { 2,4,1,7,6,1,7,6 }, res);
        }

        [Test]
        public void Part1t2()
        {
            var input = "19617804207202209144916044189917";
            List<int> arr = new List<int>();
            List<int> arr2 = new List<int>();
            foreach (char c in input)
            {
                arr.Add((c - '0'));
            }

            var res = FFT(100, arr, arr2);
            Assert.AreEqual(new int[] { 7,3,7,4,5,4,1,8 }, res);
        }

        [Test]
        public void Part1t3()
        {
            var input = "69317163492948606335995924319873";
            List<int> arr = new List<int>();
            List<int> arr2 = new List<int>();
            foreach (char c in input)
            {
                arr.Add((c - '0'));
            }

            var res = FFT(100, arr, arr2);
            Assert.AreEqual(new int[] { 5,2,4,3,2,1,3,3 }, res);
        }

        private int[] FFT(int cnt, List<int> arr, List<int> arr2)
        {
            for (int run = 0; run < cnt; run++)
            {
                arr2 = new List<int>(arr.Count);

                for (int p = 0; p < arr.Count(); p++)
                {
                    long total = 0;
                    int q = -1;
                    foreach (var phase in phase(p))
                    {
                        q++;
                        if (q >= arr.Count) break;
                        if (phase == 0) continue;
                        total += (arr[q] * phase) % 10;
                    }
                    arr2.Add((int)(Math.Abs(total)) % 10);
                }
                arr = arr2;

            }

            Console.WriteLine(string.Join("", arr2.Take(8)));
            return arr2.Take(8).ToArray();
        }
        //77038830 <= correct answer

        [Test]
        public void ShowPhase()
        {
            int count = 30;
            int[] total = new int[650*10000];
            int[] t1 = new int[30];
            int[] tm1 = new int[30];
            for (int p = 0; p < count; p++)
            {
                int q = 0;
                int ofs = 5976257;

                foreach (var phase in phase(p+ofs).Skip(p+ofs).Take(523743))
                {
                    // Console.WriteLine($"---{p} {q} {phase}----");
                    if (phase != 1)
                    {
                        throw new Exception();
                        var x = 2;
                    }
                    //total = total % 10;
                    total[q] += phase;
                    //if (phase == 1)
                    //t1[q] ++;
                    //if (phase == -1)
                    //tm1[q] ++;
                    Console.Write($"{phase},");
                    q++;
                }
                Console.WriteLine();
            }

            Console.WriteLine($"total {string.Join(",", total)}");
            //Console.WriteLine($"total {string.Join(",", t1)}");
            //Console.WriteLine($"total {string.Join(",", tm1)}");
        }

        private IEnumerable<int> phase(int t)
        {
            int s = 1;
            //if (t == 0) s = 1;
            t++;
            while(true)
            {
                for(int i = s; i < t; i++)
                {
                    yield return 0;
                }
                s = 0;
                for (int i = 0; i < t; i++)
                {
                    yield return 1;
                }
                for (int i = 0; i < t; i++)
                {
                    yield return 0;
                }
                for (int i = 0; i < t; i++)
                {
                    yield return -1;
                }
            }
        }

   

    }
}
