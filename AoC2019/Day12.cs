using LanguageExt;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static AoC2019Test.Util;

namespace AoC2019Test
{
    public class Day12
    {
        private Regex body = new Regex(@"<(?<c1>\w+)=(?<v1>-?\d+), (?<c2>\w+)=(?<v2>-?\d+), (?<c3>\w+)=(?<v3>-?\d+)>");

        public class Body
        {
            public int x;
            public int y;
            public int z;

            public int dx;
            public int dy;
            public int dz;

            public override string ToString()
            {
                return $"x={x} y={y} z={z} (dx={dx} dy={dy} dz={dz})";
            }
        }

        [Test]
        public void Part1()
        {
            var bodies = File.ReadAllLines("day12.input").Select(ReadBody).ToList();

            var stopAt = 1000;
            for (int s = 0; s < stopAt; s++)
            {
                for (int a = 0; a < bodies.Count; a++)
                {
                    for (int b = 0; b < bodies.Count; b++)
                    {
                        if (a == b) continue;
                        UpdateVelocity(bodies[a], bodies[b]);
                    }
                }
                bodies.ForEach(UpdatePosition);
            }
            Assert.AreEqual(11384, Energy(bodies));
        }

        [Test]
        public void Part2()
        {
            var bodies = File.ReadAllLines("day12.input").Select(ReadBody).ToList();

            long x = Loop(bodies, 1);
            long y = Loop(bodies, 2);
            long z = Loop(bodies, 4);

            var xyz = LeastCommonMultiple(LeastCommonMultiple(x, y), z);

            Console.WriteLine(xyz);
            Assert.AreEqual(452582583272768, xyz);
        }

        private long Loop(List<Body> bodies, int mask)
        {
            var occ = new System.Collections.Generic.HashSet<string>();

            long s = 0;
            while (true)
            {
                var hash = Hash(bodies);
                if (occ.Contains(hash)) break;
                occ.Add(hash);

                for (int a = 0; a < bodies.Count; a++)
                {
                    for (int b = 0; b < bodies.Count; b++)
                    {
                        if (a == b) continue;
                        UpdateVelocity(bodies[a], bodies[b], mask);
                    }
                }
                bodies.ForEach(UpdatePosition);
                s++;
            }
            return s;
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

        private static long LeastCommonMultiple(long a, long b)
        {
            return a * b / MaxCommonDiv(a, b);
        }

        //[Test]
        //public void Part2Test1()
        //{
        //    var occ = new System.Collections.Generic.HashSet<string>();
        //    var bodies = File.ReadAllLines("day12.test1.input").Select(ReadBody).ToList();
        //    //PrintAll(0, bodies);

        //    long s = LoopWithPrint(occ, bodies);

        //    PrintAll(s, bodies);
        //}

        //[Test]
        //public void Part2Test2()
        //{
        //    var filename = "day12.test1.input";
        //    LoopWithPrint(filename);
        //}

        //public void LoopWithPrint(string filename)
        //{
        //    var occ = new System.Collections.Generic.HashSet<string>();
        //    var bodies = File.ReadAllLines(filename).Select(ReadBody).ToList();

        //    long s = LoopWithPrint(occ, bodies);

        //    PrintAll(s, bodies);
        //}

        //public long LoopWithPrint(System.Collections.Generic.HashSet<string> occ, List<Body> bodies)
        //{
        //    //PrintSeq(bodies);
        //    long s;
        //    for (s = 0; s < 1000000000L; s++)
        //    {
        //        var hash = Hash(bodies);
        //        if (occ.Contains(hash))
        //        {
        //            Console.WriteLine();
        //            Console.WriteLine($"DONE {s}");
        //            break;
        //        }
        //        else
        //        {
        //            occ.Add(hash);
        //        }
        //        for (int a = 0; a < bodies.Count; a++)
        //        {
        //            for (int b = 0; b < bodies.Count; b++)
        //            {
        //                if (a == b) continue;
        //                UpdateVelocity(bodies[a], bodies[b]);
        //            }

        //        }
        //        bodies.ForEach(UpdatePosition);
        //        PrintSeq(bodies);
        //        //PrintMap(bodies);
        //       // Thread.Sleep(50);
        //    }
        //    return s;
        //}

        //private List<(int x, int y)> prevdraw = new List<(int x, int y)>();
        //private void PrintMap(List<Body> bodies)
        //{
        //    try
        //    {
        //        int i = 0;
        //        if (prevdraw != null)
        //        {
        //            foreach (var b in prevdraw)
        //            {
        //                i++;
        //                Console.SetCursorPosition(b.x, b.y);
        //                Console.WriteLine((char)(i + 'a'));
        //            }
        //            prevdraw.Clear();
        //        }
        //        i = 0;
        //        Console.SetCursorPosition(0, 0);
        //        int ox = 20;
        //        int oy = 20;
        //        foreach (var b in bodies)
        //        {
        //            i++;
        //            if (b.x + ox < 0 || b.x + ox >= Console.BufferWidth-2) continue;
        //            if (b.y + oy < 0 || b.y + oy >= Console.BufferHeight) continue;
        //            Console.SetCursorPosition(b.x + ox, b.y + oy);
        //            prevdraw.Add((b.x + ox, b.y + oy));
        //            Console.Write((char)(i + 'A'));
        //        }
        //    }
        //    catch (IOException)
        //    { }
        //}

        //private void PrintSeq(List<Body> bodies)
        //{
        //    int i = 0;
        //    i = 0;
        //    int ox = 20;
        //    int oy = 20;
        //    foreach (var b in bodies)
        //    {
        //        i++;
        //        if (b.x + ox < 0 || b.x + ox >= Console.BufferWidth - 2) continue;
        //        if (b.y + oy < 0 || b.y + oy >= Console.BufferHeight) continue;
        //        Console.SetCursorPosition(b.x + ox, Console.CursorTop);
        //        Console.Write((char)(i + 'A'));
        //    }
        //    Console.WriteLine();
        //}

        private string Hash(List<Body> bodies)
        {
            return string.Join(",", bodies);
        }

        //private void PrintAll(long step, List<Body> bodies)
        //{
        //    Console.WriteLine("--");
        //    foreach (var b in bodies)
        //    {
        //        Console.WriteLine(b);
        //    }
        //    Console.WriteLine($"timestep {step} {Energy(bodies)}");
        //}

        private int Energy(List<Body> bodies)
        {
            return bodies.Sum(Energy);
        }

        private int Energy(Body body)
        {
            var pot = Math.Abs(body.x) + Math.Abs(body.y) + Math.Abs(body.z);
            var kin = Math.Abs(body.dx) + Math.Abs(body.dy) + Math.Abs(body.dz);
            return pot * kin;
        }

        private void UpdateVelocity(Body a, Body b, int mask = 7)
        {
            if ((mask & 1) == 1)
            {
                if (a.x > b.x) a.dx--;
                if (a.x < b.x) a.dx++;
            }
            if ((mask & 2) == 2)
            {
                if (a.y > b.y) a.dy--;
                if (a.y < b.y) a.dy++;
            }
            if ((mask & 4) == 4)
            {
                if (a.z > b.z) a.dz--;
                if (a.z < b.z) a.dz++;
            }
        }

        private void UpdatePosition(Body a)
        {
            a.x += a.dx;
            a.y += a.dy;
            a.z += a.dz;
        }

        public Body ReadBody(string line)
        {
            var match = body.Match(line);
            Body b = new Body();
            for (int i = 1; i + 1 < match.Groups.Count; i += 2)
            {
                var v = int.Parse(match.Groups[i + 1].Value);
                switch (match.Groups[i].Value)
                {
                    case "x": b.x = v; break;
                    case "y": b.y = v; break;
                    case "z": b.z = v; break;
                }
            }
            return b;
        }

    }
}
