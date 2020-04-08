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
            if (span.Length < 2)
                return;
            
            int lastIndex = span.Length - 1;
            int baseIndex = 0;

            while (baseIndex < lastIndex)
            {
                int firstIndex = FindIndex(span, baseIndex, predicate);
                
                if (firstIndex == -1)
                    break;
                
                int secondIndex = firstIndex;

                while (secondIndex < lastIndex && predicate.Invoke(span[secondIndex + 1]))
                    ++secondIndex;
                
                if (baseIndex < firstIndex)
                {
                    int matchCount = secondIndex - firstIndex + 1;
                    int length = secondIndex - baseIndex + 1;
                    RotateRight(span.Slice(baseIndex, length), matchCount);
                    baseIndex += matchCount;
                }
                else
                {
                    baseIndex = secondIndex + 1;
                }
            }
        }
    }
}