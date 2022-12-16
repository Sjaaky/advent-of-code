
namespace AoC2022;

public class Day03
{
    [TestCase("day03.input", ExpectedResult = 2552)]
    [TestCase("day03example1.input", ExpectedResult = 70)] 
    public int Part1(string input)
    {
        var lines = File.ReadAllLines(input);
        var score = 0;
        int[] charcount = new int[256];
        int idx = -1;
        foreach (var line in lines)
        {
            idx++;
            idx = idx % 3;

            for(int i = 0; i < line.Length; i++)
            {
                var mask = 1 << idx;
                Console.Write($"{line[i]} {mask} - ");
                charcount[line[i]] |= mask;
            }
            if (idx == 2)
            {
                for (int i = 0; i < charcount.Length; i++)
                {
                    if (charcount[i] > 0)
                    {
                        Console.Write($"{(char)i} {charcount[i]},");
                    }
                    if (charcount[i] == 7)
                    {
                        var ls = i switch { >= 'a' and <= 'z' => i - 'a' + 1, _ => i - 'A' + 27 };
                        Console.WriteLine($"{(char)i} => {ls}");
                        score += ls;
                    }
                }
                charcount = new int[256];
                Console.WriteLine();
            }

        }
        return score;
    }

    [TestCase("day03.input", ExpectedResult = 2552)]
    [TestCase("day03example1.input", ExpectedResult = 70)]
    public int Part2(string input)
    {
        var lines = File
            .ReadAllLines(input);
        var score = 0;
        int[] charcount = new int[256];
        int idx = -1;
        foreach (var line in lines)
        {
            idx++;
            idx = idx % 3;

            for (int i = 0; i < line.Length; i++)
            {
                var mask = 1 << idx;
                Console.Write($"{line[i]} {mask} - ");
                charcount[line[i]] |= mask;
            }
            if (idx == 2)
            {
                for (int i = 0; i < charcount.Length; i++)
                {
                    if (charcount[i] > 0)
                    {
                        Console.Write($"{(char)i} {charcount[i]},");
                    }
                    if (charcount[i] == 7)
                    {
                        var ls = i switch { >= 'a' and <= 'z' => i - 'a' + 1, _ => i - 'A' + 27 };
                        Console.WriteLine($"{(char)i} => {ls}");
                        score += ls;
                    }
                }
                charcount = new int[256];
                Console.WriteLine();
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
