using System;
using BenchmarkDotNet.Running;

namespace KellyBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            // var summary = BenchmarkRunner.Run<RotateBenchmark>();
            var summary = BenchmarkRunner.Run<SquareRootBenchmark>();
        }
    }
}
