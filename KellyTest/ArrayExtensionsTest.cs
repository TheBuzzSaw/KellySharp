using System;
using KellySharp;
using Xunit;

namespace KellyTest
{
    public class ArrayExtensionsTest
    {
        private static int[] CreateArray(int n)
        {
            var result = new int[n];

            for (int i = 0; i < result.Length; ++i)
                result[i] = i;
            
            return result;
        }

        [Fact]
        public void StablePartitionWorks()
        {
            var expected = new int[] { 1, 2, 3, 4, 5, 6, 10, 11, 12, 13 };
            
            var actual = new int[] { 1, 2, 10, 11, 3, 4, 12, 13, 5, 6 };
            var count = actual.AsSpan().StablePartition(n => n < 10);
            
            Assert.Equal(6, count);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HalfStablePartitionWorks()
        {
            var values = new int[] { 1, 2, 3, 4, 5, 6, 10, 11, 12, 13 };
            var count = values.AsSpan().HalfStablePartition(n => 10 < n);

            Assert.Equal(3, count);
            Assert.Equal(11, values[0]);
            Assert.Equal(12, values[1]);
            Assert.Equal(13, values[2]);
        }

        [Fact]
        public void SwiftRotateWorks()
        {
            var expected = new int[] { 4, 5, 6, 7, 8, 9, 1, 2, 3 };
            var actual = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var result = actual.AsSpan().RotateSwift(3);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SafeOnEmpty()
        {
            default(Span<int>).StablePartition(_ => true);
            Array.Empty<int>().AsSpan().StablePartition(_ => true);
        }

        [Theory]
        [InlineData(8, 1)]
        [InlineData(8, 4)]
        [InlineData(8, 7)]
        public void Rotate(int n, int k)
        {
            var firstArray = CreateArray(n);
            firstArray.AsSpan().RotateRight(k);

            var secondArray = CreateArray(n);
            secondArray.AsSpan().RotateViaCycle(k);

            Assert.Equal(firstArray, secondArray);
        }

        [Fact]
        public void EnumerateSplit()
        {
            int count = 0;
            foreach (var item in "aaa,bbb,ccc,ddd".EnumerateSplit(","))
            {
                ++count;
                Assert.Equal(3, item.Length);
                Assert.Equal(item[0], item[1]);
                Assert.Equal(item[0], item[2]);
            }

            Assert.Equal(4, count);
        }
    }
}
