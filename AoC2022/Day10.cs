using System.Configuration;

namespace AoC2022;

public class Day10
{
    //[Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(10);
    }

    [TestCase("day10example1.input", ExpectedResult = 13140)]
    [TestCase("day10.input", ExpectedResult = 14160)]
    public long Part1and2(string input)
    {
        var lines = File.ReadAllLines(input);

        var clock = 0;
        var X = 1;
        var checkAtCycle = 20;
        var sum = 0;
        foreach (var line in lines)
        {
            if (line[0..4] == "addx")
            {
                var nr = int.Parse(line[5..]);
                sum = ClockTick(sum, ref clock, ref checkAtCycle, X);
                sum = ClockTick(sum, ref clock, ref checkAtCycle, X);
                X += nr;

            }
            else if (line[0..4] == "noop")
            {
                sum = ClockTick(sum, ref clock, ref checkAtCycle, X);
            }
        }

        return sum;
    }

    public int ClockTick(int sum, ref int clock, ref int checkAtCycle, int x)
    {
        if ((clock % 40) >= x - 1 && (clock % 40) <= x + 1)
        {
            Console.Write("#");
        }
        else
        {
            Console.Write(".");
        }
        clock++;
        if (clock % 40 == 0) Console.WriteLine();
        if (clock == checkAtCycle)
        {
            checkAtCycle += 40;
            var inc = (clock * x);
            var val = sum + inc;
            return val;
        }
        return sum;
    }
}

