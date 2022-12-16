using System.Text.RegularExpressions;

namespace AoC2022;

public class Token
{
    public bool IsStartOfList { get; private set; }
    public bool IsEndOfList { get; private set; }
    public bool IsNumber { get; private set; }
    public int Number { get; private set; }

    private Token() { }

    public Token(int number)
    {
        Number = number;
    }

    public static Token StartOfList = new Token { IsStartOfList = true };
    public static Token EndOfList = new Token { IsEndOfList = true };
    public static Token? ParseInt(string s, ref int i)
    {
        int? nr = null;
        while (s[i] is >= '0' and <= '9')
        {
            if (!nr.HasValue) nr = 0;
            nr = nr * 10 + (s[i] - '0');
            i++;
        }
        if (nr.HasValue)
        {
            i--;
            return new Token { IsNumber = true, Number = nr.Value };
        }
        else
        {
            return null;
        }
    }

    public override string ToString()
     => IsStartOfList ? "[" : IsEndOfList ? "]" : IsNumber ? Number.ToString() : "??";

    public static IEnumerable<Token> Tokenize(string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            var t = s[i] switch
            {
                '[' => Token.StartOfList,
                ']' => Token.EndOfList,
                >= '0' and <= '9' => Token.ParseInt(s, ref i),
                _ => null
            };
            if (t != null)
            {
                yield return t;
            }
        }
    }

}

public class Day13
{
    [Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(13);
    }

    [TestCase("day13example1.input", ExpectedResult = 13)]
    [TestCase("day13example2.input", ExpectedResult = 1)]
    [TestCase("day13example3.input", ExpectedResult = 0)]
    [TestCase("day13example4.input", ExpectedResult = 0)]
    [TestCase("day13example5.input", ExpectedResult = 1)]
    [TestCase("day13.input", ExpectedResult = 6187)] //6015 too low, 6147 too low 6240 too high, 5466 incorrect
    public int Part1(string input)
    {
        var lines = File.ReadAllLines(input);
        var pairs = lines.GroupedMapReduce(l => l == "", c => c, Compare).Select((c, i) => c ? i+1 : 0).Sum();
        return pairs;
    }

    private bool Compare(IEnumerable<string> s)
    {
        var pc = new PacketComparer();
        var v = pc.Compare(s.FirstOrDefault(), s.Skip(1).FirstOrDefault());
        return v == -1 ? true : false;
    }

    [TestCase("day13.input", ExpectedResult = 23520)]
    public int Part2(string input)
    {
        const string packet2 = "[[2]]";
        const string packet6 = "[[6]]";
        var lines = File.ReadAllLines(input).Where(l => l != "").Union(new[] { packet2, packet6 }).ToList();
        var ordered = lines.OrderBy(l => l, new PacketComparer()).ToArray();
        foreach (var l in ordered)
        {
            Console.WriteLine(l);
        }

        var i2 = Array.FindIndex(ordered, 0, x => x == packet2) + 1;
        var i6 = Array.FindIndex(ordered, 0, x => x == packet6) + 1;
        return i2 * i6;
    }

    public class PacketComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            var t1 = Token.Tokenize(x!).ToList();
            var t2 = Token.Tokenize(y!).ToList();
            var v = Compare(t1, t2);
            return v ? -1 : 1;
        }

        private bool Compare(List<Token> s1, List<Token> s2)
        {
            int i1 = 0;
            int i2 = 0;

            while (i1 < s1.Count && i2 < s2.Count)
            {
                var t1 = s1[i1];
                var t2 = s2[i2];
                TestContext.Out.WriteLine($"C {t1} - {t2};");

                if (t1.IsNumber)
                {
                    if (t2.IsNumber)
                    {
                        if (t1.Number < t2.Number) return true;
                        else
                        if (t1.Number > t2.Number) return false;
                        else
                        if (t1.Number == t2.Number) { i1++; i2++; }
                    }
                    else
                    {
                        if (t2.IsStartOfList)
                        {
                            s1.Insert(i1, Token.StartOfList);
                            s1.Insert(i1 + 2, Token.EndOfList);
                        }
                        else
                        if (t2.IsEndOfList) return false;
                    }
                }
                else
                if (t2.IsNumber)
                {
                    if (t1.IsStartOfList)
                    {
                        s2.Insert(i2, Token.StartOfList);
                        s2.Insert(i2 + 2, Token.EndOfList);
                    }
                    else
                    if (t1.IsEndOfList) return true;
                }
                else
                if (t1.IsStartOfList && t2.IsStartOfList) { i1++; i2++; }
                else
                if (t1.IsEndOfList && t2.IsEndOfList) { i1++; i2++; }
                else
                if (t1.IsEndOfList && t2.IsStartOfList) return true;
                else
                if (t1.IsStartOfList && t2.IsEndOfList) return false;
            }
            throw new Exception("case not implemented");
        }
    }
}



