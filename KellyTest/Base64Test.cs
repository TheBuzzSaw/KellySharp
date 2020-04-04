using System;
using System.Collections.Generic;
using KellySharp;
using Xunit;

namespace KellyTest
{
    public class Base64Test
    {
        public static IEnumerable<object[]> RandomByteArrays()
        {
            var random = new Random();
            for (int i = 0; i < 64; ++i)
            {
                var bytes = new byte[i];
                random.NextBytes(bytes);
                
                yield return new object[] { bytes };
            }
        }

        [Theory]
        [MemberData(nameof(RandomByteArrays))]
        public void VerifyBase64Encoding(byte[] bytes)
        {
            var expected = Convert.ToBase64String(bytes);
            var actual = Base64.Encode(bytes);
            Assert.Equal(expected, actual);
        }
    }
}