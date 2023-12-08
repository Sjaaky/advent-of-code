using FluentAssertions;
using NUnit.Framework.Constraints;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography.X509Certificates;
using static System.Formats.Asn1.AsnWriter;

namespace AoC2023;

public class Day08
{
    record node(string name, string[] to);

    [Test]
    [TestCase("day08.input", ExpectedResult = 15989)]
    public long Part1(string input)
    {
        var lines = File.ReadLines(input).ToArray();

        var instr = lines[0];
        Dictionary<string, node> gr = new();
        foreach (var line in lines.Skip(2))
        {
            string[] to = [line[7..10], line[12..15]];
            var name = line[0..3];
            gr.Add(name, new node(name, to));
        }

        var node = gr["AAA"];
        var ip = 0;
        long steps = 0;
        while (node.name != "ZZZ")
        {
            var inst = instr[(ip++ % instr.Length)];
            node = gr[node.to[(inst == 'L') ? 0 : 1]];
            steps++;
        }
        return steps;
    }

    [Test]
    [TestCase("day08.input", ExpectedResult = 13830919117339)]
    public long Part2(string input)
    {
        var lines = File.ReadLines(input).ToArray();

        var instr = lines[0];
        Dictionary<string, node> gr = new();
        foreach (var line in lines.Skip(2))
        {
            string[] to = [line[7..10], line[12..15]];
            var name = line[0..3];
            gr.Add(name, new node(name, to));
        }

        List<long> gsteps = new();
        foreach (var gn in gr.Values.Where(v => v.name.EndsWith('A')))
        {
            var node = gn;
            var ip = 0;
            long steps = 0;
            while (!node.name.EndsWith("Z"))
            {
                var inst = instr[(ip++ % instr.Length)];
                node = gr[node.to[(inst == 'L') ? 0 : 1]];
                steps++;
            }
            gsteps.Add(steps);
        }
        long lcm = 1;
        foreach (var step in gsteps)
        {
            lcm = LCM(lcm, step);
        }
        return lcm;
    }

    long LCM(long a, long b)
    {
        return (a / GCD(a, b)) * b;
    }

    long GCD(long a, long b)
    {
        while (b != 0)
        {
            var t = b;
            b = a % b;
            a = t;
        }
        return a;
    }
}