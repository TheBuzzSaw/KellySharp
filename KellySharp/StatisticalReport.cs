using System;
using System.Collections.Generic;
using System.Linq;

namespace KellySharp
{
    public static class StatisticalReport
    {
        public static StatisticalReport<TOutput> Convert<TInput, TOutput>(
            this StatisticalReport<TInput> sr,
            Converter<TInput, TOutput> converter)
        {
            return new StatisticalReport<TOutput>(
                sr.SampleCount,
                converter(sr.Min),
                converter(sr.Max),
                converter(sr.Mean),
                converter(sr.Median),
                converter(sr.StandardDeviation));
        }

        public static StatisticalReport<T> Create<T>(
            Converter<T, double> toDouble,
            Converter<double, T> fromDouble,
            params T[] values)
        {
            var doubles = Array.ConvertAll(values, toDouble);
            return Create(doubles).Convert(fromDouble);
        }

        public static StatisticalReport<T> Create<T>(
            Converter<T, double> toDouble,
            Converter<double, T> fromDouble,
            List<T>? values)
        {
            var doubles = values?.ConvertAll(toDouble);
            return Create(doubles).Convert(fromDouble);
        }

        public static StatisticalReport<double> Create(List<double>? values)
        {
            if (values is null || values.Count == 0)
            {
                return default;
            }
            else if (values.Count == 1)
            {
                var n = values[0];
                return new StatisticalReport<double>(1, n, n, n, n, default);
            }
            else
            {
                values.Sort();
                var min = values[0];
                var max = values[values.Count - 1];
                var median = default(double);

                if (IsOdd(values.Count))
                {
                    median = values[values.Count / 2];
                }
                else
                {
                    int index = values.Count / 2;
                    var high = values[index];
                    var low = values[index - 1];
                    median = (low + high) / 2.0;
                }

                var mean = values.Average();

                // https://stackoverflow.com/a/3141731
                var sum = values.Sum(d => Math.Pow(d - mean, 2.0));
                var standardDeviation = Math.Sqrt(sum / (values.Count - 1));

                return new StatisticalReport<double>(
                    values.Count,
                    min,
                    max,
                    mean,
                    median,
                    standardDeviation);
            }
        }

        public static StatisticalReport<double> Create(params double[] values)
        {
            if (values is null || values.Length == 0)
            {
                return default;
            }
            else if (values.Length == 1)
            {
                var n = values[0];
                return new StatisticalReport<double>(1, n, n, n, n, default);
            }
            else
            {
                Array.Sort(values);
                var min = values[0];
                var max = values[values.Length - 1];
                var median = default(double);

                if (IsOdd(values.Length))
                {
                    median = values[values.Length / 2];
                }
                else
                {
                    int index = values.Length / 2;
                    var high = values[index];
                    var low = values[index - 1];
                    median = (low + high) / 2.0;
                }

                var mean = values.Average();

                // https://stackoverflow.com/a/3141731
                var sum = values.Sum(d => Math.Pow(d - mean, 2.0));
                var standardDeviation = Math.Sqrt(sum / (values.Length - 1));

                return new StatisticalReport<double>(
                    values.Length,
                    min,
                    max,
                    mean,
                    median,
                    standardDeviation);
            }
        }

        private static bool IsOdd(int n) => (n & 1) == 1;
    }

    public readonly struct StatisticalReport<T>
    {
        public int SampleCount { get; }
        public T Min { get; }
        public T Max { get; }
        public T Mean { get; }
        public T Median { get; }
        public T StandardDeviation { get; }

        public StatisticalReport(
            int sampleCount,
            T min,
            T max,
            T mean,
            T median,
            T standardDeviation)
        {
            SampleCount = sampleCount;
            Min = min;
            Max = max;
            Mean = mean;
            Median = median;
            StandardDeviation = standardDeviation;
        }

        public string ToString(Converter<T, string?> converter)
        {
            var word = SampleCount == 1 ? "sample" : "samples";
            
            var result = string.Concat(
                SampleCount.ToString(),
                " ",
                word,
                ": median ",
                converter(Median),
                " mean ",
                converter(Mean),
                " stddev ",
                converter(StandardDeviation),
                " min ",
                converter(Min),
                " max ",
                converter(Max));
            
            return result;
        }

        public override string ToString() => ToString(v => v is null ? string.Empty : v.ToString());
    }
}