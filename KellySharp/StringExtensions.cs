using System;

namespace KellySharp
{
    public static class StringExtensions
    {
        public static string Without(this string s, params char[] chars)
        {
            Array.Sort(chars);
            var buffer = new char[s.Length];
            int n = 0;

            foreach (var c in s)
            {
                if (Array.BinarySearch(chars, 0, chars.Length, c) < 0)
                    buffer[n++] = c;
            }

            return new string(buffer, 0, n);
        }
    }
}