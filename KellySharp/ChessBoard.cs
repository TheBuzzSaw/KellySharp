using System;
using System.Text;

namespace KellySharp;

public class ChessBoard
{
    private static readonly ChessPieceType[] DefaultRow =
        new ChessPieceType[]
        {
            ChessPieceType.Rook,
            ChessPieceType.Knight,
            ChessPieceType.Bishop,
            ChessPieceType.Queen,
            ChessPieceType.King,
            ChessPieceType.Bishop,
            ChessPieceType.Knight,
            ChessPieceType.Rook
        };
    
    private static readonly int[] DefaultBoard = new int[]
    {
        MakeDefaultRow(true),
        MakePawnRow(true),
        0,
        0,
        0,
        0,
        MakePawnRow(false),
        MakeDefaultRow(false)
    };
    private static void Validate(int x)
    {
        if (x < 0 || 7 < x)
            throw new ArgumentOutOfRangeException(nameof(x));
    }

    private static int MakeDefaultRow(bool isBlack)
    {
        int result = 0;
        for (int i = 0; i < DefaultRow.Length; ++i)
        {
            var chessPiece = ChessPiece.Create(DefaultRow[i], isBlack);
            result |= chessPiece.Data << (i * 4);
        }
        return result;
    }

    private static int MakePawnRow(bool isBlack)
    {
        var chessPiece = ChessPiece.Create(ChessPieceType.Pawn, isBlack);
        int result = 0;
        for (int i = 0; i < 8; ++i)
        {
            result |= chessPiece.Data << (i * 4);
        }
        return result;
    }
    private readonly int[] _rows = new int[8];

    public ChessPiece this[int x, int y]
    {
        get
        {
            Validate(x);
            var offset = x * 4;
            var data = (_rows[y] >> offset) & 0xf;
            return new ChessPiece(data);
        }

        set
        {
            Validate(x);
            var offset = x * 4;
            var mask = ~(0xf << offset);
            _rows[y] = (_rows[y] & mask) | (value.Data << offset);
        }
    }

    public ChessBoard()
    {
        DefaultBoard.AsSpan().CopyTo(_rows);
    }

    private static void AppendTo(StringBuilder builder, ChessPiece chessPiece)
    {
        if (chessPiece.Type == ChessPieceType.None)
        {
            builder.Append("[  ]");
        }
        else
        {
            var piece = chessPiece.Type switch
            {
                ChessPieceType.King => 'K',
                ChessPieceType.Queen => 'Q',
                ChessPieceType.Bishop => 'B',
                ChessPieceType.Knight => 'N',
                ChessPieceType.Rook => 'R',
                ChessPieceType.Pawn => 'P',
                _ => throw new ArgumentException("Invalid piece type")
            };
            var side = chessPiece.IsBlack ? 'B' : 'W';
            builder.Append('[').Append(side).Append(piece).Append(']');
        }
    }

    public void AppendTo(StringBuilder builder)
    {
        for (int i = 0; i < 8; ++i)
            builder.Append("   ").Append((char)('A' + i));

        builder.AppendLine();
        var rowName = '8';
        foreach (var row in _rows)
        {
            builder.Append(rowName).Append(' ');
            for (int x = 0; x < 8; ++x)
            {
                var data = (row >> (x * 4)) & 0xf;
                var chessPiece = new ChessPiece(data);
                AppendTo(builder, chessPiece);
            }

            builder.Append(' ').Append(rowName--).AppendLine();
        }

        for (int i = 0; i < 8; ++i)
            builder.Append("   ").Append((char)('A' + i));
        
        builder.AppendLine();
    }
} 