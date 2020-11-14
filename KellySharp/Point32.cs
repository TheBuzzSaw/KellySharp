using System;

namespace KellySharp
{
    public readonly struct Point32 : IEquatable<Point32>
    {
        public readonly int X;
        public readonly int Y;

        public Point32(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Point32 other) => X == other.X && Y == other.Y;
        public override bool Equals(object? obj) => obj is Point32 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override string? ToString() => $"{X}, {Y}";

        public static bool operator ==(Point32 left, Point32 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point32 left, Point32 right)
        {
            return !(left == right);
        }

        public static Point32 operator +(Point32 left, Point32 right)
        {
            return new Point32(left.X + right.X, left.Y + right.Y);
        }

        public static Point32 operator -(Point32 left, Point32 right)
        {
            return new Point32(left.X - right.X, left.Y - right.Y);
        }
    }
}