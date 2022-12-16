using System.Text.RegularExpressions;

namespace AoC2022;

public class Day14
{
    //[Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(14);
    }

    [TestCase("day14.input", ExpectedResult = 412)] //200 too low 
    [TestCase("day14example1.input", ExpectedResult = 24)]
    public int Part1(string input)
    {
        var lines = File.ReadAllLines(input);

        var rocks = lines.Select(l => GetPositions(l).ToArray());
        int minx = Math.Min(500, rocks.Select(l => l.Min(p => p.X)).Min());
        int miny = Math.Min(0, rocks.Select(l => l.Min(p => p.Y)).Min());
        int maxx = Math.Max(500, rocks.Select(l => l.Max(p => p.X)).Max());
        int maxy = Math.Max(0, rocks.Select(l => l.Max(p => p.Y)).Max());

        var offset = new Direction(-minx, -miny);
        var source = new Position(500, 0);
        char[,] cave = new char[maxx - minx + 1, maxy - miny + 1].Init('.');
        cave.Set(source.Add(offset), '+');

        foreach (var rock in rocks)
        {
            for (int i = 1; i < rock.Length; i++)
            {
                var from = rock[i - 1];
                var to = rock[i];
                Draw(cave, from, to, minx, miny);
            }
        }
        Print(cave);

        int sands = 0;
        try
        {
            while (true)
            {
                Position current = new Position(500, 0);
                do
                {
                    if (cave.Get(current.Add(Direction.E).Add(offset)) == '.')
                    {
                        current = current.Add(Direction.E);
                        //n = current.Add(Direction.SE);
                    }
                    else
                    if (cave.Get(current.Add(Direction.SE).Add(offset)) == '.')
                    {
                        current = current.Add(Direction.SE);
                    }
                    else
                    if (cave.Get(current.Add(Direction.NE).Add(offset)) == '.')
                    {
                        current = current.Add(Direction.NE);
                    }
                    else
                    {
                        break;
                    }

                }
                while (true);
                sands++;
                cave.Set(current.Add(offset), 'o');
                if (sands % 20 == 0)Print(cave);
              //  if (sands == 200) break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Print(cave);

        return sands;
    }

    [TestCase("day14.input", ExpectedResult = 412)] //200 too low 
    [TestCase("day14example1.input", ExpectedResult = 24)]
    public int Part2(string input)
    {
        var lines = File.ReadAllLines(input);

        var rocks = lines.Select(l => GetPositions(l).ToArray());
        int minx = Math.Min(500, rocks.Select(l => l.Min(p => p.X)).Min());
        int miny = Math.Min(0, rocks.Select(l => l.Min(p => p.Y)).Min());
        int maxx = Math.Max(500, rocks.Select(l => l.Max(p => p.X)).Max());
        int maxy = Math.Max(0, rocks.Select(l => l.Max(p => p.Y)).Max());

        maxy += 2;
        minx = Math.Min(500 - maxy, minx);
        maxx = Math.Max(500 + maxy, minx);

        var offset = new Direction(-minx, -miny);
        char[,] cave = new char[maxx - minx + 1, maxy - miny + 1].Init('.');
        var source = new Position(500, 0);
        cave.Set(source.Add(offset), '+');

        foreach (var rock in rocks)
        {
            for (int i = 1; i < rock.Length; i++)
            {
                var from = rock[i - 1];
                var to = rock[i];
                Draw(cave, from, to, minx, miny);
            }
        }
        Draw(cave, new Position(minx,maxy), new Position(maxx, maxy), minx, miny);

        Print(cave);

        int sands = 0;
        try
        {
            while (true)
            {
                Position current = new Position(500, 0);
                do
                {
                    if (cave.Get(current.Add(Direction.E).Add(offset)) == '.')
                    {
                        current = current.Add(Direction.E);
                        //n = current.Add(Direction.SE);
                    }
                    else
                    if (cave.Get(current.Add(Direction.SE).Add(offset)) == '.')
                    {
                        current = current.Add(Direction.SE);
                    }
                    else
                    if (cave.Get(current.Add(Direction.NE).Add(offset)) == '.')
                    {
                        current = current.Add(Direction.NE);
                    }
                    else
                    {
                        if (current == new Position(500, 0))
                        {
                            Print(cave);
                            return sands +1;
                        }

                        break;
                    }

                }
                while (true);
                sands++;
                cave.Set(current.Add(offset), 'o');
                if (sands % 20 == 0) Print(cave);
                //  if (sands == 200) break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Print(cave);

        return sands;
    }

    void Print(char[,] arr)
    {
        Console.WriteLine("--");
        for (int y = 0; y < arr.GetLength(1); y++)
        {
            for (int x = 0; x < arr.GetLength(0); x++)
            {
                Console.Write(arr[x,y]);
            }
            Console.WriteLine();
        }
    }

    private void Draw(char[,] cave, Position from, Position to, int offsetx, int offsety)
    {
        if (from.X == to.X)
        {
            var fromY = Math.Min(from.Y, to.Y);
            var toY = Math.Max(from.Y, to.Y);
            for (int y = fromY; y <= toY; y++)
            {
                cave[from.X - offsetx, y - offsety] = '|';
            }
        }
        else
        if (from.Y == to.Y)
        {
            var fromX = Math.Min(from.X, to.X);
            var toX = Math.Max(from.X, to.X);
            for (int x = fromX; x <= toX; x ++)
            {
                cave[x - offsetx, from.Y - offsety] = '-';
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private IEnumerable<Position> GetPositions(string line)
    {
        var pairs = line.Split(" -> ").Select(ps => ps.Split(",").Select(c => int.Parse(c)));
        foreach (var pair in pairs)
        {
            var position = new Position(pair.First(), pair.Skip(1).First());
            yield return position;
        }
    }

}
