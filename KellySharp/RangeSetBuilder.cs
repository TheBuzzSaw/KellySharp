using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace KellySharp;

public class RangeSetBuilder
{
    private static bool IsEven(int n) => (n & 1) == 0;

    private readonly List<int> _ranges = new();

    public bool Contains(int value)
    {
        var span = CollectionsMarshal.AsSpan(_ranges);
        var index = Bisect.Right(span, value);
        return (index & 1) == 1;
    }

    public RangeSetBuilder AddRange(int low, int high)
    {
        if (high <= low)
            return this;
        
        var span = CollectionsMarshal.AsSpan(_ranges);
        var lowIndex = Bisect.Left(span, low);

        if (lowIndex == span.Length)
        {
            _ranges.Add(low);
            _ranges.Add(high);
            return this;
        }

        var highIndex = Bisect.Right(span, high);

        if (lowIndex == highIndex && IsEven(lowIndex))
        {
            _ranges.Insert(lowIndex, high);
            _ranges.Insert(lowIndex, low);
            return this;
        }

        var lowBaseIndex = lowIndex & ~1;
        span[lowBaseIndex] = Math.Min(span[lowBaseIndex], low);
        span[lowBaseIndex + 1] = IsEven(highIndex) ? high : span[highIndex];
        var removeCount = (highIndex - lowBaseIndex - 1) & ~1;
        _ranges.RemoveRange(lowBaseIndex + 2, removeCount);

        // 0 3 5 9 B F
        //L   H
        //0 0 0 2 2 4 4
        //0 1 2 3 4 5 6
        //  0 1 2 3 4 5

        return this;
    }

    public override string? ToString()
    {
        if (_ranges.Count == 0)
            return "[]";
        
        var builder = new StringBuilder()
            .Append('[')
            .Append(_ranges[0])
            .Append(" to ")
            .Append(_ranges[1]);
        
        for (int i = 2; i < _ranges.Count; i += 2)
        {
            builder
                .Append(", ")
                .Append(_ranges[i])
                .Append(" to ")
                .Append(_ranges[i + 1]);
        }

        return builder.Append(']').ToString();
    }
}