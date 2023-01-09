using System.Text.RegularExpressions;
using static AoC2022.Day07;

namespace AoC2022;

public class Day24
{
    [Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(24);
    }

    [TestCase("day24.input", ExpectedResult = 232)]
    //[TestCase("day24example1.input", ExpectedResult = 110)]
    [TestCase("day24example2.input", ExpectedResult = 18)]
    public long Part1(string input)
    {
        var lines = File.ReadAllLines(input).ToArray();
        var maze = new byte[lines.Length, lines.Max(l => l.Length)];

        int x = 0;
        foreach (var line in lines)
        {
            for (int y = 0; y < line.Length; y++)
            {
                maze[x, y] = Process(line[y]);
            }
            x++;
        }
        var start = new Position(0, 1);
        Print(maze, start);

        var mazes = new List<byte[,]> { maze };
        var goal = new Position(maze.GetLength(0) - 1, maze.GetLength(1) - 2);

        var t1 = WalkTilGoal(maze, 0, start, goal, mazes);
        return t1;
    }

    [TestCase("day24.input", ExpectedResult = 715)]
    //[TestCase("day24example1.input", ExpectedResult = 110)]
    [TestCase("day24example2.input", ExpectedResult = 54)]
    public long Part2(string input)
    {
        var lines = File.ReadAllLines(input).ToArray();
        var maze = new byte[lines.Length, lines.Max(l => l.Length)];

        int x = 0;
        foreach (var line in lines)
        {
            for (int y = 0; y < line.Length; y++)
            {
                maze[x, y] = Process(line[y]);
            }
            x++;
        }
        var start = new Position(0, 1);
        Print(maze, start);

        var mazes = new List<byte[,]> { maze };
        var goal = new Position(maze.GetLength(0) - 1, maze.GetLength(1) - 2);

        var t1 = WalkTilGoal(maze, 0, start, goal, mazes);
        var t2 = WalkTilGoal(maze, t1, goal, start, mazes);
        var t3 = WalkTilGoal(maze, t2, start, goal, mazes);
        return t3;
    }

    private int WalkTilGoal(byte[,] maze, int t, Position start, Position goal, List<byte[,]> mazes)
    {
        var q = new PriorityQueue<S, int>();
        var memo = new HashSet<S>();
        var score = Dist(start, goal);
        var startstate = new S(start, t);
        q.Enqueue(startstate, score);

        bool done = false;
        int minsteps = int.MaxValue;
        while (!done && q.TryDequeue(out var cur, out int prio))
        {
            foreach (var dir in Direction.All5)
            {
                var np = cur.p.Add(dir);
                var nt = cur.turn + 1;
                var newstate = new S(np, cur.turn + 1);
                var curmaze = GetMaze(nt, mazes);
                if (np == goal && minsteps > nt)
                {
                    Print(curmaze, newstate);
                    minsteps = nt;
                }
                if (np.Within(maze) && np.Get(curmaze) == 0)
                {
                    var nscore = nt + Dist(np, goal);
                    if (!memo.Contains(newstate) && nscore < minsteps)
                    {
                        memo.Add(newstate);
                        q.Enqueue(newstate, nscore);
                    }
                }
            }
        }
        return minsteps;
    }

    record S(Position p, int turn);

    int Dist(Position start, Position goal)
    {
        return Math.Abs(goal.X - start.X) + Math.Abs(goal.Y - start.Y);
    }

    byte[,] GetMaze(int t, List<byte[,]> mazes)
    {
        while (t >= mazes.Count)
        {
            mazes.Add(GenerateNext(mazes[^1]));
        }
        return mazes[t];
    }

    private byte[,] GenerateNext(byte[,] maze)
    {
        var newmaze = new byte[maze.GetLength(0), maze.GetLength(1)];
        foreach (var dir in Direction.All4)
        {
            var mask = Process(dir.ToChar());
            foreach (var pos in maze.Each())
            {
                var newpos = pos.Add(dir);
                var b = pos.Get(maze);
                if ((b & mask) == mask)
                {
                    if (!newpos.Add(dir).Within(newmaze))
                    {
                        // flip to other side
                        if (dir.Dx == 0)
                        {
                            newpos = new Position(pos.X, maze.GetLength(1) - pos.Y - 1);
                        }
                        else if (dir.Dy == 0)
                        {
                            newpos = new Position(maze.GetLength(0) - pos.X - 1, pos.Y);
                        }
                    }
                    newpos.Set<byte>(newmaze, (byte)(newpos.Get(newmaze) | mask));
                }
                else if (Process(b) is '#')
                {
                    pos.Set(newmaze, b);
                }
            }
        }
        return newmaze;
    }

    Direction Next(Direction dir)
    {
        if (dir == Direction.N) return Direction.S;
        if (dir == Direction.S) return Direction.W;
        if (dir == Direction.W) return Direction.E;
        if (dir == Direction.E) return Direction.N;
        throw new Exception("no dir");
    }

    void Print(byte[,] arr, Position p)
    {
        Console.WriteLine("--");
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                var b = arr[x, y];
                if (p.X == x && p.Y == y)
                {
                    Console.Write($"E");
                }
                else
                {
                    Console.Write($"{Process(b)}");
                }
            }
            Console.WriteLine();
        }
    }

    void Print(byte[,] arr, S s)
    {
        Console.WriteLine($"-- {s.turn} --");
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                var b = arr[x, y];
                if (s.p.X == x && s.p.Y == y)
                {
                    Console.Write($"E");
                }
                else
                {
                    Console.Write($"{Process(b)}");
                }
            }
            Console.WriteLine();
        }
    }

    byte Process(char x)
    {
        return (x switch
        {
            '>' => 1 << 0,
            'v' => 1 << 1,
            '<' => 1 << 2,
            '^' => 1 << 3,
            '.' => 0,
            '#' => 1 << 4
        });
    }

    char Process(byte x)
    {
        return (x switch
        {
            1 << 0 => '>',
            1 << 1 => 'v',
            1 << 2 => '<',
            1 << 3 => '^',
            0 => '.',
            1 << 4 => '#',
            3 or 5 or 6 or 9 or 10 or 12 => '2',
            15 => '4',
            _ => '3'
        });
    }
}
