using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2019Test
{
    public partial class Day6
    {
        [Test]
        public void Part1()
        {
            var lines = File.ReadAllLines("day6.input");
            var objects = CalcOrbits(lines);

            var orbits = objects.Values.Sum(o => o.Orbits);
            Console.WriteLine(orbits);
            Assert.AreEqual(253104, orbits);
        }

        [Test]
        public void Part2()
        {
            var lines = File.ReadAllLines("day6.input");
            var objects = CalcOrbits(lines);

            var you = objects["YOU"].Parent;
            var santa = objects["SAN"].Parent;

            var commonAncestor = you;
            var steps = 0;
            while (commonAncestor != null)
            {
                var santaToCommon = santa.StepsTo(commonAncestor);
                if (santaToCommon.HasValue)
                {
                    steps += santaToCommon.Value;
                    break;
                }
                steps++;
                commonAncestor = commonAncestor.Parent;
            }

            Console.WriteLine(steps);
            Assert.AreEqual(499, steps);
        }

        public Dictionary<string, SpaceObject> CalcOrbits(string[] lines)
        {
            var objects = new Dictionary<string, SpaceObject>();
            foreach (var line in lines)
            {
                var x = line.Split(')', StringSplitOptions.RemoveEmptyEntries);
                var parentName = x[0];
                var newObjectName = x[1];

                if (!objects.TryGetValue(parentName, out var parent))
                {
                    parent = new SpaceObject(parentName);
                    objects.Add(parentName, parent);
                }
                if (!objects.TryGetValue(newObjectName, out var newObject))
                {
                    newObject = new SpaceObject(newObjectName);
                    objects.Add(newObjectName, newObject);
                }

                newObject.Parent = parent;
            }

            foreach (var o in objects.Values)
            {
                var obj = o;
                while (obj.Parent != null && obj.Orbits == 0)
                {
                    o.Orbits++;
                    obj = obj.Parent;
                }
                if (obj.Parent != null)
                {
                    o.Orbits += obj.Orbits;
                }
            }

            return objects;
        }

        [Test]
        public void Example1()
        {
            var objects = CalcOrbits(new[]{
                "COM)B",
                "C)D",
                "D)E",
                "E)F",
                "B)G",
                "G)H",
                "D)I",
                "E)J",
                "J)K",
                "K)L",
                "B)C"
            });
            int orbits = objects.Values.Sum(o => o.Orbits);
            Assert.AreEqual(42, orbits);
        }

        public class SpaceObject
        {
            public string Name;
            public SpaceObject Parent;
            public int Orbits;

            public SpaceObject(string name)
            {
                Name = name;
            }

            public int? StepsTo(SpaceObject other)
            {
                int steps = 0;
                var a = this;
                while (a != null && a != other)
                {
                    steps++;
                    a = a.Parent;
                }
                return (a == other) ? steps : default(int?);
            }
        }
    }
}
