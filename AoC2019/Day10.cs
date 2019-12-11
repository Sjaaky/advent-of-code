using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AoC2019Test.Util;

namespace AoC2019Test
{
    public class Day10
    {
        [Test]
        public void Part1()
        {
            HashSet<(int, int)> asteroids = InputToAsteroids("day10.input");
            var asteroidBase = GetAsteroidBase(asteroids);

            Console.WriteLine($"base at {asteroidBase} {asteroidBase.sight.Count()}");
            Assert.AreEqual(296, asteroidBase.sight.Count());
            Assert.AreEqual((y:17, x:23), asteroidBase.asteroid);
        }

        [Test]
        public void Part2()
        {
            HashSet<(int, int)> asteroids = InputToAsteroids("day10.input");

            var asteroidBase = GetAsteroidBase(asteroids);
            var otherAsteroids = OtherAsteroids(asteroidBase);
            var zappedAsteroid = ZapAsteroids(otherAsteroids).Skip(199).First();

            int result = zappedAsteroid.x * 100 + zappedAsteroid.y;
            Console.WriteLine($"{zappedAsteroid}");
            Console.WriteLine($"{result}");

            Assert.AreEqual(204, result);
        }

        private
            HashSet<(int, int)> 
            InputToAsteroids(string filename)
        {
            HashSet<(int, int)> asteroids = new HashSet<(int, int)>();

            var lines = File.ReadAllLines(filename);
            int y = 0;
            foreach (var line in lines)
            {
                int x = 0;
                foreach (var c in line)
                {
                    if (c == '#')
                        asteroids.Add((x, y));
                    x++;
                }
                y++;
            }

            return asteroids;
        }
        
        // are tuples still handy when the datastructure gets large?
        private
            ((int x, int y) asteroid, IEnumerable<IGrouping<(int n, int d), ((int, int) asteroid, (int dx, int dy) vec)>> sight)
            GetAsteroidBase(HashSet<(int, int)> asteroids)
        {
            var r = asteroids
                .Select(a => (
                        asteroid: a,
                        sight: asteroids
                                .Where(b => b != a)
                                .Select(b => (asteroid: b, vec: Vector(a, b)))
                                .GroupBy(v => Simplify(v.vec))))
                .OrderByDescending(t => t.sight.Count())
                .ToList();

            return r.First();
        }

        private
            List<(double angle, List<((int, int) asteroid, (int dx, int dy) vec)> asteroids)> 
            OtherAsteroids(((int x, int y) asteroid, IEnumerable<IGrouping<(int n, int d), ((int, int) asteroid, (int dx, int dy) vec)>> sight) asteroidBase)
        {
            return asteroidBase.sight
                    .Select(o => (
                            angle: VectorToAngle(o.Key),
                            asteroids: o.OrderBy(o => Distance(o.vec)).ToList()))
                    .OrderBy(a => a.angle).ToList();
        }

        private 
            IEnumerable<(int x, int y)>
            ZapAsteroids(List<(double angle, List<((int, int) asteroid, (int dx, int dy) vec)> asteroids)> linesOfSight)
        {
            int zaps = 0;
            int idx = 0;
            int count = linesOfSight.Sum(pos => pos.asteroids.Count);
            while (zaps < count)
            {
                var losAsteroids = linesOfSight[idx].asteroids;
                if (losAsteroids.Count() > 0)
                {
                    zaps++;
                    yield return (losAsteroids[0].asteroid);
                    losAsteroids.RemoveAt(0);
                }
                idx++;
                if (idx >= linesOfSight.Count()) idx = 0;
            }
        }

        private double Distance((int, int) o)
        {
            return Math.Sqrt(o.Item1 * o.Item1 + o.Item2 * o.Item2);
        }

        public double VectorToAngle((int d, int n) v)
        {
            return (Math.Atan2(-v.d, v.n) * (180 / Math.PI) + 360) % 360;
        }

        public (int, int) Vector((int x, int y) a, (int x, int y) b)
        {
            var n = a.x - b.x;
            var d = a.y - b.y;
            return (n, d);
        }

        private (int n, int d) Simplify((int n, int d) p)
        {
            for(int i = Math.Abs(p.d); i >= 2; i--)
            {
                if (p.n % i == 0 && p.d % i == 0)
                {
                    return (p.n / i, p.d / i);
                }
            }
            if (p.d == 0) p.n = Math.Sign(p.n);
            return (p.n, p.d);
        }

        [Test]
        public void TestS1()
        {
            Assert.AreEqual((-1, 0), Simplify((-4, 0)));
            Assert.AreEqual((0, -1), Simplify((0, -4)));
            Assert.AreEqual((1, 0), Simplify((4, 0)));
            Assert.AreEqual((0, 1), Simplify((0, 4)));
            Assert.AreEqual((3, 2), Simplify((6, 4)));
            Assert.AreEqual((3, 2), Simplify((9, 6)));
            Assert.AreEqual((3, 2), Simplify((12, 8)));
            Assert.AreEqual((4, 3), Simplify((12, 9)));
            Assert.AreEqual((0, 1), Simplify((0, 9)));
        }

        [Test]
        public void TestVectorToAngle()
        {
            Assert.AreEqual(0, VectorToAngle((0, 1)));
            Assert.AreEqual(90, VectorToAngle((-1, 0)));
            Assert.AreEqual(180, VectorToAngle((0, -1)));
            Assert.AreEqual(270, VectorToAngle((1, 0)));
        }

        [Test]
        public void Part1test1()
        {
            HashSet<(int, int)> asteroids = InputToAsteroids("day10.test1.input");
            var asteroidBase = GetAsteroidBase(asteroids);
            Assert.AreEqual((5, 8), asteroidBase.asteroid);
            Assert.AreEqual(33, asteroidBase.sight.Count());
        }

        [Test]
        public void Part1test2()
        {
            HashSet<(int, int)> asteroids = InputToAsteroids("day10.test2.input");
            var asteroidBase = GetAsteroidBase(asteroids);
            Assert.AreEqual((1, 2), asteroidBase.asteroid);
            Assert.AreEqual(35, asteroidBase.sight.Count());
        }

        [Test]
        public void Part2Test1()
        {
            HashSet<(int, int)> asteroids = InputToAsteroids("day10.test3.input");

            var asteroidBase = GetAsteroidBase(asteroids);
            var otherAsteroids = OtherAsteroids(asteroidBase);
            var zappedAsteroid = ZapAsteroids(otherAsteroids).ToArray();

            Assert.AreEqual((11, 12), zappedAsteroid[0]);
            Assert.AreEqual((12, 1), zappedAsteroid[1]);
            Assert.AreEqual((12, 2), zappedAsteroid[2]);
            Assert.AreEqual((12, 8), zappedAsteroid[9]);
            Assert.AreEqual((16, 0), zappedAsteroid[19]);
            Assert.AreEqual((16, 9), zappedAsteroid[49]);
            Assert.AreEqual((10, 16), zappedAsteroid[99]);
            Assert.AreEqual((9, 6), zappedAsteroid[198]);
            Assert.AreEqual((8, 2), zappedAsteroid[199]);
            Assert.AreEqual((10, 9), zappedAsteroid[200]);
            Assert.AreEqual((11, 1), zappedAsteroid[298]);
        }
    }
}
