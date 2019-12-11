using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AoC2019Test.Util;

namespace AoC2019Test
{
    public partial class Day8
    {
        [Test]
        public void Part1()
        {
            List<int[,]> layers = ReadImage();

            var stats = layers.Select(l => CountDigits(l)).ToArray();
            var min0Layer = stats.OrderBy(l => l[0]).FirstOrDefault();

            var result = min0Layer[1] * min0Layer[2];
            Console.WriteLine(result);
            Assert.AreEqual(2440, result);
        }

        [Test]
        public void Part2()
        {
            List<int[,]> layers = ReadImage();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int l = 0; l < layers.Count; l++)
                    {
                        var p = layers[l][x, y];
                        if (p == 2) continue;
                        Console.Write(p == 1 ? "█" : "░");
                        break;
                    }
                }
                Console.WriteLine();
            }
        }

        int width = 25;
        int height = 6;

        private List<int[,]> ReadImage()
        {
            var input = File.ReadAllText("day8.input").ToArray();

            List<int[,]> layers = new List<int[,]> { new int[width, height] };

            int x = 0;
            int y = 0;
            int layer = 0;
            foreach (char c in input)
            {
                int pixel = c - '0';
                if (pixel < 0 || pixel > 9) continue;
                layers[layer][x, y] = pixel;
                x++;
                if (x >= width)
                {
                    x = 0;
                    y++; 
                }
                if (y >= height)
                {
                    y = 0;
                    layer++;
                    layers.Add(new int[width, height]);
                }
            }

            return layers;
        }

        public int[] CountDigits(int[,] layer)
        {
            int[] occ = new int[10];
            foreach(var p in layer)
            {
                occ[p]++;
            }
            return occ;
        }
    }
}
