using static AoC2022.Day07;
using static System.Net.Mime.MediaTypeNames;

namespace AoC2022;

public class Day12
{
    //[Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(12);
    }

    [TestCase("day12example1.input", ExpectedResult = 31)]
    [TestCase("day12.input", ExpectedResult = 412)]
    public int Part1(string input)
    {
        var lines = File.ReadAllLines(input);
        char[,] hill = new char[lines.Length, lines[0].Length];
        int[,] distance = new int[lines.Length, lines[0].Length].Init(int.MaxValue);
        Position start = Position.Empty;
        Position end = Position.Empty;

        foreach (var pos in lines.ForEachCharacter())
        {
            hill.Set(pos, lines.Get(pos));
            if (hill.Get(pos) == 'S')
            {
                distance.Set(pos, 0);
                start = pos;
                hill.Set(pos, 'a');
            }
            if (hill.Get(pos) == 'E')
            {
                end = pos;
                hill.Set(pos, 'z');
            }
        }

        if (start == Position.Empty) throw new Exception("no start found");
        if (end == Position.Empty) throw new Exception("no end found");

        Stack<Position> todo = new Stack<Position>();
        todo.Push(start);
        var steps = 0;
        Print(hill);

        int print = 0;
        while (todo.Any())
        {
            var current = todo.Pop();
            var c = hill[current.X, current.Y];
            var currentdistance = distance[current.X, current.Y];

            if (current == end)
            {
                steps = distance.Get(current);
            }
            if (print++ % 100 == 0)
            {
                //     Print("distance", distance, hill, end);
            }

            foreach (var next in hill.NeighboursOf(current, Direction.All4))
            {
                if ((hill.Get(next) <= c + 1)
                    && distance.Get(next) > currentdistance + 1)
                {
                    todo.Push(next);
                    distance.Set(next, currentdistance + 1);
                }
            }
        }
        Print("distance", distance, hill, end);

        return steps;
    }

    [TestCase("day12example1.input", ExpectedResult = 29)]
    [TestCase("day12.input", ExpectedResult = 402)]
    public int Part2(string input)
    {
        var lines = File.ReadAllLines(input);
        char[,] hill = new char[lines.Length, lines[0].Length];
        int[,] distance = new int[lines.Length, lines[0].Length].Init(int.MaxValue);
        Position start = Position.Empty;

        foreach (var pos in lines.ForEachCharacter())
        {
            hill.Set(pos, lines.Get(pos));
            if (hill.Get(pos) == 'S')
            {
                hill.Set(pos, 'a');
            }
            if (hill.Get(pos) == 'E')
            {
                start = pos;
                distance.Set(pos, 0);
                hill.Set(pos, 'z');
            }
        }

        if (start == Position.Empty) throw new Exception("no start found");

        Stack<Position> todo = new Stack<Position>(new[] { start });
        var steps = int.MaxValue;
        Print(hill);

        int print = 0;
        while (todo.Any())
        {
            var current = todo.Pop();
            var currentdistance = distance.Get(current);

            if (hill.Get(current) == 'a')
            {
                steps = Math.Min(steps, currentdistance);
            }

            if (print++ % 100 == 0) 
            {
                //Print("distance", distance, hill, end);
            }

            foreach (var next in hill.NeighboursOf(current, Direction.All4))
            {
                if ((hill.Get(next) >= hill.Get(current) - 1)
                    && distance.Get(next) > currentdistance + 1)
                {
                    todo.Push(next);
                    distance.Set(next, currentdistance + 1);
                }
            }
        }
        Print("distance", distance, hill);

        return steps;
    }

    void Print(char[,] arr)
    {
        Console.WriteLine("--");
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                Console.Write(arr[x, y]);
            }
            Console.WriteLine();
        }
    }

    void Print(string title, int[,] arr, char[,] hill, Position? end = null)
    {
        Console.WriteLine($"-- {title} --");
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                if (x == end?.X && y == end?.Y)
                {
                    Console.Write($" END,");
                }
                else
                {
                    if (arr[x, y] == int.MaxValue)
                    {
                        Console.Write($" _ {hill[x,y]},");
                    }
                    else
                    {
                        Console.Write($"{arr[x, y] ,3}{hill[x, y]},");
                    }
                }
            }
            Console.WriteLine();
        }
    }


}


