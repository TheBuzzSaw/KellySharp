using System;
using System.Diagnostics.CodeAnalysis;

namespace KellySharp
{
    public static class Money
    {
        private static char Digit(long n) => (char)('0' + n);
        
        public static string GetString(long totalPennyCount)
        {
            Span<char> span = stackalloc char[28];
            bool isNegative = totalPennyCount < 0;
            long multiplier = isNegative ? -1 : 1;
            long denominator = 100 * multiplier;
            long dollarCount = totalPennyCount / denominator;
            long pennyCount = (totalPennyCount - dollarCount * denominator) * multiplier;
            int index = span.Length;

            span[--index] = Digit(pennyCount % 10);
            span[--index] = Digit(pennyCount / 10);
            span[--index] = '.';

            if (dollarCount == 0)
            {
                span[--index] = '0';
            }
            else
            {
                int three = 3;
                for (var n = dollarCount; 0 < n; n /= 10, --three)
                {
                    if (three == 0)
                    {
                        span[--index] = ',';
                        three = 3;
                    }

                    span[--index] = Digit(n % 10);
                }
            }

            span[--index] = '$';
            
            if (isNegative)
                span[--index] = '-';

            return new string(span.Slice(index));
        }
    }
}