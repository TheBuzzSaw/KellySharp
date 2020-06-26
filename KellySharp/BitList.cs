using System;

namespace KellySharp
{
    public class BitList
    {
        private static int MajorIndexOf(int index) => index / 32;
        private static int MinorIndexOf(int index) => index % 32;

        private uint[] _bits = Array.Empty<uint>();

        public int Capacity => _bits.Length * 32;
        public int Count { get; private set; }

        public bool this[int index]
        {
            get
            {
                if (index < 0 || Count <= index)
                    throw new IndexOutOfRangeException();
                
                var minorIndex = MinorIndexOf(index);
                var mask = 1u << minorIndex;
                var majorIndex = MajorIndexOf(index);
                return (_bits[majorIndex] & mask) == mask;
            }

            set
            {
                if (index < 0 || Count <= index)
                    throw new IndexOutOfRangeException();
                
                var minorIndex = MinorIndexOf(index);
                var mask = 1u << minorIndex;
                var majorIndex = MajorIndexOf(index);

                if (value)
                    _bits[majorIndex] |= mask;
                else
                    _bits[majorIndex] &= ~mask;
            }
        }

        public BitList()
        {

        }
    }
}