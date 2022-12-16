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
    public class Day19
    {
        [Test]
        public void Part1()
        {
            var program = File.ReadAllLines("day19.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();


            var area = new Dictionary<(int x, int y), int> {  };
            var input = new List<bigint>();
            int cnt = 0;
            for (int y = 0; y < 50; y++)
            {
                for (int x = 0; x < 50; x++)
                {
                    var c = new IntCodeComputer(program, false);
                    c.Execute(new List<bigint>() { x, y });

                    area[(x, y)] = (int)c.Output.Last() == 1 ? '#':'.';
                    
                    if ((int)c.Output.Last() == 1)
                    {
                        cnt++;
                    }
                }
            }
            
            DrawHull(area, (0, 0));
            Console.WriteLine(cnt);
        }


        [Test]
        public void Part2()
        {
            var program = File.ReadAllLines("day19.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();


            var area = new Dictionary<(int x, int y), int> { };
            var input = new List<bigint>();
            int x = 1;
            int y = 1;
            (int x, int y) result = (0,0);
            while (true)
            {
                int now = 0;
                while (now == 0 && x > 0)
                {
                    x--;
                    now = GetTracktor(program, x, y);
                }
                if (GetTracktor(program, x-99, y) == 1 && GetTracktor(program, x - 99, y + 99) == 1)
                {
                    result = (x - 99, y);
                    break;
                }
                else
                {
                    y++;
                    x = y;
                }
            }

            Console.WriteLine(result.x*10000+result.y);
        }

        private static int GetTracktor(bigint[] program, int x, int y)
        {
            var c = new IntCodeComputer(program, false);
            c.Execute(new List<bigint>() { x, y });

            int now = (int)c.Output.Last();
            return now;
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
                Console.WriteLine();
            }
        }
    }
}
