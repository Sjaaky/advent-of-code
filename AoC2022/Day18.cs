using FluentAssertions.Execution;
using System.Text.RegularExpressions;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;
using System.Diagnostics;

namespace AoC2022;

public class Day18
{
    [Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(18);
    }

    [SetUp]
    public void setup()
    {
        var lines = File.ReadAllLines("day18.input");
        r.Match("x");
    }

    Regex r = new Regex(@"([\-0-9]+),([\-0-9]+),([\-0-9]+)", RegexOptions.Compiled);
    [TestCase("day18.input", ExpectedResult = 5525990)]
    [TestCase("day18example1.input", ExpectedResult = 64)]
    public int Part1(string input)
    {
        var lines = File.ReadAllLines(input);
        int max = 30;
        var rocks = new List<Position3>();
        int[,,] rock3d = new int[max, max, max];
        foreach (var line in lines)
        {
            var m = r.Match(line);
            if (m.Success)
            {
                var rock = new Position3(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value));
                rock.Set(rock3d, 1);
                rocks.Add(rock);
            }
        }
        var surface = 0;
        foreach(var rock in rocks)
        {
            foreach (var direction in Direction3.All6)
            {
                var n = rock.Add(direction);
                if (!n.Within(rock3d) || n.Get(rock3d) == 0)
                {
                    surface++;
                }
            }
        }
        return surface;
    }
    [TestCase("day18.input", ExpectedResult = 2554)] // too low 2524
    [TestCase("day18example1.input", ExpectedResult = 58)]
    public int Part2(string input)
    {
        var lines = File.ReadAllLines(input);
        int max = 30;
        var rocks = new List<Position3>();
        int[,,] rock3d = new int[max, max, max];
        foreach (var line in lines)
        {
            var m = r.Match(line);
            if (m.Success)
            {
                var rock = new Position3(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value));
                rock.Set(rock3d, 1);
                rocks.Add(rock);
            }
        }
        MarkOuterarea(rock3d, new Position3(0, 0, 0));
        var surface = 0;
        foreach (var rock in rocks)
        {
            foreach (var direction in Direction3.All6)
            {
                var n = rock.Add(direction);
                if (!n.Within(rock3d))
                {
                    surface++;
                }
                else
                if (n.Get(rock3d) == 3)
                {
                    surface++;
                }
            }
        }
        return surface;
    }

    bool IsAirpocket(int[,,] rock, Position3 p)
    {
        Queue<Position3> q = new();
        q.Enqueue(p);
        HashSet<Position3> v = new();
        HashSet<Position3> tv = new();
        while (q.TryDequeue(out var c))
        {
            v.Add(c);
            if (c.Get(rock) == 0)
            {
                foreach (var dir in Direction3.All6)
                {
                    var n = c.Add(dir);
                    if (!n.Within(rock))
                    {
                        Console.WriteLine($"{p} is not an airpocket");
                        return false;
                    }
                    if (n.Get(rock) == 0 && !v.Contains(n) && !tv.Contains(n))
                    {
                        tv.Add(c);
                        q.Enqueue(n);
                    }
                }
            }
        }
        return true;
    }

    bool MarkAirpocket(int[,,] rock, Position3 p)
    {
        Queue<Position3> q = new();
        q.Enqueue(p);
        while (q.TryDequeue(out var c))
        {
            if (c.Get(rock) == 0)
            {
                foreach (var dir in Direction3.All6)
                {
                    var n = c.Add(dir);
                    if (n.Get(rock) == 0)
                    {
                        n.Set(rock, 2);
                    }
                }
            }
        }
        return true;
    }

    bool MarkOuterarea(int[,,] rock, Position3 p)
    {
        Queue<Position3> q = new();
        HashSet<Position3> v = new();
        HashSet<Position3> tv = new();

        q.Enqueue(p);
        while (q.TryDequeue(out var c))
        {
            v.Add(c);
            c.Set(rock, 3);
            foreach (var dir in Direction3.All6)
            {
                var n = c.Add(dir);
                if (n.Within(rock) && n.Get(rock) == 0 && !v.Contains(n) && !tv.Contains(n))
                {
                    tv.Add(n);
                    q.Enqueue(n);
                }
            }
        }
        return true;
    }

}
