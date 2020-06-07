using System;

namespace KellySharp
{
    public static class ArrayExtensions
    {
        private static int FindIndex<T>(ReadOnlySpan<T> span, int startIndex, Predicate<T> predicate)
        {
            for (int i = startIndex; i < span.Length; ++i)
            {
                if (predicate.Invoke(span[i]))
                    return i;
            }

            return -1;
        }

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
        
        public static void RotateRight<T>(this Span<T> span, int count)
        {
            span.Reverse();
            span.Slice(0, count).Reverse();
            span.Slice(count).Reverse();
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
        
        public static void StablePartition<T>(this Span<T> span, Predicate<T> predicate)
        {
            int lastIndex = span.Length - 1;
            int baseIndex = 0;

            // Skip initial values that already qualify.
            while (baseIndex < lastIndex && predicate.Invoke(span[baseIndex]))
                ++baseIndex;

            while (baseIndex < lastIndex)
            {
                int begin = FindIndex(span, baseIndex, predicate); // Inclusive index
                
                if (begin == -1)
                    break;
                
                int end = begin + 1; // Exclusive index

                while (end < span.Length && predicate.Invoke(span[end]))
                    ++end;
                
                int matchCount = end - begin;
                int length = end - baseIndex;
                RotateRight(span.Slice(baseIndex, length), matchCount);
                baseIndex += matchCount;
            }
        }
    }
}