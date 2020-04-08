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
            var expected = new int[] { 1, 2, 3, 4, 5, 6, 10, 11, 12, 13 };
            
            var actual = new int[] { 1, 2, 10, 11, 3, 4, 12, 13, 5, 6 };
            actual.AsSpan().StablePartition(n => n < 10);
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SafeOnEmpty()
        {
            default(Span<int>).StablePartition(_ => true);
            Array.Empty<int>().AsSpan().StablePartition(_ => true);
        }
    }
}