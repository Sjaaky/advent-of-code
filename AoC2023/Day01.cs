using FluentAssertions;
using NUnit.Framework.Constraints;
using System.Diagnostics;

namespace AoC2023;

public class Day01
{
    [Test(ExpectedResult = 56108)]
    public int Part1()
    {
        var nrs = File
            .ReadLines("day01.input")
            .Select(l => (l.First(l => char.IsDigit(l)) - '0') * 10 + (l.Last(l => char.IsDigit(l)) - '0'));

        var sumcal = nrs.Sum();
        return sumcal;
    }

    [Test(ExpectedResult = 55652)]
    public int Part2()
    {
        var nrs = File
            .ReadLines("day01.input")
            .Select(l => {
                var val = GetFirstVal(l) * 10 + GetLastVal(l);
                return val;
            });

        var sumcal = nrs.Sum();
        return sumcal;
    }

    List<(string key, int val)> lookup = [("one", 1), ("two", 2), ("three", 3), ("four", 4), ("five", 5), ("six", 6), ("seven", 7), ("eight", 8), ("nine", 9)];

    private int GetFirstVal(string l)
    {
        var las = l.AsSpan();
        for (int i = 0; i < las.Length; i++)
        {
            if (char.IsDigit(las[i]))
            {
                return las[i] - '0';
            }
            foreach (var k in lookup)
            {
                if (las[i..].StartsWith(k.key))
                {
                    return k.val;
                }
            }
        }
        throw new Exception("nothing found");
    }

    private int GetLastVal(string l)
    {
        var las = l.AsSpan();
        for (int i = las.Length-1; i>=0; i--)
        {
            if (char.IsDigit(las[i]))
            {
                return las[i] - '0';
            }
            foreach (var k in lookup)
            {
                if (las[i..].StartsWith(k.key))
                {
                    return k.val;
                }
            }
        }
        throw new Exception("nothing found");
    }
}