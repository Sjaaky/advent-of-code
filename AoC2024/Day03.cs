using System.Text.RegularExpressions;

namespace AoC2024
{
    public class Day03
    {
        [TestCase("day03.input",ExpectedResult = 170068701)]
        [TestCase("day03.ex1.input",ExpectedResult = 20)]
        [TestCase("day03.ex2.input",ExpectedResult = 161)]
        public long Part1(string path)
        {
            var mul = new Regex(@"mul\((\d+),(\d+)\)");
            var nrs = File
                .ReadLines(path)
                .Sum(l =>
                {
                    return mul.Matches(l).OfType<Match>().Sum(m => int.Parse(m.Groups[1].ValueSpan) * int.Parse(m.Groups[2].ValueSpan));
                });


            return nrs;
        } 

        [TestCase("day03.input",ExpectedResult = 78683433)] //83596387 too high
        [TestCase("day03.ex1.input",ExpectedResult = 20)]
        [TestCase("day03.ex2.input",ExpectedResult = 48)]
        [TestCase("day03.ex3.input",ExpectedResult = 48)]
        public long Part2(string path)
        {
            var doe = new Regex(@"(?:^|do\(\))(.+?)(?:don't\(\)|$)");
            var mul = new Regex(@"mul\((\d+),(\d+)\)");

            var input = String.Join("", File.ReadLines(path));
            var sum = doe
                .Matches(input).OfType<Match>()
                .Sum(m => mul
                    .Matches(m.Groups[1].Value).OfType<Match>()
                    .Sum(m => int.Parse(m.Groups[1].ValueSpan) * int.Parse(m.Groups[2].ValueSpan)));

            return sum;
        }
    }
}