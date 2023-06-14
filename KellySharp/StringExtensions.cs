using System;

namespace KellySharp;

public static class StringExtensions
{
    public static string Repeated(this string text, int count)
    {
        if (text.Length == 0 || count < 1)
            return "";

        if (count == 1)
            return text;

        if (text.Length == 1)
            return new string(text[0], count);

        return Repeat(text, count);
    }

    public static string Repeated(this ReadOnlySpan<char> text, int count)
    {
        if (text.IsEmpty || count < 1)
            return "";

        if (count == 1)
            return new string(text);

        if (text.Length == 1)
            return new string(text[0], count);

        return Repeat(text, count);
    }

    #pragma warning disable CS8500
    private static string Repeat(ReadOnlySpan<char> text, int count)
    {
        unsafe
        {
            var result = string.Create(
                text.Length * count,
                new nint(&text),
                static (span, state) =>
                {
                    var text = *(ReadOnlySpan<char>*)state.ToPointer();
                    text.CopyTo(span);
                    int copied = text.Length;
                    while (copied < span.Length)
                    {
                        int toCopy = Math.Min(copied, span.Length - copied);
                        span[..toCopy].CopyTo(span[copied..]);
                        copied += toCopy;
                    }
                });

            return result;
        }
    }
    #pragma warning restore CS8500

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

    public static SplitEnumerable<char> EnumerateSplit(
        this string haystack,
        ReadOnlySpan<char> needle,
        SplitEnumeration splitEnumeration)
    {
        return new SplitEnumerable<char>(haystack, needle, splitEnumeration);
    }
}
