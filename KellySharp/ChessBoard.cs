using System;
using System.Text;

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
    private static byte Combine(ChessPiece first, ChessPiece second)
    {
        return (byte)(first.Data | (second.Data << 4));
    }
    private static void SetDefaults(Span<byte> row, bool isBlack)
    {
        row[0] = Combine(
            ChessPiece.Create(ChessPieceType.Rook, isBlack),
            ChessPiece.Create(ChessPieceType.Knight, isBlack));
        row[1] = Combine(
            ChessPiece.Create(ChessPieceType.Bishop, isBlack),
            ChessPiece.Create(ChessPieceType.Queen, isBlack));
        row[2] = Combine(
            ChessPiece.Create(ChessPieceType.King, isBlack),
            ChessPiece.Create(ChessPieceType.Bishop, isBlack));
        row[3] = Combine(
            ChessPiece.Create(ChessPieceType.Knight, isBlack),
            ChessPiece.Create(ChessPieceType.Rook, isBlack));
    }
    private static readonly byte WhitePawns = Combine(ChessPiece.WhitePawn, ChessPiece.WhitePawn);
    private static readonly byte BlackPawns = Combine(ChessPiece.BlackPawn, ChessPiece.BlackPawn);
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

    public ChessBoard()
    {
        SetDefaults(_data, true);
        _data.AsSpan(4, 4).Fill(BlackPawns);
        _data.AsSpan(24, 4).Fill(WhitePawns);
        SetDefaults(_data.AsSpan(28), false);
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
        for (int i = 0; i < _data.Length; ++i)
        {
            var b = _data[i];
            var pieceA = new ChessPiece(b & 0xf);
            var pieceB = new ChessPiece((b >> 4) & 0xf);
            AppendTo(builder, pieceA);
            AppendTo(builder, pieceB);

            if ((i & 3) == 3)
                builder.AppendLine();
        }
    }
} 