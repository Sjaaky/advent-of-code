namespace AoC2022;

public class Day06
{
    [TestCase("day06.input", ExpectedResult = 1760)]
    [TestCase("day06example1.input", ExpectedResult = 10)]
    public int Part1(string input)
    {
        var stream = File.ReadAllText(input);

        for (int i = 3; i < stream.Length; i++)
        {
            if (stream[i] != stream[i - 1] &&
                stream[i] != stream[i - 2] &&
                stream[i] != stream[i - 3] &&
                stream[i - 1] != stream[i - 2] &&
                stream[i - 1] != stream[i - 3] &&
                stream[i - 2] != stream[i - 3]
                )
            {
                return i + 1;
            }
        }
        for (int i = 4; i < stream.Length; i++)
        {
            if (stream[(i - 4)..i].Distinct().Count() == 4)
            {
                return i;
            }
        }
        return 0;
    }

    [TestCase("day06.input", ExpectedResult = 2974)]
    [TestCase("day06example1.input", ExpectedResult = 29)]
    public int Part2(string input)
    {
        var stream = File.ReadAllText(input);

        for (int i = 14; i < stream.Length; i++)
        {
            if (stream[(i - 14)..i].Distinct().Count() == 14)
            {
                return i;
            }
        }
        return 0;
    }
}

