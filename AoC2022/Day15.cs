using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace AoC2022;

public class Day15
{
    //[Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(15);
    }

    [SetUp]
    public void setup()
    {
        var lines = File.ReadAllLines("day15.input");
        r.Match("x");
    }

    Regex r = new Regex(@"Sensor at x=([\-0-9]+), y=([\-0-9]+): closest beacon is at x=([\-0-9]+), y=([\-0-9]+)", RegexOptions.Compiled);
    [TestCase("day15.input", 2000000, ExpectedResult = 5525990)]
    [TestCase("day15example1.input", 10, ExpectedResult = 26)]
    [TestCase("day15example2.input", 10, ExpectedResult = 12)]
    public int Part1(string input, int scanat)
    {
        var lines = File.ReadAllLines(input);
        var hs = new HashSet<int>();
        var beaconsAtRow = new HashSet<int>();
        foreach (var line in lines)
        {
            var m = r.Match(line);
            if (m.Success)
            {
                var sensor = new Position(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value));
                var beacon = new Position(int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value));
                if (beacon.Y == scanat) beaconsAtRow.Add(beacon.X);

                IntersectAt(sensor, beacon, hs, scanat);
            }
        }
        return hs.Count - beaconsAtRow.Count;
    }

    [TestCase("day15.input", ExpectedResult = 11756174628223)]
    [TestCase("day15example1.input", ExpectedResult = 56000011)]
    public long Part2(string input)
    {
        var lines = File.ReadAllLines(input);
        List<(Position sensor, int distance)> sensors = ParseInput(lines);

        var pairs = sensors.SelectMany(s1 => sensors.Select(s2 => (s1, s2)))
            .Where(p => Distance(p.s1.sensor, p.s2.sensor) - (p.s1.distance + p.s2.distance) == 2);

        int? a = null, b = null;
        foreach (var pair in pairs)
        {
            var sx = Math.Sign(pair.s1.sensor.X - pair.s2.sensor.X);
            var sy = Math.Sign(pair.s1.sensor.Y - pair.s2.sensor.Y);
            if (sx == sy)
            {
                a = pair.s1.sensor.X + pair.s1.sensor.Y - sx * (pair.s1.distance + 1);
            }
            else
            {
                b = pair.s1.sensor.X - pair.s1.sensor.Y - sx * (pair.s1.distance + 1);
            }
        }
        if (!a.HasValue || !b.HasValue) throw new Exception("no matches found");

        var x = (a.Value + b.Value) / 2;
        var y = (a.Value - b.Value) / 2;

        return x * 4000000L + y;
    }

    public void scratchpad()
    {
        //Draw(sensors, new Position(x, y));
        //Console.WriteLine($"x={x} y={y}");

        //Draw(matchingsensorbeacons);

        //foreach (var bc in matchingsensorbeacons)
        //{
        //    Position p1 = new Position((bc.sensor.X), (bc.sensor.Y - bc.distance - 1));
        //    Position p2 = new Position((bc.sensor.X + bc.distance + 1), (bc.sensor.Y));
        //    Position p3 = new Position((bc.sensor.X), (bc.sensor.Y + bc.distance + 1));
        //    Position p4 = new Position((bc.sensor.X - bc.distance - 1), (bc.sensor.Y));
        //    var current = p1;
        //    while (current != p2)
        //    {
        //        if (current.X >= 0 && current.X <= limit && current.Y >= 0 && current.Y <= limit)
        //        {
        //            if (!IsPointCoveredByAny(current, sensorbeacons))
        //            {
        //                Console.WriteLine($"{sw.ElapsedMilliseconds}ms1 {current.X} {current.Y}");

        //                var na5 = current.X + current.Y;
        //                var na6 = current.X - current.Y;
        //                Console.WriteLine($"a = {na5}, b = {na6}");
        //                Console.WriteLine($"x = {current.X}, y = {current.Y}");
        //                return current.X * 4000000L + current.Y;
        //            }
        //        }
        //        current = current.Add(Direction.NE);
        //    }
        //    while (current != p3)
        //    {
        //        if (current.X >= 0 && current.X <= limit && current.Y >= 0 && current.Y <= limit)
        //        {
        //            if (!IsPointCoveredByAny(current, sensorbeacons))
        //            {
        //                Console.WriteLine($"{sw.ElapsedMilliseconds}ms2 {current.X} {current.Y}");
        //                var na5 = current.X + current.Y;
        //                var na6 = current.X - current.Y;
        //                Console.WriteLine($"a = {na5}, b = {na6}");
        //                Console.WriteLine($"x = {current.X}, y = {current.Y}");
        //                return current.X * 4000000L + current.Y;
        //            }
        //        }
        //        current = current.Add(Direction.SE);
        //    }
        //    while (current != p4)
        //    {
        //        if (current.X >= 0 && current.X <= limit && current.Y >= 0 && current.Y <= limit)
        //        {
        //            if (!IsPointCoveredByAny(current, sensorbeacons))
        //            {
        //                Console.WriteLine($"{sw.ElapsedMilliseconds}ms3 {current.X} {current.Y}");
        //                var na5 = current.X + current.Y;
        //                var na6 = current.X - current.Y;
        //                Console.WriteLine($"a = {na5}, b = {na6}");
        //                Console.WriteLine($"x = {current.X}, y = {current.Y}");
        //                return current.X * 4000000L + current.Y;
        //            }
        //        }
        //        current = current.Add(Direction.SW);
        //    }
        //    while (current != p1)
        //    {
        //        if (current.X >= 0 && current.X <= limit && current.Y >= 0 && current.Y <= limit)
        //        {
        //            if (!IsPointCoveredByAny(current, sensorbeacons))
        //            {
        //                Console.WriteLine($"{sw.ElapsedMilliseconds}ms4 {current.X} {current.Y}");
        //                var na5 = current.X + current.Y;
        //                var na6 = current.X - current.Y;
        //                Console.WriteLine($"a = {na5}, b = {na6}");
        //                Console.WriteLine($"x = {current.X}, y = {current.Y}");
        //                return current.X * 4000000L + current.Y;
        //            }
        //        }
        //        current = current.Add(Direction.NW);
        //    }

        //}
        //Console.WriteLine(sw.ElapsedMilliseconds);

        //return 0;
        //foreach (var bc in sensorbeacons)
        //{

        //    Position p1 = new Position((bc.sensor.X), (bc.sensor.Y - bc.distance));
        //    Position p2 = new Position((bc.sensor.X + bc.distance), (bc.sensor.Y));
        //    Position p3 = new Position((bc.sensor.X), (bc.sensor.Y + bc.distance));
        //    Position p4 = new Position((bc.sensor.X - bc.distance), (bc.sensor.Y));

        //    foreach (var p in new[] { p1, p2, p3, p4 })
        //    {
        //        foreach (var np in Direction.All4.Select(d => p.Add(d)))
        //        {
        //            if (np.X >= 0 && np.X <= limit && np.Y >= 0 && np.Y <= limit)
        //            {
        //                if (!IsPointCoveredByAny(np, sensorbeacons))
        //                {
        //                    return np.X * 4000000 + np.Y;
        //                }
        //            }
        //        }
        //    }
        //}
    }

        private List<(Position sensor, int distance)> ParseInput(string[] lines)
    {
        var sensors = new List<(Position sensor, int distance)>();
        foreach (var line in lines)
        {
            var m = r.Match(line);
            if (m.Success)
            {
                var sensor = new Position(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value));
                var beacon = new Position(int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value));
                sensors.Add((sensor, Distance(sensor, beacon)));
            }
        }

        return sensors;
    }

    private IEnumerable<int> Intersections((Position sensor, Position beacon, int distance) bc, (Position sensor, Position beacon, int distance) b2)
    {
        //Console.WriteLine($"{d - bc.distance + b2.distance}");
        var d = Distance(bc.sensor, b2.sensor);
        var dif = d - (bc.distance + b2.distance);
        if (Math.Abs(dif) < 10)
            yield return d - (bc.distance + b2.distance);
    }

    private bool IsPointCoveredByAny(Position np, List<(Position sensor, Position beacon, int distance)> sensorbeacons)
    {
        foreach(var sb in sensorbeacons)
        {
            if (Distance(np, sb.sensor) <= sb.distance)
            {
                return true;
            }
        }
        return false;
    }
#pragma warning disable CA1416 // Validate platform compatibility

    private void Draw(List<(Position sensor, int distance)> sensors, Position distressPosition)
    {
        int width = 4000;
        int height = 4000;
        int factor = 1000;
        var bm = new Bitmap(width, height);
        using (var graphics = Graphics.FromImage(bm))
        {
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.CompositingMode = CompositingMode.SourceCopy;
            
            DrawSensors(sensors, factor, graphics);

            var pairs = sensors.SelectMany(s1 => sensors.Select(s2 => (s1, s2)))
                .Where(p => Distance(p.s1.sensor, p.s2.sensor) - (p.s1.distance + p.s2.distance) == 2);

            List<(Position sensor, int distance)> matchingsensors = new();
            foreach (var pair in pairs)
            {
                matchingsensors.Add(pair.s1);
                matchingsensors.Add(pair.s2);
            }
            DrawSensors(matchingsensors, factor, graphics);

            var beaconPen1 = new Pen(Color.Black);
            var beaconPen2 = new Pen(Color.White);
            graphics.DrawRectangle(beaconPen2, (distressPosition.X / factor) - 5, (distressPosition.Y / factor) - 5, 10, 10);
            graphics.DrawRectangle(beaconPen1, (distressPosition.X / factor) - 6, (distressPosition.Y / factor) - 6, 12, 12);
            graphics.DrawRectangle(beaconPen2, (distressPosition.X / factor) - 7, (distressPosition.Y / factor) - 7, 14, 14);
            //var linePen2 = new Pen(Color.White, 2.0f);
            //graphics.DrawLine(linePen2, new Point(distressPosition.X / factor + 10, distressPosition.Y / factor - 10), new Point((distressPosition.X + distressPosition.Y) / factor, 0));
            //graphics.DrawLine(linePen2, new Point(distressPosition.X / factor - 10, distressPosition.Y / factor - 10), new Point((distressPosition.X - distressPosition.Y) / factor, 0));

            bm.Save($"world1.png", ImageFormat.Png);
        }
    }

    private static void DrawSensors(List<(Position sensor, int distance)> sensors, int factor, Graphics graphics)
    {
        var sensorPen = new Pen(Color.White);
        var linePen = new Pen(Color.Red);
        var brush = (SolidBrush)Brushes.DarkGray;

        int r = 0;
        int g = 0;
        int b = 0;

        foreach (var bc in sensors)
        {
            brush.Color = Color.FromArgb(r % 128 + 128, g % 128 + 128, b % 128 + 128);
            r += 5;
            g += 11;
            b += 37;
            Point p1 = new Point((bc.sensor.X) / factor, (bc.sensor.Y - bc.distance) / factor);
            Point p2 = new Point((bc.sensor.X + bc.distance) / factor, (bc.sensor.Y) / factor);
            Point p3 = new Point((bc.sensor.X) / factor, (bc.sensor.Y + bc.distance) / factor);
            Point p4 = new Point((bc.sensor.X - bc.distance) / factor, (bc.sensor.Y) / factor);
            graphics.FillPath(brush, new GraphicsPath(
                new[] { p1, p2, p3, p4 },
                new byte[] { (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line }));
        }
        foreach (var bc in sensors)
        {
            linePen.Color = Color.FromArgb(r % 128 + 32, g % 128 + 32, b % 128 + 32);
            r += 11;
            g += 7;
            b += 5;
            graphics.DrawRectangle(sensorPen, (bc.sensor.X / factor) - 2, (bc.sensor.Y / factor) - 2, 4, 4);
            Point p1 = new Point((bc.sensor.X) / factor, (bc.sensor.Y - bc.distance) / factor);
            Point p2 = new Point((bc.sensor.X + bc.distance) / factor, (bc.sensor.Y) / factor);
            Point p3 = new Point((bc.sensor.X) / factor, (bc.sensor.Y + bc.distance) / factor);
            Point p4 = new Point((bc.sensor.X - bc.distance) / factor, (bc.sensor.Y) / factor);
            graphics.DrawLine(linePen, p1, p2);
            graphics.DrawLine(linePen, p2, p3);
            graphics.DrawLine(linePen, p3, p4);
            graphics.DrawLine(linePen, p4, p1);
        }
    }
#pragma warning restore CA1416 // Validate platform compatibility

    private void IntersectAt(Position sensor, Position beacon, HashSet<int> hs, int y)
    {
        int dist = Distance(sensor, beacon);
        int distToY = Math.Abs(sensor.Y - y);
        if (distToY > dist) return;

        var d = dist - distToY;
        for (int x = -d; x <= d; x++)
        {
            hs.Add(sensor.X + x);
        }
    }

    private void IntersectAt(Position sensor, Position beacon, HashSet<int> hs, int y, int limit)
    {
        int dist = Distance(sensor, beacon);
        int distToY = Math.Abs(sensor.Y - y);
        if (distToY > dist) return;

        var d = dist - distToY;
        var line = (dist - distToY) * 2 + 1;
        for (int x = -d; x <= d; x++)
        {
            if (sensor.X + x <= limit)
            {
                hs.Add(sensor.X + x);
            }
        }
    }

    private int Distance(Position sensor, Position beacon)
    {
        var dx = Math.Abs(sensor.X - beacon.X);
        var dy = Math.Abs(sensor.Y - beacon.Y);
        return dx + dy;
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
