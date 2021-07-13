using System;

namespace KellySharp
{
    public readonly ref struct SplitEnumerable<T> where T : IEquatable<T>
    {
        public readonly ReadOnlySpan<T> _haystack;
        public readonly ReadOnlySpan<T> _needle;
        public readonly SplitEnumeration _splitEnumeration;

        public SplitEnumerable(
            ReadOnlySpan<T> haystack,
            ReadOnlySpan<T> needle,
            SplitEnumeration splitEnumeration)
        {
            _haystack = haystack;
            _needle = needle;
            _splitEnumeration = splitEnumeration;
        }

        public SplitEnumerator<T> GetEnumerator() => new SplitEnumerator<T>(_haystack, _needle, _splitEnumeration);
    }
}
