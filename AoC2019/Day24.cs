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
    public class Day24
    {
        [Test]
        public void Part1()
        {
            var bugsTxt = File.ReadAllText("day24.input")
                .Replace("\r", "").Replace("\n", "");

            var bugs = InputToBugs(bugsTxt);
            Console.WriteLine(BugsToString(bugs));

            var bio = new System.Collections.Generic.HashSet<int>();
            do
            {
                bugs = OneMinuteOfLifeBool(bugs);
                Console.WriteLine($"--{bugs}--");
                Console.WriteLine(BugsToString(bugs));

            }
            while (bio.Add(bugs));
            Console.WriteLine(bugs);
            Console.WriteLine(BugsToString(bugs));
        }

        [Test]
        public void Part1t1()
        {
            var bugsTxt = "....#" +
                "#..#." +
                "#..##" +
                "..#.." +
                "#...."
                .Replace("\r", "").Replace("\n", "");

            var bugs = InputToBugs(bugsTxt);
            Console.WriteLine(BugsToString(bugs));

            var bio = new System.Collections.Generic.HashSet<int>();
            do
            {
                bugs = OneMinuteOfLifeBool(bugs);
                //Console.WriteLine("result");
                //Console.WriteLine(BugsToString(bugs));

            }
            while (bio.Add(bugs));
            Console.WriteLine(bugs);
            Console.WriteLine(BugsToString(bugs));
        }


        private static int OneMinuteOfLifeBool(int input)
        {
            var n = (input << 5) &  0x1FFFFFF;
            var s = (input >> 5) &  0x1FFFFFF;
            var e = (input >> 1) & ~0x7F084210;
            var w = (input << 1) & ~0x7E108421;

            var infest = (~n & ~e & s) | (~n & ~w & e) | (~s & ~n & w) | (~s & ~e & n) | (~s & e & n & ~w) | (s & ~e & n & ~w);
            var stayalive = (s & ~e & ~n & ~w) | (~s & e & ~n & ~w) | (~s & ~e & ~n & w) | (~s & ~e & n & ~w);

            return (input & stayalive) | (infest & ~input);
        }

        private static int InputToBugs(string bugsTxt)
        {
            int bugs = 0;
            int mask = 1;
            foreach (var bug in bugsTxt)
            {
                if (bug == '#')
                {
                    bugs |= mask;
                }
                mask <<= 1;
            }
            return bugs;
        }

        private static string BugsToString(int bugs)
        {
            var sb = new StringBuilder();
            for(int i = 0; i < 25; i++)
            {
                if (i > 0 && i % 5 == 0) sb.AppendLine();
                sb.Append(((bugs >> i) & 1) == 1 ? '#' : '.');
            }
            return sb.ToString();
        }

        [Test]
        public void Part2()
        {
            var bugsTxt = File.ReadAllText("day24.input");

            var area = InputToArea(bugsTxt);
            int[,,] nb;
            DrawArea(area, 0);
            var bio = new System.Collections.Generic.HashSet<int>();

            for (int i = 0; i < 200; i++)
            {
                Console.WriteLine("==============");
                (area, nb) = OneMinuteOfLife(area);
                DrawArea(area, i, nb);
            }
            Console.WriteLine(NrOfBugs(area));
        }

        [Test]
        public void Part2t1()
        {
            var bugsTxt = "....#" +
                          "#..#." +
                          "#.?##" +
                          "..#.." +
                          "#....";

            var area = InputToArea(bugsTxt);
            int[,,] nb;
            DrawArea(area, 0);
            var bio = new System.Collections.Generic.HashSet<int>();

            for (int i = 0; i < 11; i++)
            {
                Console.WriteLine("==============");
                (area, nb) = OneMinuteOfLife(area, i);
                DrawArea(area, i, nb);
            }
        }

        [Test]
        public void Part2t2()
        {
            var bugsTxt = "#...#" +
                          "....." +
                          "..?.." +
                          "....." +
                          "#...#";

            var area = InputToArea(bugsTxt);
            int[,,] nb;
            DrawArea(area, 0);
            var bio = new System.Collections.Generic.HashSet<int>();

            for (int i = 0; i < 11; i++)
            {
                Console.WriteLine("==============");
                (area, nb) = OneMinuteOfLife(area);
                DrawArea(area, i, nb);
            }
        }


        private static int NrOfBugs(int[,,] area)
        {
            int bugs = 0;
            for(int level = 0; level < 201; level++)
            {
                bugs += NrOfBugs(area, level);
            }
            return bugs;
        }
        
        private static int NrOfBugs(int[,,] area, int level)
        {
            int bugs = 0;
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (x == 2 && y == 2) continue;
                    if (area[level, x, y] > 0)
                    {
                        bugs++;
                    }
                }
            }
            return bugs;
        }

        private static int[,,] InputToArea(string bugsTxt)
        {
            int[,,] area = new int[levels, 5, 5];
            int x = 0;
            int y = 0;
            foreach(var b in bugsTxt)
            {
                if ("#.?".Contains(b))
                {
                    area[100,x, y] = b == '#' ? 1 : 0;
                    x++;
                    if (x > 4)
                    {
                        y++;
                        x = 0;
                    }
                }
            }

            return area;
        }

        private static (int[,,], int[,,]) OneMinuteOfLife(int[,,] input, int minute = 0)
        {
            int[,,] area = new int[levels, 5, 5];
            int[,,] neighbours = new int[levels, 5, 5];
            for (int level = 0; level < 201; level++)
            {
                if (level == 100-4 && minute == 9)     { }
                for (int y = 0; y < 5; y++)
                {
                    for (int x = 0; x < 5; x++)
                    {
                        if ((x, y) == (2, 2))
                        {
                            area[level, x, y] = 0;
                            input[level, x, y] = 0;
                        }
                        else
                        {
                            area[level, x, y] = input[level, x, y];

                            var n = Neighbours(input, x, y, level);
                            neighbours[level, x, y] = n;
                            if (input[level, x, y] > 0)
                            {
                                if (n != 1) area[level, x, y] = 0;
                            }
                            else if (input[level, x, y] == 0)
                            {
                                if (n == 1 || n == 2) area[level, x, y] = 1;
                            }
                        }
                    }
                }
            }

            return (area, neighbours);
        }

        private static int Neighbours(int[,,] input, int x, int y, int level)
        {
            int neighbours = 0;
            if (x > 0 && (x, y) != (3, 2)) neighbours += input[level, x - 1, y];
            if (y > 0 && (x, y) != (2, 3)) neighbours += input[level, x, y - 1];
            if (x < 4 && (x, y) != (1, 2)) neighbours += input[level, x + 1, y];
            if (y < 4 && (x, y) != (2, 1)) neighbours += input[level, x, y + 1];

            if (level > 0)
            {
                if (x == 0) neighbours += input[level - 1, 1, 2];
                if (x == 4) neighbours += input[level - 1, 3, 2];
                if (y == 0) neighbours += input[level - 1, 2, 1];
                if (y == 4) neighbours += input[level - 1, 2, 3];
            }
            else if (level < 0)
            {
                throw new Exception($"level {level}");
            }
            if (level < levels - 1)
            {
                if ((x, y) == (2, 1))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        neighbours += input[level + 1, i, 0];
                    }
                }
                if ((x, y) == (1, 2))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        neighbours += input[level + 1, 0, i];
                    }
                }
                if ((x, y) == (2, 3))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        neighbours += input[level + 1, i, 4];
                    }
                }
                if ((x, y) == (3, 2))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        neighbours += input[level + 1, 4, i];
                    }
                }
            }
            else if (level > levels)
            {
                throw new Exception($"level {level}");
            }
            return neighbours;
        }

        static int levels = 201;
        static int levelDelta = -100;
        private static void DrawArea(int[,,] area, int minute, int[,,] neighbours = null)
        {
            int bugsFoundAtLevel = 100;
            Console.WriteLine($"=============Minute {minute}==============");
            for (int level = 0; level < levels; level++)
            {
                if (NrOfBugs(area, level) == 0) continue;
                if (bugsFoundAtLevel == 100 && NrOfBugs(area, level) > 0)
                {
                    bugsFoundAtLevel = Math.Abs(100 - level);
                }
                if (Math.Abs(100 - level) <= bugsFoundAtLevel)
                {
                    Console.WriteLine($"-- level  {level + levelDelta} --");
                    for (int y = 0; y < area.GetLength(2); y++)
                    {
                        for (int x = 0; x < area.GetLength(1); x++)
                        {
                            //if ((x, y) == (2, 2))
                            //{
                            //    Console.Write('?');
                            //}
                            //else
                            {
                                Console.Write(area[level, x, y] == 1 ? '#' : '.');
                            }
                        }
                        Console.WriteLine();
                    }
                    if (neighbours != null)
                    {
                        Console.WriteLine("neighbours");
                        for (int y = 0; y < area.GetLength(2); y++)
                        {
                            for (int x = 0; x < area.GetLength(1); x++)
                            {
                                //if ((x, y) == (2, 2))
                                //{
                                //    Console.Write('?');
                                //}
                                //else
                                {
                                    Console.Write(neighbours[level, x, y]);
                                }
                            }
                            Console.WriteLine();
                        }
                        Console.WriteLine("---");
                    }
                }
            }
        }
    }
}