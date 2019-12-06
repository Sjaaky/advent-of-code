using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AoC2019Test.Day3;

namespace AoC2019Test
{
    public class Day3Tests
    {
        int distToA = 0;
        [Test]
        public void TestIntersect1()
        {
            var d = new Day3();
            var x = d.Intersections(new Line(2, 3, 6, 3, 0, distToA), new Line(3, 5, 3, 2, 1, distToA));

            Assert.AreEqual(x.Single(), new Point(3, 3));
        }

        [Test]
        public void TestIntersect2()
        {
            var d = new Day3();
            var x = d.Intersections(new Line(0, 0, -10, 0, 0, distToA), new Line(0, 0, 10, 0, 1, distToA));

            Assert.AreEqual(x.Single(), new Point(0, 0));
        }

        [Test]
        public void TestIntersect3()
        {
            var d = new Day3();
            var x = d.Intersections(new Line(0, 0, 0, -10, 0, distToA), new Line(0, 0, 0, 10, 1, distToA));

            Assert.AreEqual(x.Single(), new Point(0, 0));
        }

        [Test]
        public void TestIntersect4()
        {
            var d = new Day3();
            var x = d.Intersections(new Line(0, 1, 0, -1000, 0, distToA), new Line(0, 0, 0, 1000, 1, distToA));

            Assert.AreEqual(x.Count(), 2);
            Assert.AreEqual(x.First(), new Point(0, 0));
            Assert.AreEqual(x.Skip(1).First(), new Point(0, 1));
        }

        [Test]
        public void TestIntersect5()
        {
            var d = new Day3();
            var x = d.Intersections(new Line(0, 0, 0, 1000, 0, distToA), new Line(0, 0, 0, -1000, 1, distToA));

            Assert.AreEqual(x.Single(), new Point(0, 0));
        }

        [Test]
        public void TestIntersect6()
        {
            var d = new Day3();
            var x = d.Intersections(new Line(-1000, 0, 0, 0, 0, distToA), new Line(1000, 0, 0, 0, 1, distToA));

            Assert.AreEqual(x.Single(), new Point(0, 0));
        }

        [Test]
        public void TestIntersect7()
        {
            var d = new Day3();
            var x = d.Intersections(new Line(1000, 0, 0, 0, 0, distToA), new Line(-1000, 0, 0, 0, 1, distToA));

            Assert.AreEqual(x.Single(), new Point(0, 0));
        }

        [Test]
        public void TestIntersect8()
        {
            var d = new Day3();
            var x = d.Intersections(new Line(0, 0, 1000, 0, 0, distToA), new Line(-1004, 0, 0, 0, 1, distToA));

            Assert.AreEqual(x.Single(), new Point(0, 0));
        }
    }
}
