using System;

namespace KellySharp;

public class ChessBoard
{
    private static void Validate(int x, int y)
    {
        if (x < 0 || 7 < x)
            throw new ArgumentOutOfRangeException(nameof(x));
        
        if (y < 0 || 7 < y)
            throw new ArgumentOutOfRangeException(nameof(y));
    }
    private static int Index(int x, int y) => y * 8 + x;
    private static int OuterIndex(int index) => index >> 1;
    private static int Offset(int index) => (index & 1) * 4;
    private readonly byte[] _data = new byte[32];

    public ChessPiece this[int x, int y]
    {
        get
        {
            Validate(x, y);
            var index = Index(x, y);
            var outerIndex = OuterIndex(index);
            var offset = Offset(index);
            var data = (_data[outerIndex] >> offset) & 0xf;
            return new ChessPiece(data);
        }

        set
        {
            Validate(x, y);
            var index = Index(x, y);
            var outerIndex = OuterIndex(index);
            var offset = Offset(index);
            var mask = ~(0xf << offset);
            _data[outerIndex] = (byte)((_data[outerIndex] & mask) | (value.Data << offset));
        }
    }
}