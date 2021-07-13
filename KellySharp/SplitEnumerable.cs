using System;

namespace KellySharp
{
    public readonly ref struct SplitEnumerable<T> where T : IEquatable<T>
    {
        public readonly ReadOnlySpan<T> _haystack;
        public readonly ReadOnlySpan<T> _needle;

        public SplitEnumerable(ReadOnlySpan<T> haystack, ReadOnlySpan<T> needle)
        {
            _haystack = haystack;
            _needle = needle;
        }

        public SplitEnumerator<T> GetEnumerator() => new SplitEnumerator<T>(_haystack, _needle);
    }
}
