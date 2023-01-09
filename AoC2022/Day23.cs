using System.Text.RegularExpressions;
using static AoC2022.Day07;

namespace AoC2022;

public class Day23
{
    [Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(23);
    }

    [TestCase("day23.input", ExpectedResult = 4116)]
    [TestCase("day23example1.input", ExpectedResult = 110)]
    [TestCase("day23example2.input", ExpectedResult = 25)]
    public long Part1(string input)
    {
        var maxsteps = 1000;
        var lines = File.ReadAllLines(input).ToArray();
        char[,] elves = new char[lines.Length+2*maxsteps, lines.Max(l => l.Length) + 2 * maxsteps];
        char[,] propose = new char[lines.Length + 2 * maxsteps, lines.Max(l => l.Length) + 2 * maxsteps];
        Direction dir = Direction.N;
        {
            int x = 0;
            foreach (var line in lines)
            {
                for (int y = 0; y < line.Length; y++)
                {
                    elves[x + maxsteps, y + maxsteps] = line[y];
                }
                x++;
            }
        }
        Print(elves, propose);
        for (int turn = 0; turn < maxsteps; turn++)
        {
            int moved = 0;
            Console.WriteLine($"Turn {turn}");
            foreach (var pos in elves.Each())
            {
                var e = pos.Get(elves);
                if (e == '#')
                {
                    if (elves.NeighboursOf(pos, Direction.All8).Any(p => p.Get(elves) == '#'))
                    {
                        pos.Set(propose, '#');
                        var curdir = dir;
                        for (int i = 0; i < 4; i++)
                        {
                            if (curdir.GetSame().All(p => pos.Add(p).Get(elves) != '#'))
                            {
                                var consider = pos.Add(curdir);
                                if (consider.Get(propose) != '#')
                                {
                                    consider.Set(propose, '#');
                                    pos.Set(propose, '.');
                                    moved++;
                                }
                                else // backoff, position was already proposed by other elves
                                {
                                    moved--;
                                    consider.Set(propose, '.');
                                    pos.Set(propose, '#');
                                    consider.Add(curdir).Set(propose, '#');
                                }
                                break;
                            }
                            curdir = Next(curdir);
                        }
                    }
                    else
                    {
                        pos.Set(propose, '#');
                    }
                }
            }
            var old = elves;
            elves = propose;
            propose = old.Init(' ');

         //   Print(elves);
            if (dir == Direction.N) dir = Direction.S; else
            if (dir == Direction.S) dir = Direction.W; else
            if (dir == Direction.W) dir = Direction.E; else
            if (dir == Direction.E) dir = Direction.N;
            if (moved == 0) throw new Exception($"{turn}");
        }

        var minx = elves.Each().Where(p => p.Get(elves) == '#').Min(e => e.X);
        var miny = elves.Each().Where(p => p.Get(elves) == '#').Min(e => e.Y);
        var maxx = elves.Each().Where(p => p.Get(elves) == '#').Max(e => e.X);
        var maxy = elves.Each().Where(p => p.Get(elves) == '#').Max(e => e.Y);

        var cnt = 0;
        {
            for (int x = minx; x <= maxx; x++)
            {
                for (int y = miny; y <= maxy; y++)
                {
                    if (elves[x, y] != '#')
                    {
                        cnt++;
                    }
                }
            }
        }
        return cnt;

    }


    Direction Next(Direction dir)
    {
        if (dir == Direction.N) return Direction.S;
        if (dir == Direction.S) return Direction.W;
        if (dir == Direction.W) return Direction.E;
        if (dir == Direction.E) return Direction.N;
        throw new Exception("no dir");
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
    void Print(char[,] arr)
    {
        Console.WriteLine("--");
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                var ch = arr[x, y];
                if (ch == 0) ch = ' ';
                if (ch == ' ') ch = '.';
                Console.Write($"{ch}");
            }
            Console.WriteLine();
        }
    }

}
