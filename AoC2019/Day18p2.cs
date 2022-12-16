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

    public class Day18p2
    {
        [Test]
        public void Part2()
        {
            var (area, pois) = Input("day18_2.input");
            var r = DoIt(area, pois);
            Assert.AreEqual(2082, r);
            Console.WriteLine(r);
        }

        [Test]
        public void Part2t1()
        {
            var (area, pois) = Input("day18_2.test1.input");
            Assert.AreEqual(8, DoIt(area, pois));
        }

        private int DoIt(Dictionary<(int x, int y), char> area, Dictionary<char, (int x, int y)> pois)
        {
            DrawHull(area);
            var edges = new Dictionary<char, List<Edge>>();

            foreach (var poi in pois.Where(p => char.IsLower(p.Key) || char.IsDigit(p.Key)))
            {
                var edge1 = DistanceTo(area, (poi.Key, poi.Value.x, poi.Value.y));
                edges[poi.Key] = edge1.OrderBy(e => e.distance).ToList();
                ClearMap(area, pois);
            }
            var current = new MySortedSet();

            var l1 = ('1', pois['1'].x, pois['1'].y);
            var l2 = ('2', pois['2'].x, pois['2'].y);
            var l3 = ('3', pois['3'].x, pois['3'].y);
            var l4 = ('4', pois['4'].x, pois['4'].y);
            var s1 = new State { dist = 0, l = new[] { l1, l2, l3, l4 }, keys = new List<char>() };
            current.Add(s1);
            return FindRoute(edges, current, pois.Count(p => char.IsLower(p.Key)));
        }

        private int FindRoute(Dictionary<char, List<Edge>> edges, MySortedSet current, int targetKeys)
        {
            while (true)
            {
                var c = current.Pop();
                if (c == null)
                {
                    Console.WriteLine("DONE but no answer");
                    return -1;
                }

                if (c.keys.Count == targetKeys)
                {
                    Console.WriteLine($"DONE {c}");
                    return c.dist;
                }

                List<char> visited = new List<char>();
                int edgesToGo = 0;
                for (int i = 0; i < 4; i++)
                {
                    var edgesarr = edges[c.l[i].poi].Where(p => p.p2.Item1 != c.l[i].poi && !c.keys.Contains(p.p2.Item1)).ToArray();
                    edgesToGo += edgesarr.Length;
                    foreach (var n in edgesarr)
                    {
                        if (n.deps.Count(d => d == c.l[i].poi || c.keys.Contains(d)) != n.deps.Count()) continue;

                        visited.Add(n.p2.Item1);

                        List<char> keys2 = c.keys.ToList();
                        foreach (var k in n.keys)
                        {
                            if (!keys2.Contains(k))
                                keys2.Add(k);
                        }
                        if (char.IsLower(c.l[i].poi) && !keys2.Contains(c.l[i].poi))
                            keys2.Add(c.l[i].poi);
                        if (!keys2.Contains(n.p2.Item1))
                            keys2.Add(n.p2.Item1);

                        var locs = c.l.ToArray();
                        locs[i] = n.p2;
                        var s = new State { dist = c.dist + n.distance, l = locs, keys = keys2 };
                        current.Add(s);
                    }

                }
                if (edgesToGo == 0)
                {
                    Console.WriteLine($"!!DONE {c}");
                    return c.dist;
                }
            }
        }

        public class State : IEquatable<State>
        {
            public int dist;
            public (char poi, int x, int y)[] l = new (char poi, int x, int y)[4];
            public List<char> keys;

            public override string ToString()
            {
                return $"S: {GetHashCode()} {l[0]} {l[1]} {l[2]} {l[3]} {dist}\t{string.Join(",",keys)}";
            }

            public int Heuristic => dist + keys.Count * 5;

            public bool Equals([AllowNull] State other)
            {
                if (other is null) return false;
                return (dist == other.dist &&
                    l[0] == other.l[0] &&
                    l[1] == other.l[1] &&
                    l[2] == other.l[2] &&
                    l[3] == other.l[3] &&
                    !keys.Except(other.keys).Any() &&
                    !other.keys.Except(keys).Any());
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
            public (char, int, int) p1;
            public (char, int, int) p2;
            public char[] deps;
            public char[] keys;

            public override string ToString()
            {
                return $"E: {p1} => {p2} {distance}  deps={string.Join(",", deps)} keys={string.Join(",", keys)}";
            }
        }

        private void ClearMap(Dictionary<(int x, int y), char> map, Dictionary<char, (int x, int y)> pois)
        {
            foreach (var loc in map.Keys.ToArray())
            {
                if (map[loc] == VISITED)
                {
                    map[loc] = '.';
                }
            }
            foreach (var poi in pois)
            {
                map[poi.Value] = poi.Key;
            }
        }

        private const char VISITED = '!';
        private List<Edge> DistanceTo(Dictionary<(int x, int y), char> area, (char, int x, int y) e)
        {
            var edges = new List<Edge>();
            var result = new Dictionary<(char c, int x, int y), (int dist, List<char> deps, List<char> keys)>();
            AllDists(area, new List<(int, int, List<char>, List<char>, char)>() { (e.x, e.y, new List<char>(), new List<char>(), VISITED) }, result, 0);
            foreach (var r in result)
            {
                if (char.IsLower(r.Key.c) && r.Value.dist > 0)
                {
                    var edge = new Edge { 
                        distance = r.Value.dist, 
                        p1 = e,
                        p2 = r.Key,
                        deps = r.Value.deps.Select(c => char.ToLower(c)).ToArray(),
                        keys = r.Value.keys.ToArray()
                    };
                    edges.Add(edge);
                    Console.WriteLine(edge);
                }
            }
            return edges;
        }

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
                    if (C >= 'A' && C <= 'Z' || char.IsDigit(C))
                    {
                        poi.Add(c, (x, y));
                    }
                    x++;
                }
                y++;
            }

            return (area, poi);
        }

        private int AllDists(Dictionary<(int, int), char> map, List<(int x, int y, List<char> dependencies, List<char> keys, char fill)> current, Dictionary<(char, int, int), (int, List<char> dependencies, List<char> keys)> dist, int steps)
        {
            var nextSteps = new List<(int x, int y, List<char> dependencies, List<char> keys, char fill)>();
            foreach (var p in current)
            {
                var deps = p.dependencies;
                var keys = p.keys;
                var c = (char)map[(p.x, p.y)];
                var C = char.ToUpper(c);
                if ((c >= 'a' && c <= 'z' || C == '@'))
                {
                    dist.Add((c, p.x, p.y), (steps, p.dependencies, p.keys)); // is a place dependent on itself?
                }
                if (c >= 'A' && c <= 'Z' && steps > 0)
                {
                    deps = deps.ToList();
                    deps.Add(c);
                }
                if ((c >= 'a' && c <= 'z'))
                {
                    keys = keys.ToList();
                    keys.Add(c);
                }
                map[(p.x, p.y)] = p.fill;

                
                for (int i = 0; i < 4; i++)
                {
                    var newp = updatePosition((p.x, p.y), i);
                    if (map[newp] != '#' && map[newp] != p.fill)
                    {
                        nextSteps.Add((newp.x, newp.y, deps, keys, p.fill));
                    }
                }
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

      

        private void DrawHull(Dictionary<(int x, int y), char> area)
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
