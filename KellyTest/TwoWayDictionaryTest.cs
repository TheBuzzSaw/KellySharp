using System;
using KellySharp;
using Xunit;

namespace KellyTest
{
    public class TwoWayDictionaryTest
    {
        [Fact]
        public void EnsureBothDirections()
        {
            var dictionary = new TwoWayDictionary<int, int>();
            dictionary.Add(1, 2);

            Assert.True(dictionary.ContainsKey(1));
            Assert.True(dictionary.ContainsValue(2));
            Assert.False(dictionary.ContainsKey(2));
            Assert.False(dictionary.ContainsValue(1));
            Assert.Equal(dictionary[1], 2);
            Assert.Equal(dictionary.Reverse[2], 1);
        }
    }
}
