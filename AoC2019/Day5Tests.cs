using LanguageExt;
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
    public class Day5Tests
    {
        [Test]
        public void test1()
        {

            var d = new IntCodeComputer(new[] { 3, 0, 4, 0, 99 });

            d.Execute(new List<bigint> { 4 });
            Console.WriteLine($"output = {string.Join(',', d.Output)}");
        }


        [Test]
        public void TestGetArg()
        {
            var d = new IntCodeComputer(new[] { 1, 2, 3, 4 });
            Assert.AreEqual(2, (int)d.GetArgImmediate(1));
            Assert.AreEqual(3, (int)d.GetArg(1));
        }

        [Test]
        public void TestAdd()
        {
            var d = new IntCodeComputer(new[] { 1101, 2, 3, 0 });
            d.Add();
            Console.WriteLine(string.Join(",", d.Memory));
            Assert.AreEqual(5, (int)d.Memory[0]);
        }

        [Test]
        public void TestProgram1()
        {
            var d = new IntCodeComputer(new[] { 3, 9, 8, 9, 10, 9, 4, 9, 99, -1, 8 });

            d.Execute(new List<bigint> { 8 });
            Console.WriteLine($"output = {string.Join(',', d.Output)}");
            Assert.AreEqual(1, (int)d.Output[0]);
        }

        [Test]
        public void TestProgram2()
        {
            var d = new IntCodeComputer(new[] { 3, 9, 8, 9, 10, 9, 4, 9, 99, -1, 8 });
            d.Execute(new List<bigint> { 4 });
            Console.WriteLine($"output = {string.Join(',', d.Output)}");
            Assert.AreEqual(0, (int)d.Output[0]);
        }
    }
}
