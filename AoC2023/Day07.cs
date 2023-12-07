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

public class Day07
{
    [Test]
    [TestCase("day07.input", ExpectedResult = 241344943)]
    [TestCase("day07ex1.input", ExpectedResult = 6440)]
    public long Part1(string input)
    {
        var lines = File.ReadLines(input).ToArray();

        List<(string hand, long bid)> hands = new();
        foreach (var line in lines)
        {
            var lp = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();
            hands.Add((lp[0], int.Parse(lp[1])));
        }

        var result = hands.OrderBy(k => HandValue(k.hand));
        return result.Select((v, i) => v.bid * (i + 1)).Sum();

        long HandValue(string hand)
        {
            var hist = new int[16];
            foreach (var c in hand)
            {
                hist[CardValue(c)]++;
            }
            var max = hist.Max();
            var type = max switch
            {
                5 => 8_000_000_000L, // poker
                4 => 6_000_000_000L, // 4 of a kind
                3 => hist.Skip(1).Contains(2) ? 5_000_000_000L : 4_000_000_000L, // full house : 3 of a kind
                2 => hist.Skip(1).Count(c => c == 2) == 2 ? 3_000_000_000L : 2_000_000_000L, // 2 pair : 2 of a kind
                _ => 0L
            };

            var val = hand.Aggregate(0, (acc, c) => (acc * 16) + CardValue(c));
            return type + val;
        }

        int CardValue(char c)
            => "_23456789TJQKA".IndexOf(c);
    }

    [Test]
    [TestCase("day07.input", ExpectedResult = 243101568)]
    [TestCase("day07ex1.input", ExpectedResult = 5905)]
    public long Part2(string input)
    {
        var lines = File.ReadLines(input).ToArray();

        List<(string hand, long bid)> hands = new();
        foreach (var line in lines)
        {
            var lp = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();
            hands.Add((lp[0], int.Parse(lp[1])));
        }

        var result = hands.OrderBy(k => HandValue(k.hand));
        return result.Select((v, i) => v.bid * (i + 1)).Sum();

        long HandValue(string hand)
        {
            var hist = new long[16];
            foreach (var c in hand)
            {
                hist[CardValue(c)]++;
            }
            var max = hist.Skip(1).Max(); // don't consider 'J'
            var type = max switch
            {
                5 => 8_000_000_000L, // poker
                4 => 6_000_000_000L, // 4 of a kind
                3 => hist.Skip(1).Contains(2) ? 5_000_000_000L : 4_000_000_000L, // full house : 3 of a kind
                2 => hist.Skip(1).Count(c => c == 2) == 2 ? 3_000_000_000L : 2_000_000_000L, // 2 pair : 2 of a kind
                _ => 0L
            };
            if (hist[0] == 5) // joker poker
            {
                type = 8_000_000_000L;
            }
            else
            {
                type += hist[0] * 2_000_000_000L;
            }

            return type + hand.Aggregate(0L, (acc, c) => (acc * 16L) + CardValue(c));
        }

        int CardValue(char c)
            => "J23456789TQKA".IndexOf(c);
    }

    void DebugPrint(IEnumerable<(string hand, long bid)> result, Func<string, long> handValue)
    {
        foreach (var r in result.Select((h, i) => (h, i)))
        {
            Console.WriteLine($"{r.h.hand} {r.h.bid}*{r.i + 1}= {r.h.bid * (r.i + 1)} {handValue(r.h.hand)}");
        }
    }
}