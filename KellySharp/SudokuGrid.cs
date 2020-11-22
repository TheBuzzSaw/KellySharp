using System;
using System.Collections;
using System.Text;

namespace KellySharp
{
    public class SudokuGrid
    {
        private const int NoValues = -1;
        private const int MultipleValues = -2;

        private static int Index(int x, int y) => y * 81 + x * 9;
        private static int Index(int x, int y, int value) => y * 81 + x * 9 + value;

        private readonly BitArray _legalValues = new(9 * 9 * 9, true);

        private int CountLegalValues(int cellIndex)
        {
            int count = 0;
            
            for (int i = 0; i < 9; ++i)
                count += Convert.ToInt32(_legalValues[cellIndex + i]);
            
            return count;
        }

        private int GetValue(int cellIndex)
        {
            var result = NoValues;
            
            for (int i = 0; i < 9; ++i)
            {
                if (_legalValues[cellIndex + i])
                {
                    if (result == NoValues)
                        result = i;
                    else
                        return MultipleValues;
                }
            }

            return result;
        }

        private bool TryBanValue(int x, int y, int value)
        {
            var cellIndex = Index(x, y);
            var valueIndex = cellIndex + value;

            if (_legalValues[valueIndex])
            {
                _legalValues[valueIndex] = false;

                var newValue = GetValue(cellIndex);

                if (newValue == NoValues || ((newValue != MultipleValues && !TrySpreadBan(x, y, newValue))))
                {
                    return false;
                }
            }

            return true;
        }

        private bool TrySpreadBan(int x, int y, int value)
        {
            for (int i = 0; i < 9; ++i)
            {
                if (i != x && !TryBanValue(i, y, value))
                    return false;
            }

            for (int i = 0; i < 9; ++i)
            {
                if (i != y && !TryBanValue(x, i, value))
                    return false;
            }

            var baseX = x - (x % 3);
            var baseY = y - (y % 3);
            
            for (int i = 0; i < 3; ++i)
            for (int j = 0; j < 3; ++j)
            {
                var xx = baseX + j;
                var yy = baseY + i;

                if (xx != x && yy != y && !TryBanValue(xx, yy, value))
                    return false;
            }

            return true;
        }

        public bool TrySetValue(int x, int y, int value)
        {
            var cellIndex = Index(x, y);

            for (int i = 0; i < 9; ++i)
            {
                if (i == value)
                {
                    if (!_legalValues[cellIndex + i])
                        return false;
                }
                else
                {
                    _legalValues[cellIndex + i] = false;
                }
            }
            
            return true;
        }

        private char CellChar(int x, int y)
        {
            var cellIndex = Index(x, y);
            var value = GetValue(cellIndex);
            return value switch
            {
                MultipleValues => '.',
                NoValues => 'x',
                _ => (char)('1' + value)
            };
        }

        public void WriteToConsole()
        {
            var builder = new StringBuilder(18);
            for (int i = 0; i < 9; ++i)
            {
                builder.Length = 0;

                for (int j = 0; j < 9; ++j)
                    builder.Append(' ').Append(CellChar(j, i));
                
                Console.WriteLine(builder);
            }
        }
    }
}
