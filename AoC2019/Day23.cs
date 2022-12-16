using LanguageExt;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static AoC2019Test.Util;

namespace AoC2019Test
{
    public class Day23
    {
        [Test]
        public void Part1()
        {
            var program = File.ReadAllLines("day23.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();


            IntCodeComputer[] p = Range(0, 49).Select(_ => new IntCodeComputer(program, true)).ToArray();
            List<bigint>[] inputs = Range(0, 49).Select(i => new List<bigint>() { i }).ToArray();
            int[] outptr = new int[50];

            var c = 0;

            while(true)
            {
                p[c].Execute(inputs[c], 1);
                if (!p[c].IsHalted && outptr[c] + 3 <= p[c].Output.Count)
                {
                    var address = p[c].Output[outptr[c]];
                    var X = p[c].Output[outptr[c] + 1];
                    var Y = p[c].Output[outptr[c] + 2];
                    outptr[c] += 3;
                    if (address == 255)
                    {
                        Console.WriteLine($"DONE {c} => {address} x={X} y={Y}");
                        break;
                    }
                    else
                    {
                        inputs[(int)address].Add(X);
                        inputs[(int)address].Add(Y);
                        Console.WriteLine($"{c} => {address} x={X} y={Y}");
                    }
                }
                c = (c +1)%50;
            }
        }

        [Test]
        public void Part2()
        {
            var program = File.ReadAllLines("day23.input")
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(bigint.Parse)
                .ToArray();


            IntCodeComputer[] p = Range(0, 49).Select(_ => new IntCodeComputer(program, true)).ToArray();
            List<bigint>[] inputs = Range(0, 49).Select(i => new List<bigint>() { i }).ToArray();
            int[] outptr = new int[50];

            var c = 0;

            bigint natX = 0;
            bigint natY = 0;
            bigint prevNatY = -1;
            while (true)
            {
                if (p.All(i => i.IsIdle))
                {
                    Console.WriteLine("All IDLE");
                    inputs[0].Add(natX);
                    inputs[0].Add(natY);
                    if (prevNatY == natY)
                    {
                        Console.WriteLine($"DONE {natY}");
                        break;
                    }
                    prevNatY = natY;
                    Console.WriteLine($"{c} 0 => x={natX} y={natY}");
                    c = 0;
                }
                p[c].Execute(inputs[c], 1);
                if (!p[c].IsHalted && outptr[c] + 3 <= p[c].Output.Count)
                {
                    var address = p[c].Output[outptr[c]];
                    var X = p[c].Output[outptr[c] + 1];
                    var Y = p[c].Output[outptr[c] + 2];
                    outptr[c] += 3;
                    if (address == 255)
                    {
                        natX = X;
                        natY = Y;
                    }
                    else
                    {
                        inputs[(int)address].Add(X);
                        inputs[(int)address].Add(Y);
                        Console.WriteLine($"{c} => {address} x={X} y={Y}");
                    }
                }
                c = (c + 1) % 50;
            }
        }
    }
}