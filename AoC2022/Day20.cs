using FluentAssertions.Execution;
using System.Text.RegularExpressions;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;
using System.Diagnostics;

namespace AoC2022;

public class Day20
{
    [Test]
    public async Task Fetch()
    {
     //   await FetchInput.Fetch(20);
    }

    [SetUp]
    public void setup()
    {
        var lines = File.ReadAllLines("day20.input");
       // r.Match("x");
    }

    public class NR {
        public long nr { get; }

        public NR(long nr)
        {
            this.nr = nr;
        }
    }

    [TestCase("day20.input", ExpectedResult = 9945)] //410 too low // 2563 too low // not 5149
    [TestCase("day20example1.input", ExpectedResult = 3)]
    [TestCase("day20example2.input", ExpectedResult = 1)]
    [TestCase("day20example3.input", ExpectedResult = 7)]
    public long Part1(string input)
    {
        checked
        {
            var lines = File.ReadAllLines(input).ToArray();
            var numbers = lines.Select(n => new NR(long.Parse(n))).ToArray();
            var target = numbers.ToList();

            return Calc(numbers, target, 1);
        }
    }

    [TestCase("day20.input", ExpectedResult = 3338877775442)]  //14953530144025 too high //8926669093847 too high
    [TestCase("day20example1.input", ExpectedResult = 1623178306)]
    [TestCase("day20example2.input", ExpectedResult = -4057945765)]
    public long Part2(string input)
    {
        checked
        {
            var lines = File.ReadAllLines(input).ToArray();
            var numbers = lines.Select(n => new NR(long.Parse(n) * 811589153)).ToArray();
            var target = numbers.ToList();

            return Calc(numbers, target, 10);
        }
    }

    [TestCase("0,-1,1,2,3,4,5", ExpectedResult = "0,4,1,5,2,3,-1")]
    [TestCase("0,1,2,3,4,5,26", ExpectedResult = "0,4,1,5,2,3,26")]
    [TestCase("0,1,2,3,4,5,-26", ExpectedResult = "0,4,1,5,2,3,-26")]
    [TestCase("0,1,2,3,4,5,28", ExpectedResult = "0,4,1,5,28,2,3")]
    [TestCase("0,1,2,3,4,5,-28", ExpectedResult = "0,4,1,5,-28,2,3")]
    [TestCase("0,1,2,3,4,5,6", ExpectedResult = "0,4,1,5,2,3,6")]
    [TestCase("-1,0,1,2,3,4,5", ExpectedResult = "0,4,1,5,2,3,-1")]
    [TestCase("-8,0,1,2,3,4,5", ExpectedResult = "0,4,1,2,-8,5,3")]
    [TestCase("-15,0,1,2,3,4,5", ExpectedResult = "0,4,1,2,-15,5,3")]
    [TestCase("-15,0,1,2,3,4,26", ExpectedResult = "0,4,1,2,-15,26,3")]
    [TestCase("-15,10,1,2,3,3,0", ExpectedResult = "0,4,1,2,-15,26,3")]
    [TestCase("1,1,1,1,0,1,1,1", ExpectedResult = "1,1,1,1,1,0,1,1")]
    public string test1(string input)
    {
        var numbers = input.Split(",").Select(n => new NR(long.Parse(n))).ToArray();
        var target = numbers.ToList();
        Calc(numbers, target, 1);
        return string.Join(",",target.Select(n=>n.nr));
    }

    private long Calc(NR[] numbers, List<NR> target, int mixes)
    {
        checked
        {
            NR nr0 = null;
            Console.WriteLine($"=== Initial ===");
            Print(target);
            for (int m = 0; m < mixes; m++)
            {
                foreach (var nr in numbers)
                {
                    if (nr.nr == 0) nr0 = nr;
                    var idxo = target.IndexOf(nr);
                    if (idxo == -1) throw new Exception("not found");

                    if (nr.nr == 5)
                    {
                        var xasdf = 12;
                    }
                    


                    var idxn = idxo + nr.nr;
                    if (idxn < 0)
                    {
                        idxn = (idxn % (target.Count - 1)) + target.Count - 1;

                    }
                    if (idxn >= target.Count-1)
                    {
                        idxn %= target.Count-1;
                    }

                    if (target.Count < 100) Console.WriteLine($"move {nr.nr} from {idxo} to {idxn}");
                    
                    if (nr.nr != 0)
                    {
                        target.RemoveAt(idxo);
                        
                        if ((idxn == 0))
                        {
                            target.Add(nr);
                        }
                        else
                        {
                            target.Insert((int)idxn, nr);
                        }
                    }

                    if (target.Count < 100) Print(target);
                }
                Console.WriteLine($"=== ROUND {m} ===");
                Print(target);
            }

            var idx0 = target.IndexOf(nr0);
            Console.WriteLine($"{nr0.nr} {idx0} {target[(idx0 + 1000) % target.Count].nr} {target[(idx0 + 2000) % target.Count].nr} {target[(idx0 + 3000) % target.Count].nr}");
            return target[(idx0 + 1000) % target.Count].nr + target[(idx0 + 2000) % target.Count].nr + target[(idx0 + 3000) % target.Count].nr;
        }
    }

    private void Print(IEnumerable<NR> target)
    {
        foreach (var nr in target)
        {
            Console.Write($"{nr.nr}, ");
        }
        Console.WriteLine();
    }
}
