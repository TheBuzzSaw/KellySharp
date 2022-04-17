using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace KellySharp;

public readonly struct RangeSet : IEquatable<RangeSet>
{
    private static void Validate(ReadOnlySpan<int> values)
    {
        if ((values.Length & 1) == 1)
            throw new InvalidOperationException("Range set requires even number of values.");
        
        for (int i = 1; i < values.Length; ++i)
        {
            if (values[i] <= values[i - 1])
                throw new InvalidOperationException("Range values must be in ascending order.");
        }
    }

    public static RangeSet Create(params int[] values)
    {
        if (values is null)
            return default;
        
        Validate(values);

        var ranges = ImmutableArray.Create(values);
        return new RangeSet(ranges);
    }

    public static RangeSet Create(ReadOnlySpan<int> values)
    {
        Validate(values);
        var builder = ImmutableArray.CreateBuilder<int>(values.Length);

        foreach (var value in values)
            builder.Add(value);
        
        return new RangeSet(builder.MoveToImmutable());
    }

    private readonly ImmutableArray<int> _ranges;

    internal RangeSet(ImmutableArray<int> ranges) => _ranges = ranges;

    public bool Contains(int value)
    {
        var index = Bisect.Right(_ranges.AsSpan(), value);
        return (index & 1) == 1;
    }

    public bool Equals(RangeSet other) => _ranges.SequenceEqual(other._ranges);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is RangeSet other && Equals(other);
    public override int GetHashCode()
    {
        if (_ranges.IsDefaultOrEmpty)
            return 0;
        
        var hashCode = new HashCode();
        
        foreach (var boundary in _ranges)
            hashCode.Add(boundary);
        
        return hashCode.ToHashCode();
    }

    public override string? ToString()
    {
        if (_ranges.IsDefaultOrEmpty)
            return "[]";
        var builder = new StringBuilder()
            .Append('[')
            .Append(_ranges[0])
            .Append(" to ")
            .Append(_ranges[1]);
        
        for (int i = 2; i < _ranges.Length; i += 2)
        {
            builder
                .Append(", ")
                .Append(_ranges[i])
                .Append(" to ")
                .Append(_ranges[i + 1]);
        }
        
        return builder.Append(']').ToString();
    }

    public static bool operator == (RangeSet a, RangeSet b) => a.Equals(b);
    public static bool operator != (RangeSet a, RangeSet b) => !a.Equals(b);
}