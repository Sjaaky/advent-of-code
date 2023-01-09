using System.Text.RegularExpressions;
using static AoC2022.Day07;

namespace AoC2022;

public class Day25
{
    [Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(25);
    }

    [TestCase("day25.input", ExpectedResult = "2-2=12=1-=-1=000=222")]
    [TestCase("day25example1.input", ExpectedResult = "2=-1=0")]
    public string Part1(string input)
    {
        maxBorrow = 0;
        var lines = File.ReadAllLines(input).ToArray();

        var sum = lines.Select(SnafuToDecimal).Sum();
        
        var s = DecimalToSnafu(sum);
        Console.WriteLine(maxBorrow);
        return s;
    }

    [TestCase("day25example1.input", ExpectedResult = 4890)]
    public long TestSnafuToDecimal(string input)
    {
        maxBorrow = 0;
        var lines = File.ReadAllLines(input).ToArray();

        var sum = lines.Select(SnafuToDecimal).Sum();
        return sum;
    }

    [TestCase("day25example1.input")]
    [TestCase("day25example2.input")]
    public void TestSnafuToDecimalToSnafu(string input)
    {
        maxBorrow = 0;
        var lines = File.ReadAllLines(input).ToArray();

        foreach (var line in lines)
        {
            var dec = SnafuToDecimal(line);
            var snafu = DecimalToSnafu(dec);
            Console.WriteLine($"{line} => {dec} => {snafu}");
            snafu.Should().Be(line);
        }
        var sum = lines.Select(SnafuToDecimal).Sum();
    }

    [Test]
    public void TestSnafuToDecimalToSnafuLoop()
    {
        maxBorrow = 0;
        for (long input = 0; input < 1000000; input++)
        {
            var snafu = DecimalToSnafu(input);
            var output = SnafuToDecimal(snafu);
            Console.WriteLine($"{input} => {snafu} => {output}");
            input.Should().Be(output);
        }
        Console.WriteLine(maxBorrow);
    }

    long SnafuToDecimal(string snafu)
    {
        long nr = 0;
        foreach (char c in snafu)
        {
            nr *= 5;
            nr += c switch
            {
                '0' => 0,
                '1' => 1,
                '2' => 2,
                '-' => -1,
                '=' => -2
            };
        }
        return nr;
    }
    static int maxBorrow = 0;
    string DecimalToSnafu(long dec)
    {
        char[] nr = new char[40];
        var pow5 = Enumerable.Range(0, 40).Select(n => (n, (long)Math.Pow(5, n))).Reverse();
        var first = -1;
        foreach (var (n, p) in pow5)
        {
            var digit = dec / p;
            if (digit > 0 || first > -1)
            {
                first = Math.Max(first, n);
                var sn = digit switch
                {
                    0 => '0',
                    1 => '1',
                    2 => '2',
                    3 => '=',
                    4 => '-'
                };
                nr[n] = sn;
                if (digit is 3 or 4)
                {
                    var borrow = true;
                    var borrow_idx = 1;
                    while (borrow)
                    {
                        borrow = nr[n + borrow_idx] == '2';
                        nr[n + borrow_idx] = nr[n + borrow_idx] switch
                        {
                            (char)0 => '1',
                            '0' => '1',
                            '1' => '2',
                            '2' => '=',
                            '-' => '0',
                            '=' => '-',
                        };
                        first = Math.Max(first, n + borrow_idx);
                        maxBorrow = Math.Max(maxBorrow, borrow_idx);
                        borrow_idx++;
                    }
                }
            }
            dec = dec % p;
        }

        return new string(new string(nr, 0, first+1).Reverse().ToArray());
    }

}
