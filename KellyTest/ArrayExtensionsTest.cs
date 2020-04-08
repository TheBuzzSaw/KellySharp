using System;
using KellySharp;
using Xunit;

namespace KellyTest
{
    public class ArrayExtensionsTest
    {
        [Fact]
        public void ShiftGroup()
        {
            var expected = new int[] { 1, 2, 3, 4, 10, 11, 12, 13 };
            
            var actual = new int[] { 1, 2, 10, 11, 3, 4, 12, 13 };
            actual.AsSpan().StablePartition(n => n < 10);
            
            Assert.Equal(expected, actual);
        }
    }
}