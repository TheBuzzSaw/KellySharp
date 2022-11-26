using System;
using System.Diagnostics.CodeAnalysis;

namespace KellySharp;

public readonly struct ChessMove : IEquatable<ChessMove>
{
    public readonly ChessPosition From { get; }
    public readonly ChessPosition To { get; }
    
    public ChessMove(ChessPosition from, ChessPosition to)
    {
        From = from;
        To = to;
    }

    private void WriteTo(Span<char> span)
    {
        From.WriteTo(span);
        " to ".AsSpan().CopyTo(span[2..]);
        To.WriteTo(span[6..]);
    }

    public bool Equals(ChessMove other) => From == other.From && To == other.To;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is ChessMove other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(From, To);
    public override string? ToString() => string.Create(8, this, (span, state) => state.WriteTo(span));

    public static bool operator ==(ChessMove left, ChessMove right) => left.Equals(right);
    public static bool operator !=(ChessMove left, ChessMove right) => !left.Equals(right);
}