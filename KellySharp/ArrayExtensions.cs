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
            int inverseCount = span.Length - count;
            int index = 0;
            var value = span[index];

            for (int i = 0; i < span.Length; ++i)
            {
                int nextIndex = index < inverseCount ?
                    index + count :
                    index - inverseCount;
                
                var nextValue = span[nextIndex];
                span[nextIndex] = value;

                index = nextIndex;
                value = nextValue;
            }
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