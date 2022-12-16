namespace AoC2022;

public class Day01
{
    [Test]
    public void Part1()
    {
        var maxCalories = File
            .ReadAllLines("day01.input")
            .SumCaloriesPerElf()
            .Max();

        Console.WriteLine(maxCalories);
        maxCalories.Should().Be(67622);
    }

    [Test]
    public void Part1a()
    {
        var maxCalories = File
            .ReadAllLines("day01.input")
            .Split(l => l != "", int.Parse, Enumerable.Sum)
            .Max();

        Console.WriteLine(maxCalories);
        maxCalories.Should().Be(67622);
    }
    [Test]
    public void Part1b()
    {
        var maxCalories = File
            .ReadAllLines("day01.input")
            .GroupedMapReduce(string.IsNullOrEmpty, int.Parse, Enumerable.Sum)
            .Max();

        Console.WriteLine(maxCalories);
        maxCalories.Should().Be(67622);
    }
    [Test]
    public void Part1c()
    {
        var maxCalories = File
            .ReadAllLines("day01.input")
            .SumCaloriesPerElf()
            .Max();

        Console.WriteLine(maxCalories);
        maxCalories.Should().Be(67622);
    }



    [Test]
    public void Part2a()
    {
        var caloriesTop3 = File
            .ReadAllLines("day01.input")
            .SumCaloriesPerElf()
            .OrderByDescending(c => c)
            .Take(3)
            .Sum();

        Console.WriteLine(caloriesTop3);
        caloriesTop3.Should().Be(201491);
    }

    [Test]
    public void Part2()
    {
        var caloriesTop3 = File
            .ReadAllLines("day01.input")
            .GroupedMapReduce(string.IsNullOrEmpty, int.Parse, Enumerable.Sum)
            .OrderByDescending(c => c)
            .Take(3)
            .Sum();

        Console.WriteLine(caloriesTop3);
        caloriesTop3.Should().Be(201491);
    }

    [Test]
    public void Part2b()
    {
        var caloriesTop3 = File
            .ReadAllLines("day01.input")
            .SplitAndAggregate<int>(string.IsNullOrEmpty, (a, l) => a + int.Parse(l))
            .OrderByDescending(c => c)
            .Take(3)
            .Sum();

        Console.WriteLine(caloriesTop3);
        caloriesTop3.Should().Be(201491);
    }

    [Test]
    public void Part2c()
    {
        var caloriesTop3 = File
            .ReadAllLines("day01.input")
            .MapReduce(l => (int.TryParse(l, out int nr), nr), (a, b) => a + b)
            .OrderByDescending(c => c)
            .Take(3)
            .Sum();

        Console.WriteLine(caloriesTop3);
        caloriesTop3.Should().Be(201491);
    }
}

public static class Extensions
{
    public static IEnumerable<R> Split<R, T>(
        this IEnumerable<string> lines, 
        Func<string, bool> predicate, 
        Func<string, T> conv, 
        Func<IEnumerable<T>, R> f)
    {
        int pos = 0;
        int tw = 0;
        do
        {
            var l = lines.Skip(pos);
            if (!l.Any()) break;
            var x = l.TakeWhile((str, idx) => { tw = pos + idx; return predicate(str); });
            yield return f(x.Select(conv));
            pos = tw + 1;
        } while (true);
    }

    public static IEnumerable<R> GroupedMapReduce<R, T>(
        this IEnumerable<string> lines,
        Func<string, bool> predicate,
        Func<string, T> conv,
        Func<IEnumerable<T>, R> f)
    {
        var it = lines.GetEnumerator();
        while (it.MoveNext())
        {
            var t = it.TakeUntil(predicate).ToList();
            var s = t.Select(conv).ToList();
            yield return f(s);
        }
    }

    public static IEnumerable<R> TakeUntil<R>(this IEnumerator<R> it, Func<R, bool> pred)
    {
        do
        {
            if (!pred(it.Current))
            {
                yield return it.Current;
            }
            else
            {
                yield break;
            }
        } while (it.MoveNext());
    }

    public static IEnumerable<int> SumCaloriesPerElf(this IEnumerable<string> lines)
    {
        int total = 0;
        foreach (var line in lines)
        {
            if (int.TryParse(line, out var cals))
            {
                total += cals;
            }
            else
            {
                yield return total;
                total = 0;
            }
        }
    }

    public static IEnumerable<T> SplitAndAggregate<T>(this IEnumerable<string> lines, Func<string, bool> pred, Func<T, string, T> aggregate)
    {
        T? total = default;
        foreach (var line in lines)
        {
            if (pred(line))
            {
                yield return total;
                total = default;
            }
            else
            {
                total = aggregate(total, line);
            }
        }
    }

    public static IEnumerable<T> MapReduce<T>(this IEnumerable<string> lines, Func<string, (bool, T)> pred, Func<T, T, T> aggregate)
    {
        T? total = default;
        foreach (var line in lines)
        {
            var (success, nr) = pred(line);
            if (success)
            {
                total = aggregate(total!, nr);
            }
            else
            {
                yield return total!;
                total = default;
            }
        }
    }

    public static IEnumerable<T> SplitAndAggregate2<T>(this IEnumerable<string> lines, Func<string, (bool, T)> pred, Func<T, T, T> aggregate)
    {
        var it = lines.GetEnumerator();
        T? total = default;
        while (it.MoveNext())
        {
            var (success, nr) = pred(it.Current);
            if (success)
            {
                total = aggregate(total!, nr);
            }
            else
            {
                yield return total!;
                total = default;
            }
        }
    }
}