using System;

namespace KellySharp
{
    public ref struct SplitEnumerator<T> where T : IEquatable<T>
    {
        private readonly ReadOnlySpan<T> _haystack;
        private readonly ReadOnlySpan<T> _needle;
        private readonly SplitEnumeration _splitEnumeration;
        private int _start;
        private int _length;
        private int _next;

        public ReadOnlySpan<T> Current => _haystack.Slice(_start, _length);

        public SplitEnumerator(
            ReadOnlySpan<T> haystack,
            ReadOnlySpan<T> needle,
            SplitEnumeration splitEnumeration)
        {
            _haystack = haystack;
            _needle = needle;
            _splitEnumeration = splitEnumeration;
            _start = 0;
            _length = 0;
            _next = 0;
        }

        public bool MoveNext()
        {
            var skip = (_splitEnumeration & SplitEnumeration.SkipEmptyItems) == SplitEnumeration.SkipEmptyItems;
            
            do
            {
                if (_haystack.Length <= _next)
                    return false;
                
                _start = _next;
                _length = _haystack.Slice(_start).IndexOf(_needle);

                if (_length == -1)
                    _length = _haystack.Length - _start;

                _next = _start + _length + _needle.Length;
            }
            while (skip && Current.IsEmpty);

            return true;
        }
    }
}
