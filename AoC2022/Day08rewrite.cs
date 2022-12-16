using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AoC2022;

public class Day08rewrite
{
    [TestCase("day08.input", ExpectedResult = 1782)]
    [TestCase("day08example1.input", ExpectedResult = 21)]
    public long Part1(string input)
    {
        var trees = File.ReadAllLines(input);

        var visibility = new bool?[trees.Count(), trees.Length];
        int max = -1;
        for (int row = 0; row < trees.Count(); row++)
        {
            max = -1;
            for (int col = 0; col < trees.Length; col++)
            {
                if (max < trees[row][col])
                {
                    max = trees[row][col];
                    visibility[row, col] = true;
                }
            }
        }
        Print(visibility);

        for (int row = 0; row < trees.Count(); row++)
        {
            max = -1;
            for (int col = trees.Length - 1; col >= 0; col--)
            {
                if (max < trees[row][col])
                {
                    max = trees[row][col];
                    visibility[row, col] = true;
                }
            }
        }
        Print(visibility);


        for (int col = 0; col < trees.Length; col++)
        {
            max = -1;
            for (int row = 0; row < trees.Count(); row++)
            {
                if (max < trees[row][col])
                {
                    max = trees[row][col];
                    visibility[row, col] = true;
                }
            }
        }
        Print(visibility);

        for (int col = 0; col < trees.Length; col++)
        {
            max = -1;
            for (int row = trees.Count() - 1; row >= 0; row--)
            {
                if (max < trees[row][col])
                {
                    max = trees[row][col];
                    visibility[row, col] = true;
                }
            }
        }
        Print(visibility);

        int count = 0;
        for (int row = 0; row < trees.Count(); row++)
        {
            for (int col = 0; col < trees.Length; col++)
            {
                if (visibility[row, col] == true)
                {
                    count++;
                }
            }
        }
        Print(visibility);

        return count;
    }

    void Print(bool?[,] arr)
    {
        Console.WriteLine("--");
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                Console.Write(arr[x, y] switch { true => "V", false => " ", null => "?" });
            }
            Console.WriteLine();
        }
    }

    void Print(int[,] arr)
    {
        Console.WriteLine("--");
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                Console.Write($"{arr[x, y],4}");
            }
            Console.WriteLine();
        }
    }

    [TestCase("day08.input", ExpectedResult = 474606)]
    [TestCase("day08example1.input", ExpectedResult = 8)]
    public long Part2(string input)
    {
        var trees = File.ReadAllLines(input);
        return trees.AllPositions().Select(p => ScenicScore(trees, p)).Max();
    }

    private int ScenicScore(string[] trees, Position pos)
    {
        var tree = trees[pos.X][pos.Y];
        return Direction.All4.Aggregate(1, (a, d) => a * trees.Walk(pos, d).TakeWhileInclusive(t => t < tree).Count());
    }


    //public record Position(int Row, int Col)
    //{
    //    public Position Add(Direction d) => new Position(Row + d.DeltaRow, Col + d.DeltaCol);
    //}

    //public record Direction(int DeltaRow, int DeltaCol)
    //{
    //    public static Direction N = new(1, 0);
    //    public static Direction S = new(-1, 0);
    //    public static Direction W = new(0, -1);
    //    public static Direction E = new(0, 1);
    //    public static Direction[] All = new[] { N, S, W, E };
    //}
}

public static class extensions
{
    public static IEnumerable<Position> AllPositions(this string[] arr)
    {
        for (int x = 0; x < arr.Count(); x++)
        {
            for (int y = 0; y < arr.Length; y++)
            {
                yield return new Position(x, y);
            }
        }
    }

    public static IEnumerable<T> TakeWhileInclusive<T>(this IEnumerable<T> enumerable, Predicate<T> pred)
    {
        foreach (var item in enumerable)
        {
            yield return item;
            if (!pred(item)) yield break;
        }
    }

    public static IEnumerable<char> Walk(this string[] arr, Position start, Direction delta)
    {
        var pos = start.Add(delta);

        while (pos.X >= 0 && pos.X < arr.Count() && pos.Y >= 0 && pos.Y < arr[0].Length)
        {
            yield return arr[pos.X][pos.Y];

            pos = pos.Add(delta);
        }
    }

}
