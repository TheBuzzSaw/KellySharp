using System;
using KellySharp;
using Xunit;

namespace KellyTest
{
    public class IntegerMathTest
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(4)]
        [InlineData(10)]
        [InlineData(49)]
        [InlineData(99)]
        [InlineData(400)]
        [InlineData(5000)]
        [InlineData(Int16.MaxValue)]
        public void VerifySquareRootByBinarySearch(int n)
        {
            if (n < 0)
            {
                Assert.Throws<ArgumentException>(() => IntegerMath.SquareRootBinarySearch(n));
            }
            else
            {
                var approach1 = (int)Math.Sqrt(n);
                var approach2 = (int)Math.Floor(Math.Sqrt(n));
                var approach3 = IntegerMath.SquareRootBinarySearch(n);

                Assert.Equal(approach1, approach2);
                Assert.Equal(approach1, approach3);
            }
        }
    }
}
