namespace AoC2022;

public class Day02
{
    [TestCase("day02.input", ExpectedResult = 12645)]
    [TestCase("day02example1.input", ExpectedResult = 15)] 
    public int Part1(string input)
    {
        var score = File
            .ReadAllLines(input)
            .Select(CalculateScore)
            .Sum();

        Console.WriteLine(score);
        return score;
    }
    
    [TestCase("day02.input", ExpectedResult = 11756)]
    [TestCase("day02example1.input", ExpectedResult = 12)]
    public int Part2(string input)
    {
        var score = File
            .ReadAllLines(input)
            .Select(SetMove)
            .Select(CalculateScore)
            .Sum();

        Console.WriteLine(score);
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
