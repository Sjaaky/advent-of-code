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
    public class Day2
    {
        [Test]
        public void Part1()
        {
            var program = File.ReadAllLines("day2.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(int.Parse)
                .ToArray();
            var computer = new IntCodeComputer(program);
            computer.SetNounAndVerb(12, 2);
            var result = computer.Execute();

            Console.WriteLine(result);
            Assert.AreEqual(3654868, result);
        }

        [Test]
        public void Part2()
        {
            var program = File.ReadAllLines("day2.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(int.Parse)
                .ToArray();

            int noun = 0;
            int verb = 0;
            bool done = false;
            for(noun = 0; noun < 99; noun++)
            {
                for (verb = 0; verb < 99; verb++)
                {
                    var computer = new IntCodeComputer(program);
                    computer.SetNounAndVerb(noun, verb);
                    if (computer.Execute() == 19690720)
                    {
                        done = true;
                    }
                    if (done) break;
                }
                if (done) break;
            }

            Assert.IsTrue(done);
            var result = noun * 100 + verb;
            Console.WriteLine(result);
            Assert.AreEqual(7014, result);
        }
    }
}
