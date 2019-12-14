using AoC2019Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoCCmd
{
    class Program
    {
        static void Main2(string[] args)
        {
            var d = new Day13();
            d.Part2();
        }
        static void Main(string[] args)

        {
            var d = new Day12();

            //List<Day12.Body> bodies;
            //HashSet<string> occ;
            //long s;
            //long px = LoopWithOverride(0, d, out bodies, out occ, out s);
            //long py = LoopWithOverride(1, d, out bodies, out occ, out s);
            //long pz = LoopWithOverride(2, d, out bodies, out occ, out s);


            //long cxy = px * py / MaxCommonDiv(px, py);
            //long c = cxy * pz / MaxCommonDiv(cxy, pz);

            ////var s = d.LoopWithPrint(occ, bodies);

            //Console.WriteLine($"> {px} {py} {pz} => {cxy} => {c}");
            //Console.WriteLine($"> {px*py*pz} => {cxy} => {c}");

            //d.LoopWithPrint("day12.testA.input");

            //d.LoopWithPrint("day12.testB.input");
            //d.LoopWithPrint("day12.testC.input");

            //var r = new List<(int, int)>();
            //for (int i = 0; i < 100; i++)
            //{
            //    Console.WriteLine($"=== {i} ===");
            //    HashSet<string> occ = new HashSet<string>();
            //    var bodies = new List<Day12.Body> { new Day12.Body { x = 0, y = 0, z = 0 }, new Day12.Body { x = i, y = 0, z = 0 }
            //    , new Day12.Body { x = i + 2, y = 0, z = 0 } 
            //    };
            //    long s = d.LoopWithPrint(occ, bodies);
            //    r.Add((i, (int)s));
            //}

            //foreach (var res in r)
            //{
            //    Console.WriteLine($"{res.Item1} {res.Item2}");
            //}

        }

        private static long MaxCommonDiv(long a, long b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            long max = Math.Max(a, b);
            long min = Math.Min(a, b);
            for (long i = min; i >= 2; i--)
            {
                if (min % i == 0 && max % i == 0)
                {
                    return i;
                }
            }
            return 1;
        }


        //private static long LoopWithOverride(int o, Day12 d, out List<Day12.Body> bodies, out HashSet<string> occ, out long s)
        //{
        //    bodies = File.ReadAllLines("day12.input").Select(d.ReadBody).ToList();
        //    occ = new HashSet<string>();
        //    foreach (var b in bodies)
        //    {
        //        if (o != 0) b.x = 0;
        //        if (o != 1) b.y = 0;
        //        if (o != 2) b.z = 0;
        //    }

        //    //s = d.LoopWithPrint(occ, bodies);
        //    //Console.WriteLine($"> {s}");
        //    //return s;
        //}
    }
}
