using System;
using System.Linq;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace KellySharp;

public readonly struct Few<T> : IEquatable<Few<T>>
{
    private readonly ImmutableArray<T> _items;

    public readonly ImmutableArray<T> Items => _items.IsDefault ? ImmutableArray<T>.Empty : _items;

    public Few(ImmutableArray<T> items) => _items = items;
    public bool Equals(Few<T> other) => Items.SequenceEqual(other.Items);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Few<T> other && Equals(other);
    
    public override int GetHashCode()
    {
        if (_items.IsDefaultOrEmpty)
            return 0;
        
        var hashCode = new HashCode();

        foreach (var item in _items)
            hashCode.Add(item);

        var result = hashCode.ToHashCode();
        return result;
    }

    public override string? ToString()
    {
        if (_items.IsDefaultOrEmpty)
            return "[]";
        
        var builder = new StringBuilder("[").Append(Items[0]?.ToString());

        for (int i = 1; i < Items.Length; ++i)
            builder.Append(", ").Append(Items[i]?.ToString());

        var result = builder.Append(']').ToString();
        return result;
    }
}
