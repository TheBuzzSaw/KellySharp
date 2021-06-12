namespace KellySharp
{
    public readonly struct MazeEdge
    {
        public readonly Point32 Destination;
        public readonly int Distance;

        public MazeEdge(Point32 destination, int distance)
        {
            Destination = destination;
            Distance = distance;
        }
    }
}