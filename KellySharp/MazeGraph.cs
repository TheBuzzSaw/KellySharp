using System;
using System.Collections.Generic;

namespace KellySharp
{
    public class MazeGraph
    {
        public static Dictionary<Point32, MazeEdge[]>? Create(Maze maze)
        {
            var edgesByPosition = new Dictionary<Point32, MazeEdge[]>();

            var start = new Point32();
            var direction = default(MazeDirection);
            int exitCount = maze.CountExitsAt(start);

            if (exitCount < 1)
            {
                return null;
            }

            while (exitCount < 3)
            {
                if (!maze.CanGo(direction, start))
                {
                    if (maze.CanGo(direction.TurnLeft(), start))
                        direction = direction.TurnLeft();
                    else if (maze.CanGo(direction.TurnRight(), start))
                        direction = direction.TurnRight();
                    else
                        direction = direction.TurnAround();
                }

                start += direction.AsOffset();
                exitCount = maze.CountExitsAt(start);
            }

            var pending = new Stack<Point32>();
            edgesByPosition.Add(start, new MazeEdge[4]);
            pending.Push(start);

            while (pending.TryPop(out var position))
            {
                var edges = edgesByPosition[position];

                for (int i = 0; i < 4; ++i)
                {
                    direction = (MazeDirection)i;
                    if (edges[i].Distance == 0 && maze.CanGo(direction, position))
                    {
                        
                        int distance = 1;
                        var explorer = position + direction.AsOffset();
                        exitCount = maze.CountExitsAt(explorer);

                        while (exitCount == 2)
                        {
                            if (maze.CanGo(direction.TurnLeft(), explorer))
                                direction = direction.TurnLeft();
                            else if (maze.CanGo(direction.TurnRight(), explorer))
                                direction = direction.TurnRight();

                            ++distance;
                            explorer += direction.AsOffset();
                            exitCount = maze.CountExitsAt(explorer);
                        }

                        edges[i] = new MazeEdge(explorer, distance);

                        if (2 < exitCount)
                        {
                            var explorerEdges = new MazeEdge[4];
                            explorerEdges[(int)direction.TurnAround()] = new MazeEdge(position, distance);
                            edgesByPosition.Add(explorer, explorerEdges);
                            pending.Push(explorer);
                        }
                    }
                }
            }

            return edgesByPosition;
        }
    }
}
