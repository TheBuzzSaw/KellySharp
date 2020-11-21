using System;

namespace KellySharp
{
    public readonly struct MazeEdge : IEquatable<MazeEdge>
    {
        public readonly Point32 StartPosition { get; }
        public readonly Point32 FinishPosition { get; }
        public readonly int Distance { get; }
        private readonly int _directions;
        public readonly MazeDirection StartDirection => (MazeDirection)(_directions & 3);
        public readonly MazeDirection FinishDirection => (MazeDirection)((_directions >> 2) & 3);

        public MazeEdge(
            MazeExit start,
            MazeExit finish,
            int distance)
        {
            StartPosition = start.Position;
            FinishPosition = finish.Position;
            _directions = ((int)finish.Direction << 2) | (int)start.Direction;
            Distance = distance;
        }

        public readonly MazeEdge Reversed()
        {
            return new MazeEdge(
                new MazeExit(FinishPosition, FinishDirection),
                new MazeExit(StartPosition, StartDirection),
                Distance);
        }

        public bool Equals(MazeEdge other)
        {
            return
                StartPosition == other.StartPosition &&
                FinishPosition == other.FinishPosition &&
                Distance == other.Distance &&
                _directions == other._directions;
        }

        public override bool Equals(object? obj) => obj is MazeEdge other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(StartPosition, FinishPosition);
        public override string? ToString()
        {
            var startExit = new MazeExit(StartPosition, StartDirection);
            var finishExit = new MazeExit(FinishPosition, FinishDirection);
            return $"from {startExit} to {finishExit}";
        }

        public int CompareTo(MazeEdge other)
        {
            var result = Compare(StartPosition, other.StartPosition);
            if (result == 0)
            {
                result = StartDirection.CompareTo(other.StartDirection);
            }

            return result;
        }

        public static int Compare(Point32 a, Point32 b)
        {
            var result = a.Y.CompareTo(b.Y);
            return result == 0 ? a.X.CompareTo(b.X) : result;
        }
    }
}