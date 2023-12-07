using FluentAssertions;
using NUnit.Framework.Constraints;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
using static System.Formats.Asn1.AsnWriter;

namespace AoC2023;

public class Day04
{
    [Test(ExpectedResult = 25183)]
    public long Part1()
    {
        var lines = File.ReadLines("day04.input").ToArray();
        long totalscore = 0;
        foreach (var line in lines)
        {
            int score = 0;
            int np = 4;
            (int card, np) = ReadInt(line, np);
            np++;
            var win = new HashSet<int>();
            while (line[np] != '|')
            {
                (int nr, np) = ReadInt(line, np);
                np++;
                Console.WriteLine(nr);
                win.Add(nr);
            }
            np += 2;
            while (np < line.Length)
            {
                (int nr, np) = ReadInt(line, np);
                Console.WriteLine(nr);
                if (win.Contains(nr))
                {
                    if (score == 0)
                    {
                        score = 1;
                    }
                    else
                    {
                        score *= 2;
                    }
                }
            }
            totalscore += score;
        }

        return totalscore;
    }

    [Test]
    [TestCase("day04ex1.input", ExpectedResult = 30)]
    [TestCase("day04.input", ExpectedResult = 5667240)]
    public long Part2(string inp)
    {
        var lines = File.ReadLines(inp).ToArray();
        int[] nrs = new int[lines.Length+1];
        for (int i = 1; i < nrs.Length; i++) nrs[i] = 1;
        foreach (var line in lines)
        {
            int score = 0;
            int np = 4;
            (int card, np) = ReadInt(line, np);
            np++;
            var win = new HashSet<int>();
            while (line[np] != '|')
            {
                (int nr, np) = ReadInt(line, np);
                np++;
                win.Add(nr);
            }
            np += 2;
            while (np < line.Length)
            {
                (int nr, np) = ReadInt(line, np);
                if (win.Contains(nr))
                {
                    score++;
                }
            }
            for (int i = 1; i <= score; i++)
            {
                nrs[card + i] += nrs[card];
            }
        }

        return nrs.Sum();
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