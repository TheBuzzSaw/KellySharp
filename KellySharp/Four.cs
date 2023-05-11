using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace KellySharp;

public static class Four
{
    private static void ThrowBadIndex(string? paramName) => throw new ArgumentOutOfRangeException(paramName);
    private static void CheckIndex(int index, [CallerArgumentExpression(nameof(index))] string? paramName = null)
    {
        if (3 < (uint)index)
            ThrowBadIndex(paramName);
    }

    public static T Read<T>(in this Four<T> four, int index) where T : unmanaged
    {
        CheckIndex(index);
        return Unsafe.Add(ref Unsafe.AsRef(four.item0), index);
    }

    public static void Write<T>(ref this Four<T> four, int index, T value) where T : unmanaged
    {
        CheckIndex(index);
        Unsafe.Add(ref four.item0, index) = value;
    }

    public static Span<T> AsSpan<T>(ref this Four<T> four) where T : unmanaged
    {
        unsafe
        {
            fixed (void* a = &four.item0)
                return new Span<T>(a, 4);
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct Four<T> : IEquatable<Four<T>> where T : unmanaged
{
    public T item0;
    public T item1;
    public T item2;
    public T item3;

    public bool Equals(Four<T> other)
    {
        var ec = EqualityComparer<T>.Default;

        return
            ec.Equals(item0, other.item0) &&
            ec.Equals(item1, other.item1) &&
            ec.Equals(item2, other.item2) &&
            ec.Equals(item3, other.item3);
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Four<T> other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(item0, item1, item2, item3);
    public override string? ToString() => $"[{item0}, {item1}, {item2}, {item3}]";
}