using System;

namespace KellySharp
{
    public static class IntegerMath
    {
        public static int SquareRootBinarySearch(int value)
        {
            if (value < 0)
                throw new ArgumentException("Cannot find square root for negative value.", nameof(value));
            
            if (value == 0)
                return 0;
            
            if (value < 4)
                return 1;
            
            var low = 2;
            var high = value / 2;
            var result = low;

            while (low <= high)
            {
                var mid = (low + high) / 2;
                var square = mid * mid;

                if (square == value)
                {
                    return mid;
                }
                else if (value < square)
                {
                    high = mid - 1;
                }
                else
                {
                    result = mid;
                    low = mid + 1;
                }
            }

            return result;
        }
    }
}
