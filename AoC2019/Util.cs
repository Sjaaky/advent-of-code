using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC2019Test
{
    public static class Util
    {
        public static IEnumerable<int> Range(int a, int b)
        {
            var start = Math.Min(a, b);
            var end = Math.Max(a, b);
            return Enumerable.Range(start, end - start + 1);
        }

    }
}
