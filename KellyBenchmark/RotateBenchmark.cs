using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;

namespace KellyBenchmark
{
    public class RotateBenchmark
    {
        public static void Reverse<T>(Span<T> span)
        {
            var half = span.Length / 2;
            for (int i = 0; i < half; ++i)
            {
                int ii = span.Length - 1 - i;
                var swapValue = span[i];
                span[i] = span[ii];
                span[ii] = swapValue;
            }
        }

        public static void RotateViaReverse<T>(Span<T> span, int k)
        {
            span.Reverse();
            span.Slice(0, k).Reverse();
            span.Slice(k).Reverse();
        }

        public static void RotateViaCycle<T>(Span<T> span, int k)
        {
            int count = 0;

            for (int start = 0; count < span.Length; ++start)
            {
                int current = start;
                var prev = span[start];

                do
                {
                    int next = (current + k) % span.Length;
                    var temp = span[next];
                    span[next] = prev;
                    prev = temp;
                    current = next;
                    ++count;
                }
                while (start != current);
            }
        }

        public static void RotateViaBuffer<T>(Span<T> span, int k)
        {
            var copy = span.ToArray();

            for (int i = 0; i < span.Length; ++i)
            {
                int index = (k + i) % span.Length;
                span[index] = copy[i];
            }
        }

        public static IEnumerable<RotateInput> Values()
        {
            var mebi = 1 << 20;

            return new RotateInput[]
            {
                new RotateInput(8, 1),
                new RotateInput(8, 4),
                new RotateInput(8, 7),
                new RotateInput(2048, 1),
                new RotateInput(2048, 1024),
                new RotateInput(2048, 2047),
                new RotateInput(mebi, 1),
                new RotateInput(mebi, mebi / 2),
                new RotateInput(mebi, mebi - 1)
            };
        }
        
        private int[] _data = null;

        
        [ParamsSource(nameof(Values))]
        public RotateInput Input { get; set; }


        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(1337);
            _data = new int[Input.Count];
            
            for (int i = 0; i < _data.Length; ++i)
                _data[i] = random.Next();
        }

        [Benchmark]
        public void BenchmarkReverse()
        {
            RotateViaReverse(_data.AsSpan(), Input.K);
        }

        [Benchmark]
        public void BenchmarkCycle()
        {
            RotateViaCycle(_data.AsSpan(), Input.K);
        }

        [Benchmark]
        public void BenchmarkBuffer()
        {
            RotateViaBuffer(_data.AsSpan(), Input.K);
        }
    }
}