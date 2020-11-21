using System;

namespace KellySharp
{
    public readonly struct MazeExit : IEquatable<MazeExit>
    {
        public readonly Point32 Position;
        public readonly MazeDirection Direction;

        public MazeExit(Point32 position, MazeDirection direction)
        {
            Position = position;
            Direction = direction;
        }

        public bool Equals(MazeExit other) => Position == other.Position && Direction == other.Direction;
        public override bool Equals(object? obj) => obj is MazeExit other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Position.X, Position.Y, Direction);
        public override string? ToString() => $"({Position}):{Direction}";
    }
}