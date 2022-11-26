using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace KellySharp;

public readonly struct ChessPosition : IEquatable<ChessPosition>
{
    private readonly byte _data;

    public readonly int X => _data & 0x7;
    public readonly int Y => (_data >> 3) & 0x7;

    public ChessPosition(int x, int y)
    {
        Validate(x);
        Validate(y);
        _data = (byte)((y << 3) | x);
    }

    internal void WriteTo(Span<char> span)
    {
        span[0] = (char)('A' + X);
        span[1] = (char)('8' - Y);
    }

    public bool Equals(ChessPosition other) => _data == other._data;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is ChessPosition other && Equals(other);
    public override int GetHashCode() => _data.GetHashCode();
    public override string? ToString() => string.Create(2, this, (span, state) => state.WriteTo(span));

    private static void Validate(
        int value,
        [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        if (value < 0 || 7 < value)
            throw new ArgumentOutOfRangeException(name);
    }

    public static bool operator ==(ChessPosition left, ChessPosition right) => left.Equals(right);
    public static bool operator !=(ChessPosition left, ChessPosition right) => !left.Equals(right);
}
