namespace AoC2024
{
    public class Day02
    {
        [Test(ExpectedResult = 585)]
        public long Part1()
        {
            var nrs = File.ReadLines("day02.input")
                .Select(l => l.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray());

            var safeinc = nrs.Count(report => report.Zip(report.Skip(1)).All(l => Condition(l.First, l.Second)));
            var safedec = nrs.Count(report => report.Skip(1).Zip(report).All(l => Condition(l.First, l.Second)));

            return safeinc+safedec;
        }

        [TestCase("day02.input", ExpectedResult = 626)] //760, 631 too high. fail 609
        [TestCase("day02.ex1.input", ExpectedResult = 4)] 
        [TestCase("day02.ex2.input", ExpectedResult = 6)] 
        [TestCase("day02.ex3.input", ExpectedResult = 6)] 
        public long Part2(string path)
        {
            var nrs = File.ReadLines(path)
                .Select(l => l.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray());

            var safe = nrs.Count(report => Check(report));

            return safe;
        }
        
        public bool Condition(long first, long second)
        {
            return first - second >= 1 && first - second <= 3;
        }

        public bool Check(long[] report)
        {
            for (int i = 0; i < report.Length; i++)
            {
                var copy = report.ToList();
                copy.RemoveAt(i);
                var r = copy.Zip(copy.Skip(1)).All(l => Condition(l.First, l.Second))
                     || copy.Skip(1).Zip(copy).All(l => Condition(l.First, l.Second));
                if (r) return true;
            }
            return false;
        }
    }
}