using LanguageExt;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static AoC2019Test.Util;

namespace AoC2019Test
{

    public class Day25
    {
        List<string> inventory;
        IntCodeComputer computer;
        List<string> blacklist = new List<string>
        {
            "photons",
            "giant electromagnet",
            "molten lava",
            "infinite loop",
            "escape pod",
        };

        //        Items in your inventory:
        //- monolith > too light
        //- antenna  > too heavy
        //- astronaut ice cream > too light
        //- hologram > too light
        //- ornament > too light
        //- asterisk > too light
        //- fixed point > too light
        //- dark matter > too light
        Dictionary<(int, int), char> map = new Dictionary<(int, int), char>();
        (int x, int y) position = (0, 0);
        (int x, int y)? lastDelta;

        [Test]
        public void Part1()
        {
            var program = File.ReadAllLines("day25.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();

            inventory = new List<string>();
            computer = new IntCodeComputer(program, true, () => LogCommand(GiveInput));

            while (!computer.IsHalted)
            {

                computer.Execute(new List<bigint>());
                //Console.Write((char)computer.Output.Last());
                //if ((char)computer.Output.Last() == '=' && lastDelta.HasValue)
                //{
                //    position.x += lastDelta.Value.x;
                //    position.y += lastDelta.Value.y;
                //    map[position] = '.';
                //    DrawHull(map);
                //    lastDelta = null;
                //}
            }
            Console.WriteLine(ReadOutput());

        }

        private static void PrintProgram(bigint[] program)
        {
            foreach (var i in program)
            {
                if (i > 0 && i < 255)
                {
                    var c = (char)i;
                    Console.Write(c);
                }
            }
        }

        private static void PrintProgram(Dictionary<int, bigint> program)
        {
            foreach (var k in program.OrderBy(kv => kv.Key))
            {
                var i = k.Value;    
                if (i > 0 && i < 255)
                {
                    var c = (char)i;
                    Console.Write(c);
                }
            }
        }

        private string LogCommand(Func<string> next)
        {
            var command = next();
            Console.WriteLine($"Command: {command}");
            return command;
        }

        private int outputPtr;
        private string ReadOutput()
        {
            var newOutputPtr = computer.Output.Count;
            var output = new string(computer.Output.Skip(outputPtr).Select(c => (char)c).ToArray());
            outputPtr = newOutputPtr;
            return output;
        }

        Dictionary<string, Spot> spots = new Dictionary<string, Spot>();
        System.Collections.Generic.HashSet<(string loc, string dir)> visited = new System.Collections.Generic.HashSet<(string loc, string dir)>();
        Spot currentSpot = null;
        bool checkperms = false;

        private string GiveInput()
        {
            string outputx = ReadOutput();
            Console.WriteLine(outputx);
          //  PrintProgram(computer.Memory);

            Console.WriteLine();
            currentSpot = ParseOutput(outputx, currentSpot);
            if (currentSpot.name == "Security Checkpoint" && inventory.Count == 8)
            //{
            //    checkperms = true;
            //}
            //if (checkperms)
            {
                var perm = TryAllPermutations(currentSpot);
                if (perm != null)
                {
                    return perm;
                }
            }
            else
            {
                var pickupitem = PickupAnything(currentSpot);
                if (pickupitem != null)
                {
                    inventory.Add(pickupitem);
                    return $"take {pickupitem}\n";
                }
                var dir = FindDirection(spots, currentSpot, visited);
                if (dir != null)
                {
                    return $"{dir}\n";
                }
            }
            var k = Console.ReadKey();
            switch (k.Key)
            {
                case ConsoleKey.UpArrow:
                    lastDelta = (0, -1);
                    return "north\n";
                case ConsoleKey.DownArrow:
                    lastDelta = (0, 1);
                    return "south\n";
                case ConsoleKey.LeftArrow:
                    lastDelta = (-1, 0);
                    return "west\n";
                case ConsoleKey.RightArrow:
                    lastDelta = (1, 0);
                    return "east\n";
                case ConsoleKey.D:
                    ShowInventory();
                    var itemnr = Console.ReadKey();
                    if (int.TryParse(""+itemnr.KeyChar, out var nr) && nr >= 0 && nr < inventory.Count)
                    {
                        var inv = inventory[nr];
                        inventory.RemoveAt(nr);
                        Console.WriteLine($"drop {inv}\n");
                        return $"drop {inv}\n";
                    }
                    else
                    {
                        Console.WriteLine("invalid inventory nr");
                        return "";
                    }
                case ConsoleKey.I:
                    return "inv\n";
                case ConsoleKey.U:
                    var output = new string(computer.Output.Select(x => (char)x).ToArray());
                    var itemsProlog = "Items here:";
                    var itemsIdx = output.LastIndexOf(itemsProlog);
                    var commandIdx = output.IndexOf("Command?", itemsIdx);
                    if (itemsIdx > 0 && commandIdx > itemsIdx)
                    {
                        var itemsStr = output.Substring(itemsIdx+ itemsProlog.Length, commandIdx - (itemsIdx + itemsProlog.Length));
                        var items = itemsStr.Split("-", StringSplitOptions.RemoveEmptyEntries);
                        Console.WriteLine("== Items ==");
                        int i = 0;
                        foreach(var item in items)
                        {
                            Console.WriteLine($"{i} {item.Trim()}");
                            i++;
                        }
                        Console.WriteLine("== Items ==");
                        var itemnrd = Console.ReadKey();
                        if (int.TryParse("" + itemnrd.KeyChar, out var nrd) && nrd >= 0 && nrd < items.Count())
                        {
                            var newItem = items[nrd].Trim();
                            if (blacklist.Contains(newItem))
                            {
                                Console.WriteLine($"Item {newItem} is blacklisted");
                            }
                            else
                            {
                                inventory.Add(newItem);
                                Console.WriteLine($"take {newItem}\n");
                                return ($"take {newItem}\n");
                            }
                        }
                    }
                    return "\n";

            }
            return "\n";
        }

        List<string> allItems = null;
        List<string> currentItems = null;
        PermState state;
        public enum PermState
        {
            Dropping,
            Walking,
            Pickup
        }

        int perms = 1;
        int currentPickups = 0;
        private string TryAllPermutations(Spot currentSpot)
        {
            if (allItems == null)
            {
                allItems = inventory.OrderBy(i => i).ToList();
                currentItems = inventory.OrderBy(i => i).ToList();
            }
            var again = false;
            do
            {
                again = false;
                switch (state)
                {
                    case PermState.Dropping:
                        if (!currentItems.Any())
                        {
                            state = PermState.Pickup;
                            again = true;
                        }
                        else
                        {
                            var x = currentItems.First();
                            currentItems.Remove(x);
                            return $"drop {x}\n";
                        }
                        break;
                    case PermState.Walking:
                        if (currentSpot.name == "Security Checkpoint")
                        {
                            state = PermState.Dropping;
                            return "east\n";
                        }
                        break;
                    case PermState.Pickup:
                        if (currentPickups < 8)
                        {
                            Console.WriteLine($"\t\t PERM {perms} | ========");
                            if ((perms & (1 << currentPickups)) > 0)
                            {
                                var pickedup = allItems[currentPickups];
                                currentItems.Add(pickedup);
                                currentPickups++;
                                return $"take {pickedup}\n";
                            }
                            else
                            {
                                currentPickups++;
                                again = true;
                            }
                        }
                        else
                        {
                            if (perms < 255)
                            {
                                perms++;
                                currentPickups = 0;
                                state = PermState.Walking;
                                again = true;
                            }
                        }
                        break;
                }
            }
            while (again);
            
            return null;
        }
        
        private string PickupAnything(Spot currentSpot)
        {
            if (currentSpot.name == "Security Checkpoint")
            {
                return null;
            }
            if (currentSpot.items != null)
            {
                var item =  currentSpot.items.FirstOrDefault(i => !blacklist.Contains(i));
                currentSpot.items = currentSpot.items.Where(i => i != item).ToArray();
                return item;
            }
            else
            {
                return null;
            }
        }

        class Spot
        {
            public string name;
            public string[] directions;
            public string[] items;
            public (int, int) location;
        }


        Regex locationReg = new Regex(@"== (?<location>.*?) ==");
        Regex doorsReg = new Regex(@"Doors here lead:\n(- (?<direction>east|south|north|west)\n)*", RegexOptions.Multiline);
        Regex itemsReg = new Regex(@"Items here:\n(- (?<items>.*?)\n)*", RegexOptions.Multiline);
        private Spot ParseOutput(string output, Spot currentSpot)
        {
            var spot = new Spot();
            var matchLoc = locationReg.Match(output);
            if (matchLoc.Success)
            {
                var nm = matchLoc.NextMatch();
                if (nm.Success) matchLoc = nm;
                Console.WriteLine($"Location:\t{matchLoc.Groups["location"].Value}");
                var spotname = matchLoc.Groups["location"].Value;
                if (spotname == "Pressure - Sensitive Floor" &&
                    (!output.Contains("heavier") && !output.Contains("lighter")))
                {

                }
                if (spots.ContainsKey(spotname))
                {
                    spot = spots[spotname];
                    Console.WriteLine(" visited before");
                }
                else
                {
                    spot = new Spot() { name = spotname };
                    spots.Add(spotname, spot);
                    Console.WriteLine("** new spot **");
                }
            }
            else
            {
                return currentSpot;
            }
            var matchDoorst = doorsReg.Match(output);
            if (matchDoorst.Success)
            {
                var dirs = matchDoorst.Groups["direction"].Captures.Select(c => c.Value).ToArray();
                Console.WriteLine($"Dirs:\t\t{string.Join(",", dirs)}");
                spot.directions = dirs;
            }
            var matchItems = itemsReg.Match(output);
            if (matchItems.Success)
            {
                var items = matchItems.Groups["items"].Captures.Select(c => c.Value).ToArray();
                Console.WriteLine($"Items:\t{string.Join(",", items)}");
                spot.items = items;
            }
            return spot;
        }

        private void ShowInventory()
        {
            Console.WriteLine("Inventory");
            int i = 1;
            if (inventory != null && inventory.Any())
            {
                foreach (var inv in inventory)
                {
                    Console.WriteLine($"{i} {inv}");
                    i++;
                }
            }
        }

        private string FindDirection(Dictionary<string, Spot> spots, Spot currentSpot, System.Collections.Generic.HashSet<(string loc, string dir)> visited)
        {
            foreach (string dir in currentSpot.directions)
            {

                if (visited.Contains((currentSpot.name, dir)))
                {
                    continue;
                }
                else
                {
                    visited.Add((currentSpot.name, dir));
                    return dir;
                }
            }
            //foreach (string dir in currentSpot.directions)
            //{
            //    return dir;
            //}
            return null;
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
            var cl = Console.CursorLeft;
            var ct = Console.CursorTop;
            Console.SetCursorPosition(1, 1);
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
            Console.SetCursorPosition(cl, ct);
            
        }
    }
}
