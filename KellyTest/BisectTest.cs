using System;
using KellySharp;
using Xunit;

namespace KellyTest
{
    public class BisectTest
    {
        [Fact]
        public void BisectsCorrectly()
        {
            ReadOnlySpan<int> values = new int[] { 1, 3, 3, 5, 5, 5, 7, 7, 9};

            Assert.Equal(0, Bisect.Left(values, 0));
            Assert.Equal(0, Bisect.Right(values, 0));
            Assert.Equal(3, Bisect.Left(values, 5));
            Assert.Equal(6, Bisect.Right(values, 5));
        }
    }
}