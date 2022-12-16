namespace AoC2022;

public class Day11
{
    // [Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(11);
    }

    public Monkey[] Input()
        => new Monkey[]
    {
        new(0, new long[] {72, 97 },
            o => o*13,
            i => (i % 19 == 0) ? 5: 6),
        new(1, new long[]{55,70,90,74,95},
            o => o*o,
            i => (i% 7 == 0) ? 5: 0),
        new(2, new long[]{74,97,66,57},
            o => o+6,
            i => (i% 17 == 0) ? 1: 0),
        new(3, new long[]{86,54,53},
            o => o+2,
            i => (i% 13 == 0) ? 1: 2),
        new(4, new long[]{50, 65, 78, 50, 62, 99},
            o => o+3,
            i => (i% 11 == 0) ? 3: 7),
        new(5, new long[]{90},
            o => o+4,
            i => (i% 2 == 0) ? 4: 6),
        new(6, new long[]{88, 92, 63, 94, 96, 82, 53, 53},
            o => o+8,
            i => (i% 5 == 0) ? 4: 7),
        new(7, new long[]{  70, 60, 71, 69, 77, 70, 98},
            o => o*7,
            i => (i% 3 == 0) ? 2:3 )
    };
    public long InputM = 19 * 7 * 17 * 13 * 11 * 2 * 5 * 3;

    public Monkey[] Example()
        => new Monkey[]
    {
        new(0, new long[] {79, 98 },
            o => o*19,
            i => (i % 23 == 0) ? 2: 3),
        new(1, new long[]{54, 65, 75, 74},
            o => o+6,
            i => (i% 19 == 0) ? 2: 0),
        new(2, new long[]{79, 60, 97},
            o => o*o,
            i => (i% 13 == 0) ? 1: 3),
        new(3, new long[]{74},
            o => o+3,
            i => (i% 17 == 0) ? 0: 1)
    };
    public long ExampleM = 23 * 19 * 13 * 17;

    [TestCase(ExpectedResult = 10605)]
    public long Part1Example()
     => ExecutePart1(Example());

    [TestCase(ExpectedResult = 58056)]
    public long Part1()
     => ExecutePart1(Input());

    [TestCase(ExpectedResult = 2713310158L)]
    public long Part2Example()
     => ExecutePart2(Example(), ExampleM);

    [TestCase(ExpectedResult = 15048718170)]
    public long Part2()
     => ExecutePart2(Input(), InputM);

    private static long ExecutePart1(Monkey[] monkeys)
    {
        for (int r = 1; r <= 20; r++)
        {
            foreach (var m in monkeys)
            {
                m.Inspect(monkeys);
            }
            PrintRound(monkeys, r);
        }
        var sum = monkeys
            .Select(m => m.Inspected)
            .OrderByDescending(i => i)
            .Take(2)
            .Aggregate((a, b) => a * b);

        return sum;
    }

    private static long ExecutePart2(Monkey[] monkeys, long max)
    {
        for (int r = 1; r <= 10000; r++)
        {
            foreach (var m in monkeys)
            {
                m.Inspect2(monkeys, max);
            }
            if ((r % 1000) == 0 || r == 20 || r == 1)
            {
                PrintRound(monkeys, r);
            }
        }
        var sum = monkeys
            .Select(m => m.Inspected)
            .OrderByDescending(i => i)
            .Take(2)
            .Aggregate((a, b) => a * b);
        return sum;
    }

    private static void PrintRound(Monkey[] monkeys, int r)
    {
        Console.WriteLine($"==== Round {r} =====");
        foreach (var m in monkeys)
        {
            Console.WriteLine(m);
        }
    }
    //[TestCase("day11.input", ExpectedResult = 2427)]
    //public long Part2Example(string input)
    //{
    //    var monkeys = Example;
    //    var max = ExampleM;

    //    for (int r = 1; r <= 10000; r++)
    //    {
    //        foreach (var m in monkeys)
    //        {
    //            m.Inspect2(monkeys, max);
    //        }
    //        if ((r % 1000) == 0 || r == 20 || r == 1)
    //        {
    //            Console.WriteLine($"==== Round {r} =====");
    //            foreach (var m in monkeys)
    //            {
    //                Console.WriteLine(m);
    //            }
    //        }
    //    }
    //    var sum = monkeys.Select(m => m.Inspected).OrderByDescending(i => i).Take(2).Aggregate((a, b) => a * b);

    //    return sum;
    //}

}

public class Monkey
{
    public List<long> Items;
    public Monkey(int nr, long[] items, Func<long, long> op, Func<long, int> toMonkey)
    {
        Items = items.ToList();
        Nr = nr;
        Operation = op;
        ToMonkey = toMonkey;
    }

    public int Nr { get; }
    public Func<long, long> Operation { get; }
    public Func<long, int> ToMonkey { get; }

    public long Inspected { get; private set; } = 0;
    public void Catch(long item)
    {
        Items.Add(item);
    }
    public void Inspect(Monkey[] monkeys)
    {
        foreach (var item in Items)
        {
            var newlevel = Operation(item) / 3;
            var newmonkey = ToMonkey(newlevel);
            monkeys[newmonkey].Catch(newlevel);
            Inspected++;
        }
        Items.Clear();
    }

    public void Inspect2(Monkey[] monkeys, long max)
    {
        foreach (var item in Items)
        {
            checked
            {
                var newlevel = Operation(item);
                newlevel = newlevel % max;
                var newmonkey = ToMonkey(newlevel);
                monkeys[newmonkey].Catch(newlevel);
            }
            Inspected++;
        }
        Items.Clear();
    }

    public override string ToString()
    {
        return $"Monkey {Nr} inspected {Inspected,5}: {string.Join(", ", Items)}";
    }
}

