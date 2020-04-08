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
        
        private static void RotateRight<T>(this Span<T> span, int count)
        {
            span.Reverse();
            span.Slice(0, count).Reverse();
            span.Slice(count).Reverse();
        }

        public static void Rotate<T>(this Span<T> span, int count)
        {
            if (span.IsEmpty || span.Length < 2)
            {
                return;
            }
            else
            {
                while (count < 0)
                    count += span.Length;
                
                count %= span.Length;
                
                if (0 < count)
                    RotateRight(span, count % span.Length);
            }
        }

        public static void Reverse<T>(this Span<T> span)
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
        
        public static void StablePartition<T>(this Span<T> span, Predicate<T> predicate)
        {
            int lastIndex = span.Length - 1;
            int baseIndex = 0;

            while (baseIndex < lastIndex)
            {
                int begin = FindIndex(span, baseIndex, predicate); // Inclusive index
                
                if (begin == -1)
                    break;
                
                int end = begin + 1; // Exclusive index

                while (end < lastIndex && predicate.Invoke(span[end]))
                    ++end;
                
                int matchCount = end - begin;
                int length = end - baseIndex;
                RotateRight(span.Slice(baseIndex, length), matchCount);
                baseIndex += matchCount;
            }
        }
    }
}