using LanguageExt;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using static AoC2019Test.Util;

namespace AoC2019Test
{
    public partial class Day13
    {
        [Test]
        public void Part1()
        {
            var program = File.ReadAllLines("day13.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();

            var screen = new Dictionary<(int x, int y), int>();
            var c = new IntCodeComputer(program, true);
            
            RunArcade(c, screen);
            Console.WriteLine(screen.Count(t => t.Value == 2));
        }

        [Test]
        public void Part2()
        {
            var program = File.ReadAllLines("day13.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();

            var screen = new Dictionary<(int x, int y), int>();
            var c = new IntCodeComputer(program, true);
            c.Memory[0] = 2;

            Assert.AreEqual(12856, RunArcade(c, screen));
        }

        private int RunArcade(IntCodeComputer c, Dictionary<(int x, int y), int> screen)
        {
            var input = new List<bigint>(){  };

            var paddlex = 0;
            var score = 0;
            while (!c.IsHalted)
            {
                c.Execute(input);
                var x = (int)c.Output.Last();
                c.Execute(input);
                var y = (int)c.Output.Last();
                c.Execute(input);
                var t = c.Output.Last();

                if (t == 4) { 
                    var ballx = x;
                    if (ballx == paddlex) input.Add(0);
                    if (ballx < paddlex) input.Add(-1);
                    if (ballx > paddlex) input.Add(1);
                    screen[(x, y)] = (int)t;
                }
                if (t == 3) {
                    paddlex = x; 
                }
                if (x == -1 && y == 0)
                {
                    score = (int)t;
                }
                screen[(x,y)] = (int)t;
            }
            return score;
        }

        private void DrawScreen(Dictionary<(int x, int y), int> screen, int score)
        {
            try
            {
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 0);
            }
            catch (IOException) { }
            Console.WriteLine($"score: {score}");

            var minx = screen.Min(c => c.Key.x);
            var miny = screen.Min(c => c.Key.y);

            var width = screen.Max(c => c.Key.x) - minx + 1;
            var height = screen.Max(c => c.Key.y) - miny + 1;
            int[,] hull = new int[width, height];

            foreach (var p in screen)
            {
                hull[p.Key.x - minx, p.Key.y - miny] = p.Value;
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var p = hull[x, y];
                    char c = ' ';
                    switch (p)
                    {
                        case 1: c = '█'; break;
                        case 2: c = 'X'; break;
                        case 3: c = '_'; break;
                        case 4: c = 'o'; break;
                    }
                    Console.Write(c);
                }
                Console.WriteLine();
            }
        }
    }
}
