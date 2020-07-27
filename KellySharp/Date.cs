using System;

namespace KellySharp
{
    public readonly struct Date
    {
        private static readonly int[] DaysInMonths =
            new int[] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        private const int DaysPerYear = 365;
        private const int DaysPerFourCenturies = DaysPerYear * 400 + 97;
        private const int DaysPerCentury = DaysPerYear * 100 + 24;
        private const int DaysPerFourYears = DaysPerYear * 4 + 1;

        private static void CheckYear(int year)
        {
            if (year < 1 || 9999 < year)
                throw new ArgumentException("Year must be in range 1 to 9999.", nameof(year));
        }

        private static void CheckMonth(int month)
        {
            if (month < 1 || 12 < month)
                throw new ArgumentException("Month must be in range 1 to 12.", nameof(month));
        }

        private static void CheckDay(int year, int month, int day)
        {
            int maxDay = DaysInMonth(month, year);

            if (day < 1 || maxDay < day)
                throw new ArgumentException($"Day must be in range 1 to {maxDay}.", nameof(day));
        }

        public static int DaysInMonth(int month, int year)
        {
            CheckYear(year);
            CheckMonth(month);
            
            if (month == 2 && IsLeapYear(year))
                return 29;
            else
                return DaysInMonths[month];
        }

        public static bool IsLeapYear(int year)
        {
            if ((year % 4) != 0)
                return false;
            
            if ((year % 100) != 0)
                return true;
            
            if ((year % 400) != 0)
                return false;
            
            return true;
        }

        private (int year, int remainingDayCount) GetYears(int dayCount)
        {
            int year = 1;
            int remainingDayCount = dayCount;

            if (DaysPerFourCenturies <= remainingDayCount)
            {
                int chunkCount = remainingDayCount / DaysPerFourCenturies;
                year += chunkCount * 400;
                remainingDayCount -= chunkCount * DaysPerFourCenturies;
            }

            if (DaysPerCentury <= remainingDayCount)
            {
                int chunkCount = remainingDayCount / DaysPerCentury;

                if (chunkCount == 4)
                    chunkCount = 3;
                
                year += chunkCount * 100;
                remainingDayCount -= chunkCount * DaysPerCentury;
            }

            if (DaysPerFourYears <= remainingDayCount)
            {
                int chunkCount = remainingDayCount / DaysPerFourYears;
                year += chunkCount * 4;
                remainingDayCount -= chunkCount * DaysPerFourYears;
            }

            if (DaysPerYear <= remainingDayCount)
            {
                int chunkCount = remainingDayCount / DaysPerYear;

                if (chunkCount == 4)
                    chunkCount = 3;
                
                year += chunkCount;
                remainingDayCount -= chunkCount * DaysPerYear;
            }

            return (year, remainingDayCount);
        }

        private (int month, int remainingDayCount) GetMonth(int dayCount, int year)
        {
            int month = 1;
            int remainingDayCount = dayCount;

            while (true)
            {
                int daysInMonth = DaysInMonth(month, year);

                if (remainingDayCount < daysInMonth)
                    break;
                
                ++month;
                remainingDayCount -= daysInMonth;
            }

            return (month, remainingDayCount);
        }

        private readonly int _dayCount;

        internal Date(int dayCount)
        {
            _dayCount = dayCount;
        }

        public Date(int year, int month, int day)
        {
            CheckYear(year);
            CheckMonth(month);
            CheckDay(year, month, day);
            
            _dayCount = day - 1;

            for (int i = 1; i < month; ++i)
                _dayCount += DaysInMonth(i, year);
            
            int y = year - 1;
            _dayCount += (y * DaysPerYear) + (y / 4) - (y / 100) + (y / 400);
        }

        public readonly int Year
        {
            get
            {
                var result = GetYears(_dayCount);
                return result.year;
            }
        }

        public readonly int Month
        {
            get
            {
                var y = GetYears(_dayCount);
                var m = GetMonth(y.remainingDayCount, y.year);
                return m.month;
            }
        }

        public readonly int Day
        {
            get
            {
                var y = GetYears(_dayCount);
                var m = GetMonth(y.remainingDayCount, y.year);
                return m.remainingDayCount + 1;
            }
        }
    }
}