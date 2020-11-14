using KellySharp;
using Xunit;

namespace KellyTest
{
    public class UInt2ListTest
    {
        [Fact]
        public void StoreAndRetrieveAll()
        {
            var list = new UInt2List();

            list.Add(1);
            list.Add(2);
            list.Add(3);

            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
            Assert.Equal(3, list[2]);

            int nextValue = 0;

            foreach (var value in list)
                Assert.Equal(++nextValue, value);
        }

        [Fact]
        public void StoreAndRetrieveMany()
        {
            var list = new UInt2List();

            for (int i = 0; i < 64; ++i)
                list.Add(i);
            
            for (int i = 0; i < 64; ++i)
                Assert.Equal(i & 3, list[i]);
        }
    }
}