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

public class Day21
{
    [Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(21);
    }

    [SetUp]
    public void setup()
    {
        var day = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name.ToLower();
        var filename = $"{day}.input";
        if (File.Exists(filename))
        {
            var lines = File.ReadAllLines(filename);
        }
        r.Match("x");
    }

    Regex r = new Regex(@"(?<monkey>\w{4}): ((?<math>(?<m1>\w{4}) (?<op>[*/+-]) (?<m2>\w{4}))|(?<number>\d+))", RegexOptions.Compiled);
    [TestCase("day21.input", ExpectedResult = 38914458159166)]
    [TestCase("day21example1.input", ExpectedResult = 152)]
    public long Part1(string input)
    {
        var monkeys = new Dictionary<string, string>();
        var lines = File.ReadAllLines(input).ToArray();
        foreach (var line in lines)
        {
            var m = r.Match(line);
            if (m.Success)
            {
                var monkey = m.Groups["monkey"].Value;
                monkeys.Add(monkey, line);
            }
            else
            {
                throw new Exception(line);
            }
        }

        return Calc("root", monkeys);
    }

    [TestCase("day21.input", ExpectedResult = 3665520865940)] //4499955 too low
    [TestCase("day21example1.input", ExpectedResult = 301)]
    public long Part2(string input)
    {
        var monkeys = new Dictionary<string, string>();
        var lines = File.ReadAllLines(input).ToArray();
        foreach (var line in lines)
        {
            var m = r.Match(line);
            if (m.Success)
            {
                var monkey = m.Groups["monkey"].Value;
                monkeys.Add(monkey, line);
            }
            else
            {
                throw new Exception(line);
            }
        }

        var i = CalcToHuman("root", monkeys, null, 0, new());
        return i.Value;
    }

    long Calc(string monkey, Dictionary<string, string> monkeys)
    {
        var line = monkeys[monkey];
        var m = r.Match(line);
        if (m.Groups["math"].Success)
        {
            var m1 = m.Groups["m1"].Value;
            var m2 = m.Groups["m2"].Value;
            var op = m.Groups["op"].Value;
            var nr = op switch
            {
                "*" => Calc(m1, monkeys) * Calc(m2, monkeys),
                "/" => Calc(m1, monkeys) / Calc(m2, monkeys),
                "+" => Calc(m1, monkeys) + Calc(m2, monkeys),
                "-" => Calc(m1, monkeys) - Calc(m2, monkeys),
            };
            return nr;
        }
        else if (m.Groups["number"].Success)
        {
            return long.Parse(m.Groups["number"].Value);
        }
        else
        {
            throw new Exception("help");
        }
    }

    long? CalcToHuman(string monkey, Dictionary<string, string> monkeys, long? goal, int recl, Dictionary<string, long> val)
    {
        if (monkey == "humn")
        {
            return goal;
        }
        if (val.ContainsKey(monkey))
        {
            return val[monkey];
        }
        var line = monkeys[monkey];
        var m = r.Match(line);
        if (m.Groups["math"].Success)
        {
            var m1 = m.Groups["m1"].Value;
            var m2 = m.Groups["m2"].Value;
            var op = m.Groups["op"].Value;

            var a1 = CalcToHuman(m1, monkeys, null, recl + 1, val);
            var a2 = CalcToHuman(m2, monkeys, null, recl + 1, val);
            if (monkey == "root") op = "=";

            if (a1.HasValue && a2.HasValue)
            {
                var nr = op switch
                {
                    "*" => a1 * a2,
                    "/" => a1 / a2,
                    "+" => a1 + a2,
                    "-" => a1 - a2,
                };
                if (nr.HasValue)
                {
                    val.Add(monkey, nr.Value);
                }
                return nr;
            }
            else
            if (a1.HasValue)
            {
                if (goal == null && monkey != "root") return null;
                var nr = op switch
                {
                    "=" => CalcToHuman(m2, monkeys, a1, recl + 1, val),
                    "*" => CalcToHuman(m2, monkeys, goal / a1, recl + 1, val),
                    "/" => CalcToHuman(m2, monkeys, a1 / goal, recl + 1, val),
                    "+" => CalcToHuman(m2, monkeys, goal - a1, recl + 1, val),
                    "-" => CalcToHuman(m2, monkeys, a1 - goal, recl + 1, val),
                };
                if (nr.HasValue)
                {
                    val.Add(monkey, nr.Value);
                }
                return nr;
            }
            else //if (a2.HasValue)
            {
                if (goal == null && monkey != "root") return null;
                var nr = op switch
                {
                    "=" => CalcToHuman(m1, monkeys, a2, recl + 1, val),
                    "*" => CalcToHuman(m1, monkeys, goal / a2, recl + 1, val),
                    "/" => CalcToHuman(m1, monkeys, goal * a2, recl + 1, val),
                    "+" => CalcToHuman(m1, monkeys, goal - a2, recl + 1, val),
                    "-" => CalcToHuman(m1, monkeys, goal + a2, recl + 1, val),
                };
                if (nr.HasValue)
                {
                    val.Add(monkey, nr.Value);
                }
                return nr;
            }
        }
        else if (m.Groups["number"].Success)
        {
            return long.Parse(m.Groups["number"].Value);
        }
        else
        {
            throw new Exception("help");
        }
    }

}
