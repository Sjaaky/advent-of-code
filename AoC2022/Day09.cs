using System.Configuration;

namespace AoC2022;

public class Day09
{
    //[Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(9);
    }

    [TestCase("day09example1.input", 20, 20, 10, 10,  ExpectedResult = 13)]
    [TestCase("day09.input", 800,800, 600,600, ExpectedResult = 5513)]
    public long Part1(string input, int size1, int size2, int pos1, int pos2)
    {
        var lines = File.ReadAllLines(input);

        int[,] field = new int[size1, size2];
        Position start = new(pos1, pos2);
        Position head = start;
        Position tail = head;
        foreach (var line in lines)
        {
            var dir = line[0] switch
            {
                'U' => Direction.N,
                'D' => Direction.S,
                'L' => Direction.W,
                'R' => Direction.E
            };

            var steps = int.Parse(line[2..]);

            for (int i = 0; i < steps; i++)
            {
                head = head.Add(dir);
                tail = Follow(head, tail);
                field[tail.X, tail.Y]++;
              ///  Arr.Print(field);
            }

        }
        var sum = Count(field, i => i > 0);
        return sum;
    }
    
    [TestCase("day09example1.input", 20, 20, 10, 10, ExpectedResult = 1)]
    [TestCase("day09.input", 800, 800, 600, 600, ExpectedResult = 2427)]
    public long Part2(string input, int size1, int size2, int pos1, int pos2)
    {
        var lines = File.ReadAllLines(input);

        int[,] field = new int[size1, size2];
        Position[] knot = new Position[10];
        for (int i = 0; i < 10; i++)
        {
            knot[i] = new(pos1, pos2);
        }

        foreach (var line in lines)
        {
            var dir = line[0] switch
            {
                'U' => Direction.N,
                'D' => Direction.S,
                'L' => Direction.W,
                'R' => Direction.E
            };

            var steps = int.Parse(line[2..]);

            for (int i = 0; i < steps; i++)
            {
                knot[0] = knot[0].Add(dir);
                for (int j = 1; j < 10; j++)
                    knot[j] = Follow(knot[j - 1], knot[j]);
                field[knot[9].X, knot[9].Y]++;
            }
        }
        var sum = Count(field, i => i > 0);
        return sum;
    }


    int Count(int[,] arr, Predicate<int> pred)
    {
        int count = 0;
        for (int r = 0; r < arr.GetLength(0); r++)
        {
            for (int c = 0; c < arr.GetLength(1); c++)
            {
                if (pred(arr[r, c])) count++;
            }
        }
        return count;
    }

    Position Follow(Position head, Position tail)
    {
        var dc = head.Y - tail.Y;
        var dr = head.X - tail.X;

        var uc = dc switch { < 0 => -1, > 0 => 1, 0 => 0 };
        var ur = dr switch { < 0 => -1, > 0 => 1, 0 => 0 };
        var newtail = new Position(tail.X + ur, tail.Y + uc);
        if (newtail == head)
            return tail;
        else
            return newtail;
    }


}

