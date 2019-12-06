using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static AoC2019Test.Util;

namespace AoC2019Test
{
    public class Day4
    {
        [Test]
        public void Part1()
        {
            var start = 231832;
            var end = 767346;

            var part1 = Range(start, end).Count(i => Check(i, digits => digits.Any(c => c >= 2)));
            Console.WriteLine(part1);
            Assert.AreEqual(1330, part1);
        }

        [Test]
        public void Part2()
        {
            var start = 231832;
            var end = 767346;

            var part2 = Range(start, end).Count(i => Check(i, digits => digits.Any(c => c == 2)));
            Console.WriteLine(part2);
            Assert.AreEqual(876, part2);
        }

        private bool Check(int i, Func<int[], bool> OccurrenceCheck)
        {
            int[] occ = new int[10];
            char x = ' ';
            foreach (char c in i.ToString())
            {
                if (c < x) return false;
                occ[c - '0']++;
                x = c;
            }
            return OccurrenceCheck(occ);
        }
    }
}
