using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AoC2022;

public class Day07
{
    [TestCase("day07.input", ExpectedResult = 1444896)]
    [TestCase("day07example1.input", ExpectedResult = 95437)]
    public long Part1(string input)
    {
        var lines = File.ReadAllLines(input);

        var dir = Parse(lines);
        var sum = DFS(dir).Where(d => d.Size <= 100000).Sum(d => d.Size);
        return sum;
    }

    [TestCase("day07.input", ExpectedResult = 404395)]
    [TestCase("day07example1.input", ExpectedResult = 24933642)]
    public long Part2(string input)
    {
        var lines = File.ReadAllLines(input);

        var dir = Parse(lines);
        var capacity = 70000000;
        var goal = 30000000;
        var rootsize = dir.Size;
        var available = capacity - rootsize;
        var toFree = goal - available;

        var dirToDelete = DFS(dir).Where(d => d.Size >= toFree).OrderBy(d => d.Size).First();
        return dirToDelete.Size;
    }

    Regex cd = new Regex(@"\$ cd ([a-z/\.]+)");
    Regex ls = new Regex(@"\$ ls");
    Regex dir = new Regex(@"dir ([a-z]+)");
    Regex dfile = new Regex(@"(\d+) ([a-z\.]+)");
    private Dir Parse(string[] lines)
    {
        Dir root = new Dir() { Name = "/" };
        Stack<Dir> current = new();
        foreach (var line in lines)
        {
            var mc = cd.Match(line);
            if (mc.Success)
            {
                var dirname = mc.Groups[1].Value;
                if (dirname == "..")
                {
                    current.Pop();
                }
                else
                {
                    if (current.Any())
                    {
                        var dirs = current.Peek().Dirs;
                        var dir = dirs.Single(d => d.Name == dirname);
                        current.Push(dir);
                    }
                    else
                    {  //root
                        current.Push(root);
                    }
                }
            }
            var mls = ls.Match(line);
            if (mls.Success)
            {

            }
            var mdir = dir.Match(line);
            if (mdir.Success)
            {
                var dirname = mdir.Groups[1].Value;
                var dir = new Dir() { Name = dirname };
                current.Peek().Dirs.Add(dir);
            }
            var mdfile = dfile.Match(line);
            if (mdfile.Success)
            {
                var size = mdfile.Groups[1].Value;
                var filename = mdfile.Groups[2].Value;
                var file = new DFile() { Name = filename, Size = long.Parse(size) };
                current.Peek().Files.Add(file);
            }
        }
        return root;
    }

    [DebuggerDisplay("{Name} {Dirs.Count} {Files.Count}")]
    public class Dir
    {
        public string Name { get; set; }
        public List<Dir> Dirs { get; set; } = new();
        public List<DFile> Files { get; set; } = new();
        public long Size => Dirs.Sum(s => s.Size) + Files.Sum(s => s.Size);

        public override string ToString() => $"{Name} {Dirs.Count} {Files.Count}";
    }

    [DebuggerDisplay("{Name} {Size}")]
    public class DFile
    {
        public string Name { get; set; }
        public long Size { get; set; }

        public override string ToString() => $"{Name} {Size}";
    }

    public void PrintDir(TextWriter tw, Dir d, int indent = 0)
    {
        tw.WriteLine($"{new string(' ', indent)} {d.Name} (dir)");
        foreach (var dir in d.Dirs)
        {
            PrintDir(tw, dir, indent + 2);
        }
        foreach (var file in d.Files)
        {
            tw.WriteLine($"{new string(' ', indent+1)} {file.Name} (file, size={file.Size})");
        }
    }

    public IEnumerable<Dir> DFS(Dir d)
    {
        Stack<Dir> current = new();
        current.Push(d);
        while (current.Any())
        {
            var dir = current.Pop();
            foreach (var d1 in dir.Dirs)
            {
                current.Push(d1);
            }
            yield return dir;
        }
    }
}

