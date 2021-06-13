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
        
        public static int StablePartition<T>(this Span<T> span, Predicate<T> predicate)
        {
            int leftCount = 0;

            while (leftCount < span.Length && predicate.Invoke(span[leftCount]))
                ++leftCount;

            int rightCount = 0;
            while (true)
            {
                int begin = leftCount + rightCount;

                while (begin < span.Length && !predicate.Invoke(span[begin]))
                {
                    ++begin;
                    ++rightCount;
                }
                
                if (begin == span.Length)
                    break;
                
                int end = begin + 1;

                while (end < span.Length && predicate.Invoke(span[end]))
                    ++end;
                
                int matchCount = end - begin;
                int length = end - leftCount;
                RotateRight(span.Slice(leftCount, length), matchCount);
                leftCount += matchCount;
            }

            return leftCount;
        }

        public static int HalfStablePartition<T>(Span<T> span, Predicate<T> predicate)
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

        public static T[] Without<T>(this T[] array, params T[] values)
        {
            Array.Sort(values);
            var buffer = new T[array.Length];
            int n = 0;

            foreach (var item in array)
            {
                if (Array.BinarySearch(values, 0, values.Length, item) < 0)
                    buffer[n++] = item;
            }

            var result = new T[n];
            Array.Copy(buffer, result, n);
            return result;
        }
    }
}