
using System.Text.RegularExpressions;

namespace AoC2022;

public class Day05
{
    Regex moveRegex = new Regex(@"move (\d+) from (\d+) to (\d+)");
    
    [TestCase("day05.input", ExpectedResult = "VWLCWGSDQ")]
    [TestCase("day05example1.input", ExpectedResult = "CMZ")]
    public string Part1(string input)
    {
        var stack = new char[9, 100];
        var top = new int[9];
        var lines = File.ReadAllLines(input);
        var mode = 0;
        foreach (var line in lines)
        {
            if (line.Length >= 2 && line[1] == '1') mode = 1;
            if (mode == 0)
            {
                for (int i = 0; i < 9; i++)
                {
                    var idx = (i * 4) + 1;
                    if (line.Length > idx && line[idx] != ' ')
                    {
                        stack[i, top[i]++] = line[idx];
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(line))
            {
                PrintStack(stack, top);
                ReverseStack(stack, top);
                PrintStack(stack, top);
            }
            if (mode == 1 && line.StartsWith("move"))
            {
                Console.WriteLine(line);
                var m = moveRegex.Match(line);
                if (m.Success)
                {
                    var g = m.Groups;
                    var ints = g.Values.Skip(1).Select(i => int.Parse(i.Value)).ToArray();
                    Move(stack, top, ints[0], ints[1]-1, ints[2]-1);
                }
                PrintStack(stack, top);
            }
        }
        PrintStack(stack, top);
        var topchars = Top(stack, top);
        return topchars;
    }

    [TestCase("day05.input", ExpectedResult = "TCGLQSLPW")]
    [TestCase("day05example1.input", ExpectedResult = "MCD")]
    public string Part2(string input)
    {
        var stack = new char[9, 100];
        var top = new int[9];
        var lines = File.ReadAllLines(input);
        var mode = 0;
        foreach (var line in lines)
        {
            if (line.Length >= 2 && line[1] == '1') mode = 1;
            if (mode == 0)
            {
                for (int i = 0; i < 9; i++)
                {
                    var idx = (i * 4) + 1;
                    if (line.Length > idx && line[idx] != ' ')
                    {
                        stack[i, top[i]++] = line[idx];
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(line))
            {
                PrintStack(stack, top);
                ReverseStack(stack, top);
                PrintStack(stack, top);
            }
            if (mode == 1 && line.StartsWith("move"))
            {
                Console.WriteLine(line);
                var m = moveRegex.Match(line);
                if (m.Success)
                {
                    var g = m.Groups;
                    var ints = g.Values.Skip(1).Select(i => int.Parse(i.Value)).ToArray();
                    Move2(stack, top, ints[0], ints[1] - 1, ints[2] - 1);
                }
                PrintStack(stack, top);
            }
        }
        PrintStack(stack, top);
        var topchars = Top(stack, top);
        return topchars;
    }

    private void Move(char[,] stack, int[] top, int cnt, int src, int dst)
    {
        for (int i = 0; i < cnt; i++)
        {
            stack[dst, top[dst]++] = stack[src, --top[src]];
            stack[src, top[src]] = '*'; // we should never see this in prints
        }
    }

    private void Move2(char[,] stack, int[] top, int cnt, int src, int dst)
    {
        for (int i = 0; i < cnt; i++)
        {
            stack[dst, top[dst]+i] = stack[src, top[src]-cnt+i];
            stack[src, top[src]-cnt+i] = '*'; // we should never see this in prints
        }
        top[src] -= cnt;
        top[dst] += cnt;
    }

    private void ReverseStack(char[,] stack, int[] top)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < top[i] / 2; j++)
            {
                var x = stack[i, j];
                var y = stack[i, top[i] - j - 1];
                stack[i, j] = y;
                stack[i, top[i] - j - 1] = x;
            }
            Console.WriteLine();
        }
    }

    private void PrintStack(char[,] stack, int[] top)
    {
        for (int i = 0; i < 9; i++)
        {
            Console.Write($"{i+1}:");
            for (int j = 0; j < top[i]; j++)
            {
                Console.Write($" {stack[i,j]}");
            }
            Console.WriteLine();
        }
    }

    private string Top(char[,] stack, int[] top)
     => string.Join("", top.TakeWhile(t => t > 0).Select((t, i) => stack[i, t - 1]));
}


