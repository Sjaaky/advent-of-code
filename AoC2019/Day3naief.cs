using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AoC2019Test.Util;

namespace AoC2019Test
{
    public class Day3naief
    {
        private static Point CentralPoint = new Point(0, 0);

        //[Test]
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

        //[Test]
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
            foreach (var w1 in wires.Where(w => w.LineNr == 0))
            {
                foreach (var w2 in wires.Where(w => w.LineNr == 1))
                {
                    var intersections = w1.PointsOnLine().Intersect(w2.PointsOnLine()).Select(p => new Intersection(w1, w2, p));
                    foreach (var intersection in intersections)
                    {
                        yield return intersection;
                    }
                }
            }
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

            public IEnumerable<Point> PointsOnLine()
            {
                var self = this;
                if (A.X == B.X)
                {
                    return Range(A.Y, B.Y).Select(y => new Point(self.A.X, y));
                }
                else
                {
                    return Range(A.X, B.X).Select(x => new Point(x, self.A.Y));
                }
            }

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
