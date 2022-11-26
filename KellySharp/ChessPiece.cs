using System;
using System.Diagnostics.CodeAnalysis;

namespace KellySharp;

public readonly struct ChessPiece : IEquatable<ChessPiece>
{
    private const int SideMask = 1 << 3;
    private const int TypeMask = 7;
    private readonly int _data;

    public bool IsBlack => (_data & SideMask) == SideMask;
    public bool IsWhite => !IsBlack;
    public ChessPieceType Type => (ChessPieceType)(_data & TypeMask);
    internal int Data => _data;

    internal ChessPiece(int data) => _data = data;
    public bool Equals(ChessPiece other) => _data == other._data;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is ChessPiece other && Equals(other);
    public override int GetHashCode() => _data.GetHashCode();
    public override string? ToString()
    {
        if (Type == ChessPieceType.None)
            return "";

        var color = IsWhite ? "white " : "black ";
        return color + Type.ToString().ToLowerInvariant();
    }

    public static ChessPiece Create(ChessPieceType type, bool isBlack)
    {
        if (type == ChessPieceType.None)
            return default;
        
        var mask = isBlack ? 1 << 3 : 0;
        return new ChessPiece(mask | (int)type);
    }

    public static bool operator ==(ChessPiece left, ChessPiece right) => left.Equals(right);
    public static bool operator !=(ChessPiece left, ChessPiece right) => !left.Equals(right);
}