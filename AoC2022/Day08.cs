using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AoC2022;

public class Day08
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
                Console.Write(arr[x, y] switch { true=> "V", false=>" ", null=>"?"});
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
        var scores = new int[trees.Count(), trees.Length];

        var maxscore = 0;
        for (int row = 0; row < trees.Count(); row++)
        {
            for (int col = 0; col < trees.Length; col++)
            {
                var scenic = ScenicScore(trees, row, col);
                scores[row, col] = scenic;
                maxscore = Math.Max(scenic, maxscore);
            }
        }
        Print(scores);

        return maxscore;
    }

    private int ScenicScore(string[] trees, int row, int col)
    {
        var tree = trees[row][col];

        int[] score = new int[4];
        for (int r = row + 1; r < trees.Count(); r++)
        {
            score[0]++;
            if (trees[r][col] >= tree)
            {
                break;
            }
        }

        for (int r = row - 1; r >= 0; r--)
        {
            score[1]++;
            if (trees[r][col] >= tree)
            {
                break;
            }
        }

        for (int c = col + 1; c < trees[0].Length; c++)
        {
            score[2]++;
            if (trees[row][c] >= tree)
            {
                break;
            }
        }

        for (int c = col - 1; c >= 0; c--)
        {
            score[3]++;
            if (trees[row][c] >= tree)
            {
                break;
            }
        }
        return score[0] * score[1] * score[2] * score[3];
    }
}

