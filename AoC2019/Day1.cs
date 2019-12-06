using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2019Test
{
    public class Day1
    {
        [Test]
        public void Part1()
        {
            var input = File.ReadAllLines("day1.input");
            var totalFuel = input.Select(int.Parse).Sum(mass => mass / 3 - 2);

            Console.WriteLine(totalFuel);
            Assert.AreEqual(3279287, totalFuel);
        }

        [Test]
        public void Part2()
        {
            var input = File.ReadAllLines("day1.input");
            var totalFuel = input.Select(int.Parse).Sum(mass => CalcFuel(mass));

            Console.WriteLine(totalFuel);
            Assert.AreEqual(4916076, totalFuel);
        }

        private static int CalcFuel(int mass)
        {
            var fuel = 0;
            while (mass > 0)
            {
                mass = mass / 3 - 2;
                if (mass <= 0) break;
                fuel += mass;
            }
            return fuel;
        }
    }
}
