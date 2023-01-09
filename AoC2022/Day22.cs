using System.Text.RegularExpressions;

namespace AoC2022;

public class Day22
{
    [Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(22);
    }

    [SetUp]
    public void setup()
    {
        var filename = $"day22.input";
        if (File.Exists(filename))
        {
            var lines = File.ReadAllLines(filename);
        }
        r.Match("x");
    }

    Regex r = new Regex(@"(?<monkey>\w{4}): ((?<math>(?<m1>\w{4}) (?<op>[*/+-]) (?<m2>\w{4}))|(?<number>\d+))", RegexOptions.Compiled);
    [TestCase("day22.input", ExpectedResult = 26558)]
    [TestCase("day22example1.input", ExpectedResult = 6032)]
    public long Part1(string input)
    {
        var lines = File.ReadAllLines(input).ToArray();
        char[,] maze = new char[lines.Length - 2, lines.SkipLast(2).Max(l => l.Length)];
        char[,] walk = new char[lines.Length - 2, lines.SkipLast(2).Max(l => l.Length)];

        int x = 0;
        Position start = Position.Empty;
        Direction dir = Direction.E;
        foreach (var line in lines.SkipLast(2))
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                if (y >= line.Length)
                {
                    maze[x, y] = ' ';
                }
                else
                {
                    maze[x, y] = line[y];
                }
                if (x == 0 && maze[x, y] == '.' && start == Position.Empty)
                {
                    start = new Position(x, y);
                }
            }
            x++;
        }
        var instructions = ParseInstructions(lines.Last());
        return WalkMaze(maze, walk, start, instructions, Wrap1);
    }

    [TestCase("day22.input", ExpectedResult = 26558)] //32222 too low, 117010 to high, 21334 too low, 110402 not correct
    [TestCase("day22example1.input", ExpectedResult = 6032)]
    [TestCase("day22example2.input", ExpectedResult = 3305)]
    [TestCase("day22example3.input", ExpectedResult = 13213)]
    [TestCase("day22example4.input", ExpectedResult = 9452)]
    [TestCase("day22example5.input", ExpectedResult = 2238)]
    [TestCase("day22example6.input", ExpectedResult = 2438)]
    [TestCase("day22example7.input", ExpectedResult = 200052)]
    [TestCase("day22example8.input", ExpectedResult = 149370)]
    [TestCase("day22example9.input", ExpectedResult = 188200)]
    public long Part2(string input)
    {
        var lines = File.ReadAllLines(input).ToArray();
        char[,] walk = new char[lines.Length - 2, lines.SkipLast(2).Max(l => l.Length)];

        int x = 0;
        Direction dir = Direction.E;

        char[,] maze;
        Position start;
        x = ReadMaze(lines, x, out maze, out start);
        var instructions = ParseInstructions(lines.Last());
        var checkstring = string.Join("", instructions);
        checkstring.Should().Be(lines.Last());
        return WalkMaze(maze, walk, start, instructions, Wrap50);
    }

    [TestCase("day22example2.input")]
    public void Testwalk(string input)
    {
        var lines = File.ReadAllLines(input).ToArray();

        int x = 0;
        Direction dir = Direction.E;

        char[,] maze;
        Position start;
        x = ReadMaze(lines, x, out maze, out start);
        //var instructions = ParseInstructions(lines.Last());

        var starts = new[]{
            new Position(10,60), new Position(10,110),
            new Position(60,60),
            new Position(110,10), new Position(110,60),
            new Position(160,10)
        };

        var insts = new[]
        {
            "200",
            "0R200L0",
            "0R0R200L0L0",
            "0R0R0R200L0L0L0",
            "0L200R0",
            "0L0L200R0R0",
            "0L0L0L200R0R0R0"
        };
        foreach (var p in starts)
        {
            foreach (var inst in insts)
            {
                char[,] walk = new char[lines.Length - 2, lines.SkipLast(2).Max(l => l.Length)];

                var score = WalkMaze(maze, walk, p, ParseInstructions(inst), Wrap50);
                Console.WriteLine();
                Console.WriteLine($"====  start {p} inst: {inst} =====");

                score.Should().Be(CalcScore(Direction.E, p));
            }

        }
    }

    private static int ReadMaze(string[] lines, int x, out char[,] maze, out Position start)
    {
        maze = new char[lines.Length - 2, lines.SkipLast(2).Max(l => l.Length)];
        start = Position.Empty;
        foreach (var line in lines.SkipLast(2))
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                if (y >= line.Length)
                {
                    maze[x, y] = ' ';
                }
                else
                {
                    maze[x, y] = line[y];
                }
                if (x == 0 && maze[x, y] == '.' && start == Position.Empty)
                {
                    start = new Position(x, y);
                }
            }
            x++;
        }
        return x;
    }

    private long WalkMaze(
        char[,] maze,
        char[,] walk,
        Position start,
        IEnumerable<Instruction> instructions, 
        Func<char[,], Position, Direction, Position, int, (Direction dir, Position next)> wrap
        )
    {
        var cur = start;
        var dir = Direction.E;
        try
        {
            foreach (var inst in instructions)
            {
                Console.WriteLine($"exec {inst}");
                for (int i = 0; i < inst.steps; i++)
                {
                    cur.Set(walk, dir.ToChar());
                    var next = cur.Add(dir);
                    if (!next.Within(maze) || next.Get(maze) == ' ')
                    {
                        Console.Write($"Wrap from {cur} {dir.ToChar()}");
                        var curdir = dir;
                        (dir, next) = wrap(maze, cur, dir, next, 50);
                        //(dir, next) = Wrap1(maze, cur, dir, next);
                        Console.Write($" to {next} {dir.ToChar()}");

                        if (next.Within(maze)) next.Set(walk, dir.ToChar());
                        var p = next.Get(maze);
                        if (p == '#')
                        {
                            next.Set(walk, 'X');
                            Console.Write($" blocked: reset direction to {curdir.ToChar()}");
                            dir = curdir;
                        }

                      //  Print(maze, walk);
                    }
                    if (next.Within(maze))
                    {
                        var p = next.Get(maze);
                        if (p == '#')
                        {
                            break;
                        }
                        if (p == '.')
                        {
                            cur = next;
                            continue;
                        }
                        else
                        {
                            throw new Exception("help");
                        }
                    }
                    else
                    {
                        var e = new Exception($"outside cube {next}");
                        e.Data.Add("next", next);
                        throw e;
                    }
                }

                dir = inst.turn switch { 'L' => dir.TurnLeft(), 'R' => dir.TurnRight(), _ => dir };
                if (inst.turn == ' ')
                {
                    Console.WriteLine("- - - - - - - - -");
                    Console.WriteLine($"last pos {cur} direction = {dir.ToChar()} {inst.direction.ToChar()}");
                }
            }

        }
        catch (Exception e)
        {

            Console.WriteLine($"Exception {e}");
            Console.WriteLine($"current {cur} {dir.ToChar()} to {e.Data["next"]}");
            cur.Set(walk, 'X');

            Print(maze, walk);
            throw;
        }

        Print(maze, walk);
        //Print(walk);

        return CalcScore(dir, cur);
    }

    private long CalcScore(Direction dir, Position cur)
    {
        return (cur.X + 1) * 1000 + (cur.Y + 1) * 4 + DirectionValue(dir);
    }

    private static (Direction dir, Position next) Wrap1(char[,] maze, Position cur, Direction dir, Position next, int dim)
    {
        var oppositeDirection = dir.Opposite();
        var nextopp = next;
        do
        {
            next = nextopp;
            nextopp = next.Add(oppositeDirection);
        }
        while (nextopp.Within(maze) && nextopp.Get(maze) != ' ');
        return (dir, next);
    }

    private static IEnumerable<int> Boundaries(int dim)
    {
        yield return 0; // oops made mistake with indexes.
        for (int i = 0; i <= 200; i += 50)
        {
            yield return i - 1;
            yield return i;
        }
    }
    private static (Direction dir, Position next) Wrap50(char[,] maze, Position cur, Direction dir, Position next, int dim)
    {
        int[] boundaries = Boundaries(dim).ToArray();
        //wrap
        //1 -> 4 V
        if (next.X >= boundaries[2] && next.X < boundaries[4] && next.Y == boundaries[3])
        {
            next = new Position(boundaries[7] - next.X, boundaries[2]);
            dir = dir.Opposite();
        }else
        //4 -> 1 V
        if (next.X >= boundaries[6] && next.X < boundaries[8] && next.Y == boundaries[1])
        {
            next = new Position(boundaries[7] - next.X, boundaries[4]);
            dir = dir.Opposite();
        }else
        //1 -> 6 V
        if (next.X == boundaries[1] && next.Y >= boundaries[4] && next.Y < boundaries[6])
        {
            next = new Position(boundaries[6] + cur.Y, boundaries[2]);
            dir = Direction.E;
        }else
        //6 -> 1 V
        if (next.X >= boundaries[8] && next.X < boundaries[10] && next.Y == boundaries[1])
        {
            next = new Position(boundaries[2], next.X - boundaries[6]);
            dir = Direction.S;
        }else
        //2 -> 6 V
        if (next.X == boundaries[1] && next.Y >= boundaries[6] && next.Y < boundaries[8])
        {
            next = new Position(boundaries[9], next.Y - boundaries[6]);
        }else
        //6 -> 2 V
        if (next.X == boundaries[10] && next.Y >= boundaries[2] && next.Y < boundaries[4])
        {
            next = new Position(boundaries[2], next.Y + boundaries[6]);
        }else
        //2 -> 5 V
        if (next.X >= boundaries[2] && next.X < boundaries[4] && next.Y == boundaries[8])
        {
            next = new Position(boundaries[7] - next.X, boundaries[5]);
            dir = Direction.W;
        }else
        //5 -> 2 V
        if (next.X >= boundaries[6] && next.X < boundaries[8] && next.Y == boundaries[6])
        {
            next = new Position(boundaries[7] - next.X, boundaries[7]);
            dir = Direction.W;
        }else
        //2 -> 3 V
        if (next.X == boundaries[4] && next.Y >= boundaries[6] && next.Y < boundaries[8])
        {
            next = new Position(next.Y - boundaries[4], boundaries[5]);
            dir = Direction.W;
        }else
        //3 -> 2 V
        if (next.X >= boundaries[4] && next.X < boundaries[6] && next.Y == boundaries[6])
        {
            next = new Position(boundaries[3], next.X + boundaries[4]);
            dir = Direction.N;
        }else
        //3 -> 4 V
        if (next.X >= boundaries[4] && next.X < boundaries[6] && next.Y == boundaries[3])
        {
            next = new Position(boundaries[6], next.X - boundaries[4]);
            dir = Direction.S;
        }else
        //4 -> 3 V
        if (next.X == boundaries[5] && next.Y >= boundaries[2] && next.Y < boundaries[4])
        {
            next = new Position(next.Y + boundaries[4], boundaries[4]);
            dir = Direction.E;
        }else
        //5 -> 6 V
        if (next.X == boundaries[8] && next.Y >= boundaries[4] && next.Y < boundaries[6])
        {
            next = new Position(next.Y + boundaries[6], boundaries[3]);
            dir = Direction.W;
        }else
        //6 -> 5 V
        if (next.X >= boundaries[8] && next.X < boundaries[10] && next.Y == boundaries[4])
        {
            next = new Position(boundaries[7], next.X - boundaries[6]);
            dir = Direction.N;
        }
        return (dir, next);
    }

    public int DirectionValue(Direction d)
    {
        if (d == Direction.N) return 3;
        if (d == Direction.S) return 1;
        if (d == Direction.E) return 0;
        if (d == Direction.W) return 2;
        throw new Exception("unknown direction");
    }

    record Instruction(Direction direction, char turn, int steps)
    {
        public override string ToString()
         => $"{steps}{turn switch { ' ' => "", _ => turn }}";
    }

    IEnumerable<Instruction> ParseInstructions(string line)
    {
        Direction dir = Direction.E;
        int nr = 0;
        foreach (var c in line)
        {
            if (c is >= '0' and <= '9')
            {
                nr = nr * 10 + c - '0';
            }
            else
            if (c is 'L')
            {
                yield return new Instruction(dir, c, nr);
                nr = 0;
                dir = dir.TurnLeft();
            }
            else
            if (c is 'R')
            {
                yield return new Instruction(dir, c, nr);
                nr = 0;
                dir = dir.TurnRight();
            }
        }
        yield return new Instruction(dir, ' ', nr);
    }

    void Print(char[,] arr)
    {
        Console.WriteLine("--");
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                var ch = arr[x, y];
                if (ch == 0) ch = ' ';
                Console.Write($"{ch}");
            }
            Console.WriteLine();
        }
    }

    void Print(char[,] arr, char[,] walk)
    {
        Console.WriteLine("--");
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                var ch = arr[x, y];
                if (ch == 0) ch = ' ';
                var w = walk[x, y];
                if (w != 0) ch = w;
                Console.Write($"{ch}");
            }
            Console.WriteLine();
        }
    }
}
