using FluentAssertions;
using NUnit.Framework.Constraints;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
using static System.Formats.Asn1.AsnWriter;

namespace AoC2023;

public class Day06
{
    [Test(ExpectedResult = 840336)]
    public long Part1()
    {
        var lines = File.ReadLines("day06.input").ToArray();

        var times = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(int.Parse).ToArray();
        var records = lines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(int.Parse).ToArray();

        int mul = 1;
        for (int g = 0; g < times.Length; g++)
        {
            int sum = 0;
            for (int hold = 0; hold < times[g]; hold++)
            {
                var dist = (times[g] - hold) * hold;
                if (dist > records[g])
                {
                    sum++;
                }
            }
            mul *= sum;
        }

        return mul;
    }

    [Test(ExpectedResult = 41382569)]
    public long Part2()
    {
        var lines = File.ReadLines("day06.input").ToArray();

        var times = long.Parse(string.Join("", lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1)));
        var records = long.Parse(string.Join("", lines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1)));

        
        long sum = 0;
        for (int hold = 0; hold < times; hold++)
        {
            var dist = (times - hold) * hold;
            if (dist > records)
            {
                sum++;
            }
        }

        return sum;
    }

    private (int, int) ReadInt(string line, int p)
    {
        int nr = 0;
        int c = p;
        while (c < line.Length && line[c] == ' ') { c++; }
        while (c < line.Length && char.IsDigit(line[c]))
        {
            nr = (nr * 10) + (line[c] - '0');
            c++;
        }
        return (nr, c);
    }

}