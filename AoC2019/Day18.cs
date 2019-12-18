using LanguageExt;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static AoC2019Test.Util;

namespace AoC2019Test
{
    public class Day18
    {
        [Test]
        public void Part1()
        {
            var (area, pois) = Input("day18.input");
            DoIt(area, pois);
        }

        [Test]
        public void Part1t1()
        {
            var (area, pois) = Input("day18.test1.input");
            Assert.AreEqual(86, DoIt(area, pois));
        }

        [Test]
        public void Part1t2()
        {
            var (area, pois) = Input("day18.test2.input");
            Assert.AreEqual(132, DoIt(area, pois));
        }

        [Test]
        public void Part1t3()
        {
            var (area, pois) = Input("day18.test3.input");
            Assert.AreEqual(81, DoIt(area, pois));
        }

        [Test]
        public void Part1t4()
        {
            var (area, pois) = Input("day18.test4.input");
            Assert.AreEqual(136, DoIt(area, pois));
        }

        private int DoIt(Dictionary<(int x, int y), char> area, Dictionary<char, (int x, int y)> pois)
        {
            DrawHull(area);
            var edges = new Dictionary<char, List<Edge>>();

            foreach (var poi in pois.Where(p => char.IsLower(p.Key) || p.Key == '@'))
            {
                // Console.WriteLine($"== {poi.Key} ==");
                var edge1 = DistanceTo(area, (poi.Key, poi.Value.x, poi.Value.y));
                edges[poi.Key] = edge1.OrderBy(e => e.distance).ToList();
                ClearMap(area, pois);
            }

            var s = new State { dist = 0, currentLocation = ('@', pois['@'].x, pois['@'].y), keys = new List<char>() };
            var current = new SortedSet<State>();
            current.Add(s);
            return FindRoute(edges, current, pois.Count(p => char.IsLower(p.Key)));
        }

        private int FindRoute(Dictionary<char, List<Edge>> edges, SortedSet<State> current, int targetKeys)
        {
            while (true)
            {
                if (!current.Any())
                {
                    Console.WriteLine("!!EMPTY!!");
                    return -1;
                }
                var c = current.FirstOrDefault();
                current.Remove(c);
                if (c.keys.Count == targetKeys)
                {
                    Console.WriteLine($"DONE {c}");
                    throw new Exception($"DONE {c.dist}");
                    return -1;
                }
                var keys = c.keys;
                if (char.IsLower(c.currentLocation.poi))
                {
                    keys = keys.ToList();
                    if (!keys.Contains(c.currentLocation.poi))
                        keys.Add(c.currentLocation.poi);
                 //   Console.WriteLine($"key {c.currentLocation.poi} {c}");
                }

                var edgesarr = edges[c.currentLocation.poi].Where(p => !keys.Contains(p.p2.Item1)).ToArray();
                foreach (var n in edgesarr)
                {
                    if (n.deps.Count(d => keys.Contains(d)) != n.deps.Count()) continue;


                    var s = new State { dist = c.dist + n.distance, currentLocation = n.p2, keys = keys };
                    Console.WriteLine($"{c} ---> {s}");
                    current.Add(s);
                }
               // Console.WriteLine($">> {edgesarr.Count()}");
                if (edgesarr.Count() < 10)
                {
                   // Console.WriteLine(edgesarr[0]);
                }
                if (!edgesarr.Any())
                {
                    Console.WriteLine($"!!DONE {c}");
                    return c.dist;
                }
            }

        }

        public class State : IComparable<State>
        {
            public int dist;
            public (char poi, int x, int y) currentLocation;
            public List<char> keys;

            public override string ToString()
            {
                return $"{currentLocation} {dist}\t{string.Join(",",keys)}";
            }

            public int CompareTo([AllowNull] State other)
            {
                if (other == null) return 1;
                if (dist > other.dist) return 1;
                if (dist < other.dist) return -1;
                return 0;
            }
        }

        public class Edge
        {
            public int distance;
            public (char, int, int) p1;
            public (char, int, int) p2;
            public char[] deps;

            public override string ToString()
            {
                return $"{p1} => {p2} {distance}  deps={string.Join(",", deps)}";
            }
        }

        private void ClearMap(Dictionary<(int x, int y), char> map, Dictionary<char, (int x, int y)> pois)
        {
            foreach (var loc in map.Keys.ToArray())
            {
                if (map[loc] == '1')
                {
                    map[loc] = '.';
                }
            }
            foreach (var poi in pois)
            {
                map[poi.Value] = poi.Key;
            }
        }

        private List<Edge> DistanceTo(Dictionary<(int x, int y), char> area, (char, int x, int y) e)
        {
            var edges = new List<Edge>();
            var result = new Dictionary<(char c, int x, int y), (int dist, List<char> deps)>();
            AllDists(area, new List<(int, int, List<char>, char)>() { (e.x, e.y, new List<char>(), '1') }, result, 0);
            //DrawHull(area);
            foreach (var r in result)
            {
                Console.WriteLine($"{e} => {r.Key} {r.Value.dist} {string.Join(",", r.Value.deps)}");
                if (char.IsLower(r.Key.c) && r.Value.dist > 0)
                {
                    edges.Add(new Edge { distance = r.Value.dist, p1 = e, p2 = r.Key, deps = r.Value.deps.Select(c => char.ToLower(c)).ToArray() });
                }
            }
            return edges;
        }

        //private int GetDistance(Dictionary<(int, int), int> area, (int, int) from, (int, int) to)
        //{
        //    var todo = new SortedList<int, (int, int)>();
        //    todo.Add(0, from);

        //    while (todo.Any())
        //    {
        //        var p = todo.First();
        //        Console.WriteLine($"Process {p.Key} {p.Value}");
        //        todo.RemoveAt(0);

        //        foreach(var dir in Range(0,4))
        //        {
        //            var newpos = updatePosition(p.Value, dir);
        //            if (newpos != '#')
        //            {

        //            }
        //        }
        //    }
        //}

        private (Dictionary<(int x, int y), char> area, Dictionary<char, (int x, int y)> poi)
                Input(string filename)
        {
            var area = new Dictionary<(int x, int y), char>();
            var lines = File.ReadAllLines(filename);
            var poi = new Dictionary<char, (int, int)>();
            int y = 0;
            foreach (var line in lines)
            {
                int x = 0;
                foreach (var c in line)
                {
                    area.Add((x, y), c);
                    var C = char.ToUpper(c);
                    if (C >= 'A' && C <= 'Z' || C=='@')
                    {
                        poi.Add(c, (x, y));
                    }
                    x++;
                }
                y++;
            }

            return (area, poi);
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
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 4);
            //DrawHull(map, (0,0));

            return FindFastest(map, nextSteps, steps + 1, dest);
        }

        private int AllDists(Dictionary<(int, int), char> map, List<(int x, int y, List<char> dependencies, char fill)> current, Dictionary<(char, int, int), (int, List<char> dependencies)> dist, int steps)
        {
            var nextSteps = new List<(int x, int y, List<char> dependencies, char fill)>();
            foreach (var p in current)
            {
                var deps = p.dependencies;
                var c = (char)map[(p.x, p.y)];
                var C = char.ToUpper(c);
                if ((C >= 'A' && C <= 'Z' || C == '@'))
                {
                    dist.Add((c, p.x, p.y), (steps, p.dependencies)); // is a place dependent on itself?
                }
                if (c >= 'A' && c <= 'Z' && steps > 0)
                {
                    deps = deps.ToList();
                    deps.Add(c);
                }

                map[(p.x, p.y)] = p.fill;
                for (int i = 0; i < 4; i++)
                {
                    var newp = updatePosition((p.x, p.y), i);
                    if (map[newp] != '#' && map[newp] != p.fill)
                    {
                        nextSteps.Add((newp.x, newp.y, deps, p.fill));
                    }
                    //if (Math.Abs(map[newp] - p.fill) > 1 && map[newp] >= '0' && map[newp] <= '9')
                    //{
                       // Console.WriteLine($"@@@@@!!!!!!!!!!@@@@@@ {newp} {map[newp]} != {p.fill}");
//                        map[(p.x, p.y)] = '!';
                    //}
                }
                //DrawHull(map);
            }

            if (!nextSteps.Any()) 
                return steps;
            else
                return AllDists(map, nextSteps, dist, steps + 1);
        }

        private (int x, int y) updatePosition((int x, int y) pos, int dir)
        {
            switch(dir)
            {
                case 0: return (pos.x, pos.y - 1);
                case 1: return (pos.x + 1, pos.y);
                case 2: return (pos.x, pos.y + 1);
                case 3: return (pos.x - 1, pos.y);
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
            var program = File.ReadAllLines("day18.input")
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

        private void DrawHull(Dictionary<(int x, int y), char> area)
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
                    //if (robot.x - minx == x && robot.y - miny == y)
                    //{
                    //    if (hull[x, y] == '!')
                    //    {
                    //        Console.Write('*');
                    //    }
                    //    else
                    //    {
                    //        Console.Write('R');
                    //    }
                    //}
                    //else
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
