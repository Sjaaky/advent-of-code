using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static AoC2019Test.Util;

namespace AoC2019Test
{
    public class Day3
    {
        private static Point CentralPoint = new Point(0, 0);

        [Test]
        public void Part1()
        {
            var str = File.ReadAllLines($"day3.input");
            var wires = ParseInput(str).ToArray();

            var allIntersections = DetermineIntersections(wires).ToArray();

            var nearestIntersection = allIntersections
                .Where(t => t.ManhattanDistanceToCentral > 0)
                .OrderBy(t => t.ManhattanDistanceToCentral)
                .FirstOrDefault();

            Console.WriteLine(nearestIntersection.ManhattanDistanceToCentral);
            Assert.AreEqual(217, nearestIntersection.ManhattanDistanceToCentral);
        }

        [Test]
        public void Part2()
        {
            var str = File.ReadAllLines($"day3.input");
            var wires = ParseInput(str).ToArray();

            var allIntersections = DetermineIntersections(wires).ToArray();

            var nearestIntersection = allIntersections
                .Where(t => t.Distance > 0)
                .OrderBy(t => t.Distance)
                .FirstOrDefault();

            Console.WriteLine(nearestIntersection.Distance);
            Assert.AreEqual(3454, nearestIntersection.Distance);
        }

        private IEnumerable<Intersection> DetermineIntersections(Line[] wires)
        {
            foreach (var w1 in wires)
            {
                foreach (var w2 in wires)
                {
                    if (w1.LineNr == w2.LineNr) continue;
                    foreach (var i in Intersections(w1, w2))
                    {
                        yield return new Intersection(w1, w2, i);
                    }
                }
            }
        }

        public IEnumerable<Point> Intersections(Line w1, Line w2)
        {
            if (w1.A.X == w1.B.X && w2.A.Y == w2.B.Y && Between(w1.A.X, w2.A.X, w2.B.X) && Between(w2.A.Y, w1.A.Y, w1.B.Y))
            {
                return new [] { new Point(w1.A.X, w2.A.Y) };
            }
            else
            if (w2.A.X == w2.B.X && w1.A.Y == w1.B.Y && Between(w2.A.X, w1.A.X, w1.B.X) && Between(w1.A.Y, w2.A.Y, w2.B.Y))
            {
                return new [] { new Point(w2.A.X, w1.A.Y) };
            }
            else
            if (w1.A.X == w1.B.X && w2.A.X == w2.B.X && w1.A.X == w2.A.X)
            {
                // both lines have the same X coordinate.
                var start1 = Math.Min(w1.A.Y, w1.B.Y);
                var end1 = Math.Max(w1.A.Y, w1.B.Y);
                var start2 = Math.Min(w2.A.Y, w2.B.Y);
                var end2 = Math.Max(w2.A.Y, w2.B.Y);

                if (Between(start1, start2, end2))
                {
                    return Range(start1, Math.Min(end1, end2)).Select(y => new Point(w1.A.X, y));
                }
                else
                if (Between(end1, start2, end2))
                {
                    return Range(Math.Max(start1, start2), end1).Select(y => new Point(w1.A.X, y));
                }
            }
            else
            if (w1.A.Y == w1.B.Y && w2.A.Y == w2.B.Y && w1.A.Y == w2.A.Y)
            {
                // both lines have the same Y coordinate
                var start1 = Math.Min(w1.A.X, w1.B.X);
                var end1 = Math.Max(w1.A.X, w1.B.X);
                var start2 = Math.Min(w2.A.X, w2.B.X);
                var end2 = Math.Max(w2.A.X, w2.B.X);

                if (Between(start1, start2, end2))
                {
                    return Range(start1, Math.Min(end1, end2)).Select(x => new Point(x, w1.A.Y));
                }
                else
                if (Between(end1, start2, end2))
                {
                    return Range(Math.Max(start1, start2), end1).Select(x => new Point(x, w1.A.Y));
                }
            }

            return Enumerable.Empty<Point>();
        }

        private bool Between(int x1, int end1, int end2)
        {
            return x1 >= Math.Min(end1, end2) && x1 <= Math.Max(end1, end2);
        }

        public IEnumerable<Line> ParseInput(string[] input)
        {
            int lineNr = 0;
            foreach (var line in input)
            {
                Point current = CentralPoint;
                int distanceToCentralPoint = 0;
                foreach (var lineInstruction in line.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    (var wire, var newPoint) = ParseLine(current, lineInstruction, lineNr, distanceToCentralPoint);
                    distanceToCentralPoint += wire.Length;
                    current = newPoint;
                    yield return wire;
                }
                lineNr++;
            }
        }

        private (Line, Point) ParseLine(Point current, string lineInstruction, int lineNr, int distanceToCentralPoint)
        {
            var length = int.Parse(lineInstruction.Substring(1));
            Point newPoint;
            switch (lineInstruction[0])
            {
                case 'U':
                    newPoint = new Point(current.X, current.Y + length);
                    break;
                case 'D':
                    newPoint = new Point(current.X, current.Y - length);
                    break;
                case 'L':
                    newPoint = new Point(current.X - length, current.Y);
                    break;
                case 'R':
                    newPoint = new Point(current.X + length, current.Y);
                    break;
                default:
                    throw new Exception($"unknown direction {lineInstruction[0]}");
            }
            var newLine = new Line(current, newPoint, lineNr, distanceToCentralPoint);
            return (newLine, newPoint);
        }

        public struct Line
        {
            public Point A;
            public Point B;
            public int LineNr;
            public int DistanceAToCentral;

            public Line(Point a, Point b, int nr, int distanceAToCentral)
            {
                A = a;
                B = b;
                LineNr = nr;
                DistanceAToCentral = distanceAToCentral;
            }

            public Line(int ax, int ay, int bx, int by, int nr, int distanceAToCentral)
            {
                A = new Point(ax, ay);
                B = new Point(bx, by);
                LineNr = nr;
                DistanceAToCentral = distanceAToCentral;
            }

            public int Length => A.ManhattanDistance(B);

            public override string ToString()
            {
                return $"Line {LineNr} a=({A}) b=({B}) {DistanceAToCentral}";
            }
        }

        public struct Point
        {
            public int X;
            public int Y;

            public Point(Point a)
            {
                X = a.X;
                Y = a.Y;
            }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int ManhattanDistance(Point b)
            {
                return Math.Abs(X - b.X) + Math.Abs(Y - b.Y);
            }

            public override string ToString()
            {
                return $"Point x={X} y={Y}";
            }
        }

        public struct Intersection
        {
            public Line L1;
            public Line L2;

            public Point Point;

            public Intersection(Line l1, Line l2, Point intersection)
            {
                L1 = l1;
                L2 = l2;
                Point = intersection;
            }

            public int Distance => L1.DistanceAToCentral + L1.A.ManhattanDistance(Point) + L2.DistanceAToCentral + L2.A.ManhattanDistance(Point);
            public int ManhattanDistanceToCentral => Point.ManhattanDistance(CentralPoint);

            public override string ToString()
            {
                return $"Intersection {Point} {L1} {L2}";
            }
        }
    }
}
