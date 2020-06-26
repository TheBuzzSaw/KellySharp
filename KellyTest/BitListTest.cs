using System;
using KellySharp;
using Xunit;

namespace KellyTest
{
    public class BitListTest
    {
        [Fact]
        public void NewListIsEmpty()
        {
            var bitList = new BitList();
            Assert.Equal(0, bitList.Count);
            Assert.Throws<IndexOutOfRangeException>(() => bitList.ReadBit(0));
        }

        [Fact]
        public void BitSetsAndReads()
        {
            var bitList = new BitList();
            bitList.AddClearBit();
            Assert.False(bitList[0]);
            bitList.SetBit(0);
            Assert.True(bitList[0]);
            bitList.ClearBit(0);
            Assert.False(bitList[0]);
        }
    }
}