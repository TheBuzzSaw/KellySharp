using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using KellySharp;

namespace KellyBenchmark
{
    public class SquareRootBenchmark
    {
        public static IEnumerable<int> Values()
        {
            return new int[]
            {
                4,
                99,
                400,
                1 << 20 + 1
            };
        }

        [ParamsSource(nameof(Values))]
        public int N { get; set; }

        [Benchmark(Baseline = true)]
        public void SimpleSquareRoot()
        {
            _ = (int)Math.Sqrt(N);
        }

        [Benchmark]
        public void FloorSquareRoot()
        {
            _ = (int)Math.Floor(Math.Sqrt(N));
        }

        [Benchmark]
        public void BinarySearchSquareRoot()
        {
            _ = IntegerMath.SquareRootBinarySearch(N);
        }
    }
}
