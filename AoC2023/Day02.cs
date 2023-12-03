using FluentAssertions;
using NUnit.Framework.Constraints;
using System.Diagnostics;
using System.Drawing;
using System.Linq.Expressions;
using System.Numerics;

namespace AoC2023;

public class Day02
{
    [Test(ExpectedResult = 2683)]
    public int Part1()
    {
        var lines = File.ReadLines("day02.input");
        int sum = 0;
        var bag = new Dictionary<string, int> { { "green", 13 }, { "red", 12 }, { "blue", 14 } };
        foreach (var line in lines)
        {
            (int game, int np) = ReadInt(line, 5);
            var grabfit = true;
            while (np < line.Length)
            {
                var grab = new Dictionary<string, int>();
                while (np < line.Length && line[np] != ';')
                {
                    np++;
                    (int n, np) = ReadInt(line, np);
                    (string color, np) = ReadString(line, np);
                    grab.Add(color, n);
                }
                foreach (var key in grab.Keys)
                {
                    if (grab.ContainsKey(key) && grab[key] > bag[key])
                    {
                        grabfit = false;
                    }
                }
                np++;
            }
            if (grabfit) sum += game;
        }
        return sum;
    }

    [Test(ExpectedResult = 49710)]
    public int Part2()
    {
        var lines = File.ReadLines("day02.input");
        int sum = 0;
        foreach (var line in lines)
        {
            (int game, int np) = ReadInt(line, 5);

            var grab = new Dictionary<string, int>();
            while (np < line.Length)
            {
                np++;
                (int n, np) = ReadInt(line, np);
                (string color, np) = ReadString(line, np);
                if (grab.ContainsKey(color))
                {
                    if (grab[color] < n)
                    {
                        grab[color] = n;
                    }
                }
                else
                {
                    grab.Add(color, n);
                }
            }
            np++;

            sum += grab["green"] * grab["red"] * grab["blue"];
        }
        return sum;
    }

    private (int, int) ReadInt(string line, int p)
    {
        int nr = 0;
        int c = p;
        while(c < line.Length && line[c] == ' ') { c++; }
        while(c < line.Length && char.IsDigit(line[c]))
        {
            nr = (nr * 10) + (line[c] - '0');
            c++;
        }
        return (nr, c);
    }

    private (string, int) ReadString(string line, int p)
    {
        int c = p;
        while (c < line.Length && line[c] == ' ') { c++; }
        int start = c;
        while (c < line.Length && char.IsLetter(line[c])) { c++;  }
        return (line.Substring(start, c - start), c);
    }
}