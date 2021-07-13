using System;

namespace KellySharp
{
    public static class ArrayExtensions
    {
        public static void RotateViaCycle<T>(this Span<T> span, int k)
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

        public static int RotateSwift<T>(this Span<T> span, int middle)
        {
            // https://github.com/apple/swift/blob/main/test/Prototypes/Algorithms.swift
            // public mutating func rotate(shiftingToStart middle: Index) -> Index

            var m = middle;
            var s = 0;
            var e = span.Length;

            if (s == m)
                return e;
            if (m == e)
                return s;
            
            var ret = e;

            while (true)
            {
                var lhs = span[s..m];
                var rhs = span[m..e];

                int i = 0;

                do
                {
                    var swapValue = lhs[i];
                    lhs[i] = rhs[i];
                    rhs[i++] = swapValue;
                }
                while (i < lhs.Length && i < rhs.Length);

                var s1 = s + i;
                var m1 = m + i;

                if (m1 == e)
                {
                    if (ret == e)
                        ret = s1;
                    if (s1 == m)
                        break;
                }

                s = s1;

                if (s == m)
                    m = m1;
            }

            return ret;
        }
        
        public static void RotateRight<T>(this Span<T> span, int count)
        {
            span.Reverse();
            span[..count].Reverse();
            span[count..].Reverse();
        }

        public static void Rotate<T>(this Span<T> span, int count)
        {
            if (span.Length < 2)
            {
                return;
            }
            else
            {
                if (count < 0)
                {
                    do
                    {
                        count += span.Length;
                    }
                    while (count < 0);
                }
                else
                {
                    count %= span.Length;
                }
                
                if (0 < count)
                    RotateRight(span, count);
            }
        }
        
        public static int StablePartition<T>(this Span<T> span, Predicate<T> predicate)
        {
            int leftCount = 0;

            while (leftCount < span.Length && predicate.Invoke(span[leftCount]))
                ++leftCount;

            int begin = leftCount;
            while (true)
            {
                while (begin < span.Length && !predicate.Invoke(span[begin]))
                    ++begin;
                
                if (begin == span.Length)
                    break;
                
                int end = begin + 1;

                while (end < span.Length && predicate.Invoke(span[end]))
                    ++end;
                
                int matchCount = end - begin;
                int length = end - leftCount;
                RotateRight(
                    span.Slice(leftCount, length),
                    matchCount);
                leftCount += matchCount;
                begin = end;
            }

            return leftCount;
        }

        public static int HalfStablePartition<T>(this Span<T> span, Predicate<T> predicate)
        {
            int count = 0;

            while (count < span.Length && predicate.Invoke(span[count]))
                ++count;
            
            for (int i = count + 1; i < span.Length; ++i)
            {
                if (predicate.Invoke(span[i]))
                {
                    var swapValue = span[count];
                    span[count++] = span[i];
                    span[i] = swapValue;
                }
            }

            return count;
        }

        public static SplitEnumerable<T> EnumerateSplit<T>(this ReadOnlySpan<T> haystack, ReadOnlySpan<T> needle) where T : IEquatable<T>
        {
            return new SplitEnumerable<T>(haystack, needle);
        }
    }
}
