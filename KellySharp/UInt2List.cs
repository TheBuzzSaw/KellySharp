using System;
using System.Collections.Generic;

namespace KellySharp
{
    public class UInt2List
    {
        private static int OuterIndex(int index) => index / 16;
        private static int ShiftIndex(int index) => (index % 16) * 2;

        private readonly List<uint> _values = new List<uint>();

        public int Count { get; private set; }
        public int Capacity => _values.Count * 16;


        private void CheckIndex(int index)
        {
            if (index < 0 || Count <= index)
                throw new IndexOutOfRangeException();
        }

        private void DangerousSet(int index, int value)
        {
            var outerIndex = OuterIndex(index);
            var shift = ShiftIndex(index);
            var clearMask = ~(uint)(3 << shift);
            var valueMask = (uint)((value & 3) << shift);
            var currentValue = _values[outerIndex];
            _values[outerIndex] = (currentValue & clearMask) | valueMask;
        }

        public int this[int index]
        {
            get
            {
                CheckIndex(index);
                var outerIndex = OuterIndex(index);
                var shift = ShiftIndex(index);
                var mask = 3U << shift;
                return (int)((_values[outerIndex] & mask) >> shift);
            }

            set
            {
                CheckIndex(index);
                DangerousSet(index, value);
            }
        }

        public void Add(int value)
        {
            if (Count < Capacity)
            {
                DangerousSet(Count, value);
            }
            else
            {
                _values.Add((uint)(value & 3));
            }

            ++Count;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        public struct Enumerator
        {
            private readonly UInt2List _list;
            private int _index;

            public int Current => _list[_index];

            public Enumerator(UInt2List list)
            {
                _list = list;
                _index = -1;
            }

            public bool MoveNext() => ++_index < _list.Count;
        }
    }
}