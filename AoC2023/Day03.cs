using FluentAssertions;
using NUnit.Framework.Constraints;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Numerics;

namespace AoC2023;

public class Day03
{
    [Test(ExpectedResult = 525911)]
    public long Part1()
    {
        var lines = File.ReadLines("day03.input").ToArray();
        long sum = 0;
        bool addToSum = false;
        for(int y = 0; y < lines.Length; y++)
        {
            int val = 0;
            for (int x = 0; x < lines[y].Length; x++)
            {
                char c = lines[y][x];
                if (char.IsDigit(c))
                {
                    val = (val * 10) + (c - '0');
                    (var addts, _, _) = NextTo(Marker, lines, y, x);
                    addToSum |= addts;
                }
                if (val > 0 && (!char.IsDigit(c) || x == lines[y].Length-1))
                {
                    if (addToSum)
                    {
                        sum += val;
                        addToSum = false;
                    }
                    val = 0;
                }
            }
        }
        return sum;
    }

    [Test(ExpectedResult = 75805607)]
    public long Part2()
    {
        var lines = File.ReadLines("day03.input").ToArray();
        long sum = 0;
        bool addToSum = false;

        Dictionary<(int x, int y), int> gears = new Dictionary<(int x, int y), int>();
        for (int y = 0; y < lines.Length; y++)
        {
            int val = 0;
            int gearX = 0;
            int gearY = 0;
            for (int x = 0; x < lines[y].Length; x++)
            {
                char c = lines[y][x];
                if (char.IsDigit(c))
                {
                    val = (val * 10) + (c - '0');
                    (var addts, var gx, var gy) = NextTo(Gear, lines, y, x);
                    if (addts)
                    {
                        addToSum |= addts;
                        gearX = gx;
                        gearY = gy;
                    }
                }
                if (val > 0 && (!char.IsDigit(c) || x == lines[y].Length - 1))
                {
                    if (addToSum)
                    {
                        if (gears.ContainsKey((gearX, gearY)))
                        {
                            sum += val * gears[(gearX, gearY)];
                        }
                        else
                        {
                            gears.Add((gearX, gearY), val);
                        }
                        addToSum = false;
                        gearX = 0;
                        gearY = 0;
                    }
                    val = 0;
                }
            }
        }
        return sum;
    }

    private (bool, int, int) NextTo(Func<char, bool> pred, string[] lines, int y, int x)
    {
        if (y >= 1 && x >= 1 && pred(lines[y - 1][x - 1])) return (true, y-1, x-1);
        if (y >= 1 && pred(lines[y - 1][x])) return (true, y -1,x);
        if (y >= 1 && x < lines.Length - 1 && pred(lines[y - 1][x + 1])) return (true, y -1 , x+1);

        if (x >= 1 && pred(lines[y][x - 1])) return (true, y, x-1);
        if (x < lines[y].Length - 1 && pred(lines[y][x + 1])) return (true, y, x+1);

        if (y < lines.Length - 1 && x >= 1 && pred(lines[y + 1][x - 1])) return (true, y+1, x-1);
        if (y < lines.Length - 1 && pred(lines[y + 1][x])) return (true, y+1, x);
        if (y < lines.Length - 1 && x < lines.Length - 1 && pred(lines[y + 1][x + 1])) return (true, y+1, x+1);
        return (false,-10,-10);
    }

    private bool Gear(char c)
        => c == '*';

    private bool Marker(char c)
        => !char.IsDigit(c) && c != '.';

}