using System;
using System.Collections.Generic;
using Xunit;

namespace KellySharp
{
    public class DateTest
    {
        public static IEnumerable<object[]> DateData()
        {
            var start = new DateTime(1900, 1, 1);
            var end = new DateTime(2100, 1, 1);
            for (var date = start; date < end; date = date.AddDays(1))
            {
                yield return new object[] {date.Year, date.Month, date.Day};
            }
        }

        [Theory]
        [MemberData(nameof(DateData))]
        public void RoundTrip(int year, int month, int day)
        {
            var date = new Date(year, month, day);
            Assert.Equal(year, date.Year);
            Assert.Equal(month, date.Month);
            Assert.Equal(day, date.Day);
        }
    }
}