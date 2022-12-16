using System.Text.RegularExpressions;

namespace AoC2022;

public class Day13old
{
    [Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(13);
    }

    [TestCase("day13example1.input", ExpectedResult = 13)]
    [TestCase("day13example2.input", ExpectedResult = 1)]
    [TestCase("day13example3.input", ExpectedResult = 1)]
    [TestCase("day13example4.input", ExpectedResult = 0)]
    [TestCase("day13.input", ExpectedResult = -1)] //6015 too low, 6147 too low 6240 too high
    public int Part1(string input)
    {
        var lines = File.ReadAllLines(input);

        var pairs = lines.GroupedMapReduce(l => l == "", c => c, Compare).Select((c, i) => c ? i+1 : 0).Sum();
        return pairs;
    }

    private bool Compare(IEnumerable<string> s)
    {
        var s1 = s.FirstOrDefault();
        var s2 = s.Skip(1).FirstOrDefault();
        if (s1 == null || s2 == null) return false;
        TestContext.Out.WriteLine($"compare");
        TestContext.Out.WriteLine(s1);
        TestContext.Out.WriteLine(s2);
        var e1 = s1.GetEnumerator();
        var e2 = s2.GetEnumerator();
        var res = MoveBoth(e1, e2);
        if (res.HasValue)
        {
            TestContext.Out.WriteLine($"direct {res}");
            return res.Value;
        }
        else
        {
            var v = Compare(e1, e2, true);
            TestContext.Out.WriteLine(v);
            return v;
        }
    }

    private bool? MoveBoth(IEnumerator<char> s1, IEnumerator<char> s2)
    {
        var m1 = s1.MoveNext();
        var m2 = s2.MoveNext();
        if (!m1) return true;
        if (!m2) return false;
        return null;
    }

    private bool Compare(IEnumerator<char> s1, IEnumerator<char> s2, bool skipMove = false)
    {
        bool? result = null;
        if (!skipMove)
        {
            result = MoveBoth(s1, s2);
        }
        while (result == null)
        {
            TestContext.Out.Write($"C {s1.Current} - {s2.Current};");
            var n1 = ParseInt(s1);
            var n2 = ParseInt(s2);
            TestContext.Out.WriteLine($" {n1} - {n2}");

            if (n1.HasValue && !n2.HasValue && s2.Current == ',')
            {
                s2.MoveNext();
                Compare(s1, s2, true);
            }

            if (n2.HasValue && !n1.HasValue && s1.Current == ',')
            {
                s1.MoveNext();
                Compare(s1, s2, true);
            }

            if (n1 < n2) return true;
            else
            if (n1 > n2) return false;
            else
            if (n1.HasValue && n1 == n2)
                return Compare(s1, s2);
            else
            if (n1.HasValue)
            {
                if (s2.Current == '[')
                {
                    return Compare(FirstToList(n1.Value, s1).GetEnumerator(), s2, true);
                }
                if (s2.Current == ']') return false;
            }
            else if (n2.HasValue)
            {
                if (s1.Current == '[')
                {
                    return Compare(s1, FirstToList(n2.Value, s2).GetEnumerator(), true);
                }
                if (s1.Current == ']') return true;
            }
            else
            if (s1.Current == '[' && s2.Current == '[') return Compare(s1, s2);
            else
            if (s1.Current == ']' && s2.Current == ']') return Compare(s1, s2);
            else
            if (s1.Current == ',' && s2.Current == ',') return Compare(s1, s2);
            else
            if (s1.Current == ']' && s2.Current == ',') return true;
            else
            if (s1.Current == ']' && s2.Current == '[') return true;
            else
            if (s1.Current == ',' && s2.Current == ']') return false;
            else
            if (s1.Current == '[' && s2.Current == ']') return false;
            else

                //throw new Exception("help1");
                result = MoveBoth(s1, s2);
        }
        return result.Value;
    }

    IEnumerable<char> FirstToList(int n1, IEnumerator<char> s)
    {
        var n1str = n1.ToString();
        foreach (var c in n1str) yield return c;
        yield return ']';
        do
        {
            yield return s.Current;
        } while (s.MoveNext());
    }

    private int? ParseInt(IEnumerator<char> s)
    {
        int? nr = null;
        do
        {
            if (s.Current is >= '0' and <= '9')
            {
                if (nr == null) nr = 0;
                nr = nr * 10 + (s.Current - '0');
            }
            else
            {
                break;
            }
        }
        while (s.MoveNext());
        return nr;
    }

    [TestCase("day12.input", ExpectedResult = 402)]
    public int Part2(string input)
    {
        var lines = File.ReadAllLines(input);
        var steps = 0;
        return steps;
    }
}


