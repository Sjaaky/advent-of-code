using LanguageExt;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static AoC2019Test.Util;

namespace AoC2019Test
{
    public class Day11
    {
        [Test]
        public void Part1()
        {
            var program = File.ReadAllLines("day11.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();

            var c = new IntCodeComputer(program, true);
            var area = new Dictionary<(int x, int y), int> { [(0, 0)] = 0 };
            RunHullPaintingRobot(c, area);
            Console.WriteLine(area.Count);
            Assert.AreEqual(2373, area.Count);
        }

        [Test]
        public void Part2()
        {
            var program = File.ReadAllLines("day11.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();

            var c = new IntCodeComputer(program, true);
            var area = new Dictionary<(int x, int y), int> { [(0, 0)] = 1 };
            RunHullPaintingRobot(c, area);
            DrawHull(area);
        }

        private (int x, int y) RunHullPaintingRobot(IntCodeComputer c, Dictionary<(int x, int y), int> area)
        {
            var input = new List<bigint>();
            var location = (x: 0, y: 0);
            var directionIdx = 0;
            var directions = new[] { (x: 0, y: -1), (x: 1, y: 0), (x: 0, y: 1), (x: -1, y: 0) };

            while (!c.IsHalted)
            {
                input.Add(area.GetValueOrDefault(location));
                c.Execute(input);
                var color = c.Output.Last();
                c.Execute(input);
                var turn = c.Output.Last();

                area[location] = (int)color;
                directionIdx = (directionIdx + (turn == 0 ? 3 : 5)) % 4;
                location.x += directions[directionIdx].x;
                location.y += directions[directionIdx].y;
            }

            return location;
        }

        private void DrawHull(Dictionary<(int x, int y), int> area)
        {
            var minx = area.Min(c => c.Key.x);
            var miny = area.Min(c => c.Key.y);

            var width = area.Max(c => c.Key.x) - minx + 1;
            var height = area.Max(c => c.Key.y) - miny + 1;
            int[,] hull = new int[width, height];
            foreach (var p in area)
            {
                hull[p.Key.x - minx, p.Key.y - miny] = p.Value;
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var p = hull[x, y];
                    Console.Write(p == 1 ? "█" : "░");
                }
                Console.WriteLine();
            }
        }
    }
}
