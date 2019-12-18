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
    public class Day15
    {
        [Test]
        public void Part1()
        {
            var program = File.ReadAllLines("day15.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();


            var c = new IntCodeComputer(program, true);
            var area = new Dictionary<(int x, int y), int> { [(0, 0)] = 0 };
            var input = new List<bigint>();
            var steps = new List<int>();
            var stepIdx = 0;
            var newStepIdx = 0;
            var x = 0;
            var y = 0;
            var cnt = 0;
            var fwd = 1;
            (int, int)? dest = null;
            area[(0, 0)] = '.';
            while (true)
            {
                if (steps.Count <= stepIdx) steps.Add(3);
                input.Add(steps[stepIdx]);
                c.Execute(input);
                var output = (int)c.Output.Last();
                switch (output)
                {
                    case 0:
                        area[updatePosition((x, y), steps[stepIdx])] = '#';
                        break;
                    case 1:
                        (x, y) = updatePosition((x, y), steps[stepIdx]);
                        newStepIdx = stepIdx + fwd;
                        area[(x, y)] = '.';
                        break;
                    case 2:
                        (x, y) = updatePosition((x, y), steps[stepIdx]);
                        newStepIdx = stepIdx + fwd;
                        area[(x, y)] = '!';
                        dest = (x, y);
                        break;

                }
                bool f = false;
                if (steps.Count <= newStepIdx) steps.Add(3);

                for (int i = 1; i < 5; i++)
                {
                    var checkpos = updatePosition((x, y), i);
                    if (!area.ContainsKey(checkpos))
                    {
                        steps[newStepIdx] = i;
                        f = true;
                        break;
                    }
                }
                if (f)
                {
                    fwd = 1;
                }
                else
                {
                    fwd = -1;
                    if (newStepIdx == 0) break;
                    steps[newStepIdx] = reverse(steps[newStepIdx - 1]);
                }

                cnt++;
                stepIdx = newStepIdx;
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 0);
                var robot = (x, y);
                DrawHull(area, robot);
            }

            Assert.IsNotNull(dest);
            var s = FindFastest(area, new List<(int, int)>{ (0, 0)}, 0, dest.Value);
            var o = Fill(area, new List<(int, int)>{ dest.Value }, 0);
            Console.WriteLine(s);
            Console.WriteLine(o);
        }

        private int FindFastest(Dictionary<(int, int), int> map, List<(int, int)> current, int steps, (int, int) dest)
        {
            var nextSteps = new List<(int, int)>();
            foreach (var p in current)
            {
                map[p] = '@';
                for (int i = 1; i < 5; i++)
                {
                    var newp = updatePosition(p, i);
                    if (dest == newp)
                    {
                        return steps+1;
                    }
                    if (map[newp] != '#' && map[newp] != '@')
                    {
                        nextSteps.Add(newp);
                    }
                } 
            }
            Console.SetCursorPosition(0, 4);
            DrawHull(map, (0,0));

            return FindFastest(map, nextSteps, steps + 1, dest);
        }

        private int Fill(Dictionary<(int, int), int> map, List<(int, int)> current, int steps)
        {
            var nextSteps = new List<(int, int)>();
            foreach (var p in current)
            {
                map[p] = 'O';
                for (int i = 1; i < 5; i++)
                {
                    var newp = updatePosition(p, i);
                    if (map[newp] != '#' && map[newp] != 'O')
                    {
                        nextSteps.Add(newp);
                    }
                }
            }
            Console.SetCursorPosition(0, 4);
            DrawHull(map, (0, 0));
            if (!nextSteps.Any()) 
                return steps;
            else
                return Fill(map, nextSteps, steps + 1);
        }

        private (int x, int y) updatePosition((int x, int y) pos, int dir)
        {
            switch(dir)
            {
                case 1: return (pos.x, pos.y - 1);
                case 2: return (pos.x, pos.y + 1);
                case 3: return (pos.x - 1, pos.y);
                case 4: return (pos.x + 1, pos.y);
            }
            throw new Exception($"no valid direction {dir}");
        }

        private int reverse(int dir)
        {
            switch (dir)
            {
                case 1: return 2;
                case 2: return 1;
                case 3: return 4;
                case 4: return 3;
            }
            throw new Exception($"no valid direction {dir}");
        }

        [Test]
        public void Part2()
        {
            var program = File.ReadAllLines("day15.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();


            var c = new IntCodeComputer(program, true);
            var area = new Dictionary<(int x, int y), int> { [(0, 0)] = 1 };
            RunHullPaintingRobot(c, area);
         //   DrawHull(area);
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

        private void DrawHull(Dictionary<(int x, int y), int> area, (int x, int y) robot)
        {
            //return;
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
                    if (robot.x - minx == x && robot.y - miny == y)
                    {
                        if (hull[x, y] == '!')
                        {
                            Console.Write('*');
                        }
                        else
                        {
                            Console.Write('R');
                        }
                    }
                    else
                    if (x == 0 && y == 0)
                    {
                        Console.Write('o');
                    }
                    else
                    {
                        var p = hull[x, y];
                        Console.Write((char)p);
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
