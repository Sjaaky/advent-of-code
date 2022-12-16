
using System.Text.RegularExpressions;

namespace AoC2022;

public class Day04
{
    Regex pairRegex = new Regex(@"(\d+)-(\d+),(\d+)-(\d+)");
    [TestCase("day04.input", ExpectedResult = 515)]
    public int Part1(string input)
    {
        var score = 0;
        var lines = File
            .ReadAllLines(input);
        foreach (var line in lines)
        {
            var m = pairRegex.Match(line);
            if (m.Success)
            {
                var g = m.Groups;
                var ints = g.Values.Skip(1).Select(i => int.Parse(i.Value)).ToArray();

                if ((ints[0] <= ints[2] && ints[1] >= ints[3]) ||
                    (ints[0] >= ints[2] && ints[1] <= ints[3]))
                    {
                    score++;
                }
            }
        }
        return score;
    }

    [TestCase("day04.input", ExpectedResult = 883)]
    public int Part2(string input)
    {
        var score = 0;
        var lines = File
            .ReadAllLines(input);
        foreach (var line in lines)
        {
            var m = pairRegex.Match(line);
            if (m.Success)
            {
                var g = m.Groups;
                var ints = g.Values.Skip(1).Select(i => int.Parse(i.Value)).ToArray();

                if ((ints[0] <= ints[2] && ints[1] >= ints[3]) ||
                    (ints[0] >= ints[2] && ints[1] <= ints[3]) ||
                    (ints[0] >= ints[2] && ints[0] <= ints[3]) ||
                    (ints[1] >= ints[2] && ints[1] <= ints[3]))
                {
                    score++;
                }
            }
        }
        return score;
    }
    public static int CalculateScore(string line)
    {
        var onr = line[0] - 'A';
        var mnr = line[2] - 'X';
        var score = (((onr + 1) % 3 == mnr) ? 6 : (onr == mnr) ? 3 : 0);
        Console.WriteLine($" {mnr + 1} +{score}");
        return mnr + 1 + score;
    }

    public static string SetMove(string line)
    {
        Console.Write(line);
        Console.Write(" => ");
        var onr = line[0] - 'A';
        var mnr = line[2] - 'X';
        var move = (char)('X' + (onr + mnr + 2) % 3);
        if (move is < 'X' or > 'Z') throw new Exception("help");
        var output = $"{(char)(onr+'A')} {move}";
        Console.Write(output);
        return output;
    }
}
