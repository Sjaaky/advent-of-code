namespace AoC2022;


public record Position(int X, int Y)
{
    public static Position Empty { get; } = new Position(int.MinValue, int.MinValue);

    public Position Add(Direction d) => new Position(X + d.Dx, Y + d.Dy);
    public Position Sub(Direction d) => new Position(X - d.Dx, Y - d.Dy);

    public char Get(string[] lines)
     => lines[X][Y];

    public T Get<T>(T[,] arr)
     => arr[X, Y];

    public void Set<T>(T[,] arr, T v)
     => arr[X, Y] = v;

    public bool Within<T>(T[,] arr)
     => X >= 0 && X < arr.GetLength(0) && Y >= 0 && Y < arr.GetLength(1);
}

public record Position3(int X, int Y, int Z)
{
    public static Position3 Empty { get; } = new Position3(int.MinValue, int.MinValue, int.MinValue);

    public Position3 Add(Direction3 d) => new Position3(X + d.Dx, Y + d.Dy, Z + d.Dz);
    public Position3 Sub(Direction3 d) => new Position3(X - d.Dx, Y - d.Dy, Z - d.Dz);

    public T Get<T>(T[,,] arr)
     => arr[X, Y, Z];

    public void Set<T>(T[,,] arr, T v)
     => arr[X, Y, Z] = v;

    public bool Within<T>(T[,,] arr)
     => X >= 0 && X < arr.GetLength(0) 
        && Y >= 0 && Y < arr.GetLength(1)
        && Z >= 0 && Z < arr.GetLength(2);
}

public static class ArrExtensions
{
    public static IEnumerable<Position> NeighboursOf<T>(this T[,] arr, Position current, IEnumerable<Direction> directions)
     => directions.Select(d => current.Add(d)).Where(p => p.Within(arr));

    public static IEnumerable<Position> ForEachCharacter(this string[] lines)
    {
        for (int x = 0; x < lines.Length; x++)
        {
            for (int y = 0; y < lines[x].Length; y++)
            {
                yield return new Position(x, y);
            }
        }
    }

    public static IEnumerable<Position> ForEach<T>(this T[,] arr)
    {
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                yield return new Position(x, y);
            }
        }
    }

    public static char Get(this string[] lines, Position p) => lines[p.X][p.Y];

    public static void Set<T>(this T[,] arr, Position p, T v) => arr[p.X, p.Y] = v;
    public static T Get<T>(this T[,] arr, Position p) => arr[p.X, p.Y];

    public static T[,] Init<T>(this T[,] arr, T v)
    {
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                arr[x, y] = v;
            }
        }
        return arr;
    }
}


public record Direction(int Dx, int Dy)
{
    public static Direction N = new(-1, 0);
    public static Direction NW = new(-1, -1);
    public static Direction NE = new(-1, 1);
    public static Direction S = new(1, 0);
    public static Direction SW = new(1, -1);
    public static Direction SE = new(1, 1);
    public static Direction W = new(0, -1);
    public static Direction E = new(0, 1);
    
    public static Direction[] All4 = new[] { N, E, S, W };
    public static Direction[] All8 = new[] { N, NE, E, SE, S, SW, W, NW };

    public Direction TurnLeft()
    {
        if (this == N) return W;
        if (this == W) return S;
        if (this == S) return E;
        if (this == E) return N;
        throw new Exception("unknown direction");
    }
    public Direction TurnRight()
    {
        if (this == N) return E;
        if (this == W) return N;
        if (this == S) return W;
        if (this == E) return S;
        throw new Exception("unknown direction");
    }

    public Direction Opposite()
    {
        if (this == N) return S;
        if (this == S) return N;
        if (this == E) return W;
        if (this == W) return E;
        throw new Exception("unknown direction");
    }

    public char ToChar()
    {
        if (this == N) return '^';
        if (this == S) return 'v';
        if (this == E) return '>';
        if (this == W) return '<';
        throw new Exception("unknown direction");
    }

}

public record Direction3(int Dx, int Dy, int Dz)
{
    public static Direction3 N = new(1, 0, 0);
    public static Direction3 NW = new(1, -1, 0);
    public static Direction3 NE = new(1, 1, 0);
    public static Direction3 S = new(-1, 0, 0);
    public static Direction3 SW = new(-1, -1, 0);
    public static Direction3 SE = new(-1, 1, 0);
    public static Direction3 W = new(0, -1, 0);
    public static Direction3 E = new(0, 1, 0);
    public static Direction3 D = new(0, 0, -1);
    public static Direction3 U = new(0, 0, 1);

    public static Direction3[] All6 = new[] { N, E, S, W, D, U };
}


public static class Arr
{
    public static void Print(int[,] arr)
    {
        Console.WriteLine("--");
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                Console.Write($"{arr[x, y],2}");
            }
            Console.WriteLine();
        }
    }
}