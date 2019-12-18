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
    public class Day17
    {
        [Test]
        public void Part1()
        {
            var program = File.ReadAllLines("day17.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();

            var c = new IntCodeComputer(program, false);
            var area = new Dictionary<(int x, int y), int> { [(0, 0)] = 0 };
            var input = new List<bigint>();

            c.Execute();
            var x = 0;
            var y = 0;
            var maxx = 0;
            var maxy = 0;
            (int, int) robot = (0,0);
            foreach (var o in c.Output)
            {
                area[(x, y)] = (int)o;
                maxx = Math.Max(maxx, x);
                maxy = Math.Max(maxy, y);
                if (o < 120 && "<>^v".Contains((char)o))
                {
                    robot = (x, y);
                }
                x++;
                if (o == '\n')
                {
                    x = 0;
                    y++;
                }
            }

            int checksum = 0;
            for(int xi = 0; xi < maxx; xi++)
            {
                for (int yi = 0; yi< maxy; yi++)
                {
                    if (area[(xi, yi)] == '#' && 
                        Util.Range(1,4).All(d => area.GetValueOrDefault(updatePosition((xi, yi), d), 0) == '#'))
                    {
                        checksum += xi * yi;
                    }
                }
            }

            //WalkShip(area, robot);
            //Console.WriteLine("walk back");
            //WalkShip(area, (10,40));


            DrawHull(area, (0, 0));
            Console.WriteLine(checksum);
        }

        private void WalkShip(Dictionary<(int x, int y), int> area, (int, int) robot)
        {
            var pos = robot;
            var olddir = 1;
            var dir = 1;
            foreach (var d in Util.Range(1, 600))
            {
                dir = (d % 4) + 1;
                if (dir == reverse(olddir)) continue;
                int steps = 0;
                do
                {
                    var newpos = updatePosition(pos, dir);
                    if (area.GetValueOrDefault(newpos) != 0 && area[newpos] == '#')
                    {
                        pos = newpos;
                        if (steps == 0)
                        {
                            Console.Write($"{Direction(olddir, dir)},");
                            olddir = dir;
                        }
                        steps++;
                    }
                    else
                    {
                        if (steps > 0)
                            Console.Write($"{steps},");
                        break;
                    }
                } while (area[pos] == '#');
            }
            Console.WriteLine(pos);
        }

        private char Direction(int old, int nw)
        {
            int[] dirs = new int[] { 1, 4, 2, 3 };
            var o = Array.IndexOf(dirs, old);
            var n = Array.IndexOf(dirs, nw);
            if (((o + 1) % 4) == n) return 'R';
            if (((n + 1) % 4) == o) return 'L';
            throw new Exception($"wrong direction {old} {nw}");
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
            var program = File.ReadAllLines("day17.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();

            program[0] = 2;

            var c = new IntCodeComputer(program, false);
            var area = new Dictionary<(int x, int y), int> { [(0, 0)] = 1 };

            string input =
                "A,C,A,C,B,B,C,A,C,B\n" +
                "L,12,L,10,R,8,L,12\n" +
                "L,10,R,12,R,8\n" +
                "R,8,R,10,R,12\n" +
                "n\n"
                ;
            c.Execute(input.Select(c => (bigint)c).ToList());

            var prev = (0, 0);
            var dead = false;
            var stil = false;
            var done = false;
            while (!done && !dead && !stil)
            {
                var x = 0;
                var y = 0;
                var maxx = 0;
                var maxy = 0;
                var onStatus = false;
                foreach (var o in c.Output)
                {
                    if (o == '\\') { onStatus = true; }
                    area[(x, y)] = (int)o;
                    maxx = Math.Max(maxx, x);
                    maxy = Math.Max(maxy, y);
                    x++;
                    if (o == '\n')
                    {
                        //if (onStatus) break;
                        x = 0;
                        y++;
                    }
                    if (o > 120)
                    {
                        Console.WriteLine(o);
                        Console.WriteLine(""+o);
                        Console.WriteLine((long)o);
                        done = true;
                        break;
                    }
                    if (o < 120 && "<>^v".Contains((char)o))
                    {
                        if (prev == (x,y))
                        {
                            stil = true;
                        }
                        prev = (x, y);
                    }
                    if (o == 'X')
                    {
                        dead = true;
                    }
                }
                if (dead)
                {
                    break;
                }
                //Console.SetCursorPosition(1, 1);
                //DrawHull(area, (0, 0));
            }

        }



        private void DrawHull(Dictionary<(int x, int y), int> area, (int x, int y)? robot)
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
                    if (hull[x, y] == '.')
                    {
                        Console.Write('-');
                    }
                    else
                    {
                        var p = hull[x, y];
                        Console.Write((char)p);
                    }
                }
           //     Console.WriteLine();
            }
        }
    }
}
