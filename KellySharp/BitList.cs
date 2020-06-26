using System;

namespace KellySharp
{
    public class BitList
    {
        private const int BitsPerBlock = 32;
        private static int MajorIndexOf(int index) => index / BitsPerBlock;
        private static int MinorIndexOf(int index) => index % BitsPerBlock;

        private uint[] _bits = Array.Empty<uint>();

        public int Capacity => _bits.Length * BitsPerBlock;
        public int Count { get; private set; }

        public bool this[int index]
        {
            get => ReadBit(index);
            set => SetBit(index, value);
        }

        public BitList()
        {
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || Count <= index)
                throw new IndexOutOfRangeException();
        }

        private int GetNewIndex()
        {
            int index = Count++;

            if (Capacity < Count)
            {
                int newSize = Math.Max(_bits.Length * 2, 8);
                Array.Resize(ref _bits, newSize);
            }

            return index;
        }

        private void DangerousSetBit(int index)
        {
            var minorIndex = MinorIndexOf(index);
            var mask = 1u << minorIndex;
            var majorIndex = MajorIndexOf(index);
            _bits[majorIndex] |= mask;
        }

        private void DangerousClearBit(int index)
        {
            var minorIndex = MinorIndexOf(index);
            var mask = 1u << minorIndex;
            var majorIndex = MajorIndexOf(index);
            _bits[majorIndex] &= ~mask;
        }

        public void AddBit(bool value)
        {
            int index = GetNewIndex();

            if (value)
                DangerousSetBit(index);
            else
                DangerousClearBit(index);
        }

        public void AddSetBit()
        {
            int index = GetNewIndex();
            DangerousSetBit(index);
        }

        public void AddClearBit()
        {
            int index = GetNewIndex();
            DangerousClearBit(index);
        }

        public bool ReadBit(int index)
        {
            ValidateIndex(index);
            var minorIndex = MinorIndexOf(index);
            var mask = 1u << minorIndex;
            var majorIndex = MajorIndexOf(index);
            return (_bits[majorIndex] & mask) == mask;
        }

        public void SetBit(int index, bool value)
        {
            if (value)
                SetBit(index);
            else
                ClearBit(index);
        }

        public void SetBit(int index)
        {
            ValidateIndex(index);
            DangerousSetBit(index);
        }

        public void ClearBit(int index)
        {
            ValidateIndex(index);
            DangerousClearBit(index);
        }
    }
}