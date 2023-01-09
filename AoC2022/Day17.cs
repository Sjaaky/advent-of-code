using System.Text;
using System.Text.RegularExpressions;

namespace AoC2022;

public class Day17
{
    [Test]
    public async Task Fetch()
    {
        await FetchInput.Fetch(17);
    }

    [TestCase("day17.input", ExpectedResult = 3111)] //200 too low 
    [TestCase("day17example1.input", ExpectedResult = 3068)]
    public int Part1(string input)
    {
        var instructions = File.ReadAllLines(input).First();
        var blocks = File.ReadAllLines("day17.blocks").GroupedMapReduce(l => l == "", c => c, c => c).ToList();

        var chamber = RunSimulation(instructions, blocks, 2022);
        return chamber.Height;
    }

    [TestCase("day17.input", ExpectedResult = 1526744186042L)] //200 too low 
    [TestCase("day17example1.input", ExpectedResult = 1514285714288L)]
    public long Part2(string input)
    {
        var instructions = File.ReadAllLines(input).First();
        var blocks = File.ReadAllLines("day17.blocks").GroupedMapReduce(l => l == "", c => c, c => c).ToList();

        var chamber = RunSimulation(instructions, blocks, 5000);

        bool repetitionFound = false;
        int i1 = 0;
        int i2 = 0;
        for (i1 = 1; i1 < chamber.Height; i1++)
        {
            for (i2 = i1 + 10; i2 < chamber.Height; i2++)
            {
                repetitionFound = true;
                for (int dx = 0; dx < i2 - i1; dx++)
                {
                    repetitionFound = Compareline(chamber.chamber[i1 + dx], chamber.chamber[i2 + dx]);
                    if (!repetitionFound) break;
                }
                    
                if (repetitionFound) break;
            }
            if (repetitionFound) break;
        }
        if (repetitionFound)
        {
            Console.WriteLine($"pattern {i1} {chamber.rocks[i1]} {i2} {chamber.rocks[i2]} {i2 - i1}: {chamber.rocks[i2] - chamber.rocks[i1]}");
            long repHeight = i2 - i1;
            long repRocks = chamber.rocks[i2] - chamber.rocks[i1];

            long rocks = 1000000000000L;
            long height = i1;
            rocks -= chamber.rocks[i1];
            long repetitions = rocks / repRocks;
            height += repetitions * repHeight;
            int extrarocks = (int)(rocks % repRocks);
            int i3 = chamber.rocks.IndexOf(chamber.rocks[i1] + extrarocks);
            height += i3 - i1;
            return height;
        }
        return 0;
    }

    private bool Compareline(char[] a, char[] b)
    {
        for (int i = 1; i < a.Length - 1; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    private static Chamber RunSimulation(string instructions, List<IEnumerable<string>> blocks, int steps)
    {
        var chamber = new Chamber();

        var ip = 0;
        var block = 0;
        chamber.AddBlock(blocks[block++]);
        Console.WriteLine(chamber.Print());
        var step = 0;
        while (true)
        {
            var inst = instructions[ip++ % instructions.Count()];
            chamber.Push(Direction.FromChar(inst));
            // Console.WriteLine($"INST: {inst}");
            // Console.WriteLine(chamber.Print());
            if (!chamber.Fall())
            {
                if (++step == steps)
                    break;
                chamber.AddBlock(blocks[block++ % blocks.Count()]);
                //Console.WriteLine("new block");
                //Console.WriteLine(chamber.Print());
            }
        }
        Console.WriteLine(chamber);
        return chamber;
    }

    class Chamber
    {
        const string wall = "|.......|";
        int top = 0;
        string[] currentBlock = null;
        Position currentBlockPosition;

        public List<char[]> chamber = new() { "+-------+".ToCharArray() };
        public List<int> rocks = new() { 0 };
        
        public void AddBlock(IEnumerable<string> block)
        {
            currentBlock = block.Reverse().ToArray();

            var idx = top + 4;
            currentBlockPosition = new Position(idx, 3);

            while (chamber.Count() <= currentBlockPosition.X + block.Count())
            {
                chamber.Add(wall.ToCharArray());
            }
        }

        public int Height => top;

        public bool Push(Direction dir)
        {
            var x = 0;
            var newpos = currentBlockPosition.Add(dir);
            foreach (var line in currentBlock)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == '#')
                    {
                        var space = chamber[newpos.X + x][i + newpos.Y];
                        if (space != '.')
                        {
                            return false;
                        }
                    }
                }
                x++;
            }
            currentBlockPosition = newpos;
            return true;
        }

        public bool Fall()
        {
            var x = 0;
            var newpos = currentBlockPosition.Add(Direction.N);
            foreach (var line in currentBlock)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == '#')
                    {
                        var space = chamber[newpos.X + x][i + newpos.Y];
                        if (space != '.')
                        {
                            Solidify();
                            return false;
                        }
                    }
                }
                x++;
            }
            currentBlockPosition = newpos;
            return true;
        }

        public void Solidify()
        {
            var x = 0;
            foreach (var line in currentBlock)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == '#')
                    {
                        chamber[currentBlockPosition.X + x][i + currentBlockPosition.Y] = '#';
                        top = Math.Max(top, currentBlockPosition.X + x);

                    }
                }
                x++;
            }
            var curBlock = rocks[^1];
            while (rocks.Count <= top)
            {
                rocks.Add(-1);
            }
            rocks[top] = curBlock + 1;
        }

        public string Print()
        {
            StringBuilder sb = new();
            var l = chamber.Count;

            foreach (var chline in Enumerable.Reverse(chamber))
            {
                var idx = l - currentBlockPosition.X - 1;
                if (idx < 0 || idx >= currentBlock.Length)
                {
                    sb.AppendLine(new string(chline));
                }
                else
                {
                    for (int i = 0; i < chline.Length; i++)
                    {
                        var yidx = i - currentBlockPosition.Y;
                        if (yidx >= 0 && yidx < currentBlock[idx].Length &&
                            currentBlock[idx][yidx] == '#')
                        {
                            sb.Append("@");
                        }
                        else
                        {
                            sb.Append(chline[i]);
                        }
                    }
                    sb.AppendLine();
                }
                l--;
            }
            return sb.ToString();
        }

        enum Result
        {
            NoOp,
            Moved,
            Stopped
        }

        //public string Print()
        //{
        //    return string.Join("\r\n", Enumerable.Reverse(chamber).Select(c => new string(c, 0, c.Length)));
        //}

        public override string ToString()
        {
            return string.Join("\r\n", Enumerable.Reverse(chamber).Select(c => new string(c, 0, c.Length)));
        }

    }
}
