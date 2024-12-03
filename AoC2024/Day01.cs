namespace AoC2024
{
    public class Day01
    {
        [Test(ExpectedResult = 1110981)]
        public long Part1()
        {
            var nrs = File
                .ReadLines("day01.input")
                .Select(l => l.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray());

            var left = nrs.Select(l => l.First()).Order().ToArray();
            var right = nrs.Select(l => l.Skip(1).First()).Order().ToArray();

            var sum = left.Zip(right).Sum(l => Math.Abs(l.First - l.Second));

            return sum;
        }

        [Test(ExpectedResult = 24869388)]
        public long Part2()
        {
            var nrs = File
                .ReadLines("day01.input")
                .Select(l => l.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray());

            var left = nrs.Select(l => l.First()).Order().ToArray();
            var right = nrs.Select(l => l.Skip(1).First()).Order().ToArray();

            var sum = left.Sum(l => l * right.Count(r => r == l));

            return sum;
        }
    }
}