using FluentAssertions;
using NUnit.Framework.Constraints;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
using static System.Formats.Asn1.AsnWriter;

namespace AoC2023;

public class Day05
{
    class Mapping(string From, string To)
    {
        public void AddMap(long dest, long src, long length)
        {
            ordered = false;
            maps.Add(new map(dest, src, length));
        }

        bool ordered = false;
        //public IEnumerable<(long, long)> Breaks(long input, long length)
        //{
        //    if (!ordered)
        //    {
        //        maps = maps.OrderBy(m => m.src).ToList();
        //        ordered = true;
        //    }
        //    long start = input;
        //    long end = input + length;

        //    foreach (var map in maps)
        //    {
        //        if (start <= map.src && end > map.src + map.length)
        //        {
        //            yield return (map.dest, length + (map.src - start));
        //        }
        //    }
        //    return input;
        //}

        public long Map(long input)
        {
            foreach (var map in maps)
            {
                if (input >= map.src && input < map.src + map.length)
                {
                    return map.dest + input - map.src;
                }
            }
            return input;
        }

        List<map> maps = new List<map>();

        record map(long dest, long src, long length);


    }

    [Test]
    [TestCase("day05.input", ExpectedResult = 993500720)]
    [TestCase("day05ex.input", ExpectedResult = 35)]
    public long Part1(string input)
    {
        var lines = File.ReadLines(input).ToArray();
        var seeds = lines[0][6..].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

        List<Mapping> mappings = new();
        Mapping mapping = null;
        foreach (var line in lines.Skip(1))
        {
            if (line.Length == 0) { }
            else if (char.IsLetter(line[0]))
            {
                var fromto = line.Split(' ')[0].Split("-to-");
                //if (mapping != null) mapping.Order();
                mapping = new Mapping(fromto[0], fromto[1]);
                mappings.Add(mapping);
            }
            else if (char.IsDigit(line[0]))
            {
                var abc = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
                mapping.AddMap(abc[0], abc[1], abc[2]);
            }
        }
        long lowest = long.MaxValue;
        foreach (var seed in seeds)
        {
            var s = seed;
            Console.WriteLine($"start {s}");
            foreach (var map in mappings)
            {
                s = map.Map(s);
                Console.WriteLine($"int {s}");
            }
            Console.WriteLine($"{seed} => {s}");
            if (s < lowest) lowest = s;
        }
        return lowest;
    }

    [Test]
    [TestCase("day05.input", ExpectedResult = 25183)]
    [TestCase("day05ex.input", ExpectedResult = 25183)]
    public long Part2(string input)
    {
        var lines = File.ReadLines(input).ToArray();
        var seeds = lines[0][6..].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

        List<Mapping> mappings = new();
        Mapping mapping = null;
        foreach (var line in lines.Skip(1))
        {
            if (line.Length == 0) { }
            else if (char.IsLetter(line[0]))
            {
                var fromto = line.Split(' ')[0].Split("-to-");
                mapping = new Mapping(fromto[0], fromto[1]);
                mappings.Add(mapping);
            }
            else if (char.IsDigit(line[0]))
            {
                var abc = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
                mapping.AddMap(abc[0], abc[1], abc[2]);
            }
        }

        long lowest = long.MaxValue;
        foreach (var seed in seeds)
        {
            var s = seed;
            foreach (var map in mappings)
            {
                s = map.Map(s);
            }
            if (s < lowest) lowest = s;
        }
        return lowest;
    }
}