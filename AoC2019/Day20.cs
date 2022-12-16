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

    public class Day20
    {
        [Test]
        public void Part1()
        {
            var sw = Stopwatch.StartNew();
            var (area, pois) = Input("day20.input");
            //DrawHull(area);
            foreach(var poi in pois)
            {
                Console.WriteLine($"{poi.Key} {string.Join(",", poi.Value.p1)}");
            }
            DoIt(area, pois);
            Console.WriteLine(sw.Elapsed);
        }

        [Test]
        public void Part1t1()
        {
            var (area, pois) = Input("day20.test1.input");
            foreach (var poi in pois)
            {
                Console.WriteLine($"{poi.Key} {string.Join(",", poi.Value.p1)}");
            }
            DoIt(area, pois);
        }

        [Test]
        public void Part1t2()
        {
            var (area, pois) = Input("day20.test2.input");
            foreach (var poi in pois)
            {
                Console.WriteLine($"{poi.Key} {string.Join(",", poi.Value.p1)}");
            }
            DoIt(area, pois);
        }

        [Test]
        public void Part1t3()
        {
            var (area, pois) = Input("day20.test3.input");
            foreach (var poi in pois)
            {
                Console.WriteLine($"{poi.Key} {string.Join(",", poi.Value.p1)}");
            }
            DoIt(area, pois);
        }

        private int DoIt(Dictionary<(int x, int y), string> area, Dictionary<string, (string label, (int x, int y)[] p)> pois)
        {
            var edges = new Dictionary<string, List<Edge>>();
            foreach (var poi in pois)
            {
                for (int i = 0; i < poi.Value.p.Length; i++)
                {
                    var edge1 = DistanceTo(area, (poi.Value.label, poi.Value.p[i].x, poi.Value.p[i].y));
                    foreach (var edge in edge1)
                    {
                        if (!edges.ContainsKey(poi.Value.label))
                        {
                            edges.Add(poi.Value.label, new List<Edge>());
                        }
                        edges[poi.Value.label].Add(edge);
                    }
                    ClearMap(area);
                }
            }

            var s = new State { dist = 0, currentLocation = ("AA", pois["AA"].p[0].x, pois["AA"].p[0].y), level = 0 };
            var dest = (label: "ZZ", pois["ZZ"].p[0].x, pois["ZZ"].p[0].y, level: 0);
            var current = new MySortedSet();
            current.Add(s);
            return FindRoute(edges, current, dest, pois.Count());
        }

        private int FindRoute(Dictionary<string, List<Edge>> edges, MySortedSet current, (string label, int x, int y, int level) dest, int portals)
        {
            int display = 0;
            while (true)
            {
                var c = current.Pop();
                if (c == null)
                {
                    Console.WriteLine("DON!");
                    return -1;
                }

                if (c.currentLocation.poi == dest.label && c.level == dest.level)
                {
                    Console.WriteLine($"DONE {c} => {c.dist-1}");
                    return c.dist;
                }
                var edgesarr = edges[c.currentLocation.poi].Where(p => p.p2.Item1 != c.currentLocation.poi).ToArray();
                foreach (var n in edgesarr)
                {
                    var newlevel = c.level + n.levelDelta;
                    if (newlevel < 0) continue;
                    if (newlevel > 0 && (n.p2.Item1 == "AA" || n.p2.Item1 == "ZZ")) continue;
                    if (newlevel > portals) continue;
                    var s = new State { dist = c.dist + n.distance, currentLocation = n.p2, level = c.level + n.levelDelta };
                    if (display++ % 10000==0) Console.WriteLine($"{c} -> {s}");
                    current.Add(s);
                }
                if (!edgesarr.Any())
                {
                    Console.WriteLine($"!!DONE {c} => {c.dist - 1}");
                    return c.dist;
                }
            }
        }

        public class State : IEquatable<State>
        {
            public int dist;
            public (string poi, int x, int y) currentLocation;
            public int level;

            public override string ToString()
            {
                return $"S: {GetHashCode()} {currentLocation} {dist} {level}";
            }

            public int Heuristic => dist + (level * 30);

            public bool Equals([AllowNull] State other)
            {
                if (other is null) return false;
                return (dist == other.dist &&
                    currentLocation == other.currentLocation);
            }
        }

        public class MySortedSet
        {
            SortedList<int, List<State>> n = new SortedList<int, List<State>>();

            public void Add(State s)
            {
                var heur = s.Heuristic;
                if (n.ContainsKey(heur))
                {
                    if (!n[heur].Any(i => i.Equals(s)))
                    {
                        n[heur].Add(s);
                    }
                }
                else
                {
                    n.Add(heur, new List<State>() { s });
                }
            }

            public State Pop()
            {
                var l = n.FirstOrDefault();
                if (l.IsDefault()) return null;
                if (l.Value.Count <= 1)
                {
                    n.RemoveAt(0);
                }
                var result = l.Value.FirstOrDefault();
                l.Value.RemoveAt(0);
                return result;
            }
        }


        public class Edge
        {
            public int distance;
            public (string, int, int) p1;
            public (string, int, int) p2;
            public int levelDelta;

            public override string ToString()
            {
                return $"E: {p1} => {p2} {distance}";
            }
        }

        private void ClearMap(Dictionary<(int x, int y), string> map)
        {
            foreach (var loc in map.Keys.ToArray())
            {
                if (map[loc] == FILL)
                {
                    map[loc] = ".";
                }
            }
        }

        private List<Edge> DistanceTo(Dictionary<(int x, int y), string> area, (string label, int x, int y) start)
        {
            var edges = new List<Edge>();
            var result = new List<(string label, int x, int y, int dist)>();
            var maxx = area.Max(l => l.Key.x) - 1;
            var maxy = area.Max(l => l.Key.y) - 1;
            AllDists(area, new List<(int, int, int)>() { (start.x, start.y, 0) }, result, 0);
            foreach (var r in result)
            {
                if (start.label != r.label)
                {
                    var startOuter = start.x == 1 || start.x == maxx || start.y == 1 || start.y == maxy;
                    var endOuter = r.x == 1 || r.x == maxx || r.y == 1 || r.y == maxy;
                    var edge = new Edge
                    {
                        distance = r.dist,
                        p1 = start,
                        p2 = (r.label, r.x, r.y),
                        levelDelta = (startOuter ? 1 : 0) - (endOuter ? 1 : 0),
                    };
                    edges.Add(edge);
                    Console.WriteLine(edge);
                }
            }
            return edges;
        }


        private (Dictionary<(int x, int y), string> area, Dictionary<string, (string portal, (int x, int y)[] p1)> poi)
                Input(string filename)
        {
            var area = new Dictionary<(int x, int y), string>();
            var lines = File.ReadAllLines(filename);
            var poi = new Dictionary<string, (string, (int, int)[])>();
            int y = 0;
            foreach (var line in lines)
            {
                int x = 0;
                foreach (var c in line)
                {
                    area.Add((x, y), ""+c);
                    x++;
                }
                y++;
            }

            var portals = FoldAndFindPortalLabels(area);

            return (area, portals);
        }

        private Dictionary<string, (string portal, (int x, int y)[] coords)> FoldAndFindPortalLabels(Dictionary<(int x, int y), string> area)
        {
            var portals = new Dictionary<string, (string portal, (int x, int y)[] coords)>();
            foreach (var l in new Dictionary<(int x, int y), string>(area))
            {
                var p1 = l.Key;
                var id = l.Value;
                if (id[0] >= 'A' && id[0] <= 'Z' && id.Length == 1)
                {
                    for (int d = 1; d < 3; d++)
                    {
                        var p2 = updatePosition(p1, d);
                        if (!area.ContainsKey(p2)) continue;
                        var id2 = area[p2];
                        var p3 = updatePosition(p2, d);
                        var toClear = p1;
                        var center = p2;
                        if (!area.ContainsKey(p3) || area[p3] != ".")
                        {
                            p3 = updatePosition(p1, (d + 2) % 4);
                            if (!area.ContainsKey(p3)) continue;
                            toClear = p2;
                            center = p1;
                        }

                        if (area[p3] == "." && id2[0] >= 'A' && id2[0] <= 'Z')
                        {
                            var label = id + id2;
                            area[center] = label;
                            area[toClear] = " ";

                            if (portals.ContainsKey(label))
                            {
                                portals[label] = (label, new[] { portals[label].coords[0], center });
                            }
                            else
                            {
                                portals.Add(label, (label, new[] { center }));
                            }
                        }
                    }
                }
            }
            return portals;
        }

        const string FILL = "!";
        private int AllDists(Dictionary<(int, int), string> map, List<(int x, int y, int dist)> current, List<(string label, int x, int y, int dist)> result, int steps)
        {
            var nextSteps = new List<(int x, int y, int dist)>();
            foreach (var p in current)
            {
                for (int i = 0; i < 4; i++)
                {
                    var newp = updatePosition((p.x, p.y), i);
                    if (map[newp] == "." && map[newp] != FILL)
                    {
                        nextSteps.Add((newp.x, newp.y, steps));
                        map[newp] = FILL;
                    }
                    else if(map[newp].Length == 2)
                    {
                        result.Add((map[newp], newp.x, newp.y, steps));
                    }
                }
            }

            if (!nextSteps.Any()) 
                return steps;
            else
                return AllDists(map, nextSteps, result, steps + 1);
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

        private void DrawHull(Dictionary<(int x, int y), string> area)
        {
            var minx = area.Min(c => c.Key.x);
            var miny = area.Min(c => c.Key.y);

            var width = area.Max(c => c.Key.x) - minx + 1;
            var height = area.Max(c => c.Key.y) - miny + 1;
            string[,] hull = new string[width, height];

            foreach (var p in area)
            {
                hull[p.Key.x - minx, p.Key.y - miny] = p.Value;
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x == 0 && y == 0)
                    {
                        //Console.Write("o_");
                        Console.Write("o");
                    }
                    else
                    {
                        var p = hull[x, y];
                        if (p.Length == 1) p += "_";
                        Console.Write(p.Substring(0,1));
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
