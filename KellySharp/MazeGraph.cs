using System;
using System.Collections.Generic;

namespace KellySharp
{
    public class MazeGraph
    {
        private static int CountExits(Maze maze, Point32 point)
        {
            return
                Convert.ToInt32(maze.CanGoUp(point.X, point.Y)) +
                Convert.ToInt32(maze.CanGoDown(point.X, point.Y)) +
                Convert.ToInt32(maze.CanGoLeft(point.X, point.Y)) +
                Convert.ToInt32(maze.CanGoRight(point.X, point.Y));
        }

        private static bool CanGo(Maze maze, Point32 point, int direction)
        {
            return direction switch
            {
                0 => maze.CanGoRight(point.X, point.Y),
                1 => maze.CanGoDown(point.X, point.Y),
                2 => maze.CanGoLeft(point.X, point.Y),
                3 => maze.CanGoUp(point.X, point.Y),
                _ => false
            };
        }

        private static Point32 Offset(int direction)
        {
            return direction switch
            {
                0 => new Point32(1, 0),
                1 => new Point32(0, 1),
                2 => new Point32(-1, 0),
                3 => new Point32(0, -1),
                _ => new Point32()
            };
        }

        private static int TurnLeft(int direction) => (direction + 3) & 3;
        private static int TurnRight(int direction) => (direction + 1) & 3;
        private static int TurnAround(int direction) => (direction + 2) & 3;

        public static Dictionary<Point32, MazeEdge[]>? Create(Maze maze)
        {
            var edgesByPosition = new Dictionary<Point32, MazeEdge[]>();

            var start = new Point32();
            int direction = 0;
            int exitCount = CountExits(maze, start);

            if (exitCount < 1)
            {
                return null;
            }

            while (exitCount < 3)
            {
                if (!CanGo(maze, start, direction))
                {
                    if (CanGo(maze, start, TurnLeft(direction)))
                        direction = TurnLeft(direction);
                    else if (CanGo(maze, start, TurnRight(direction)))
                        direction = TurnRight(direction);
                    else
                        direction = TurnAround(direction);
                }

                start += Offset(direction);
                exitCount = CountExits(maze, start);
            }

            var pending = new Stack<Point32>();
            edgesByPosition.Add(start, new MazeEdge[4]);
            pending.Push(start);

            while (pending.TryPop(out var position))
            {
                var edges = edgesByPosition[position];

                for (int i = 0; i < 4; ++i)
                {
                    if (edges[i].Distance == 0 && CanGo(maze, position, i))
                    {
                        direction = i;
                        
                        int distance = 1;
                        var explorer = position + Offset(direction);
                        exitCount = CountExits(maze, explorer);

                        while (exitCount == 2)
                        {
                            if (CanGo(maze, explorer, TurnLeft(direction)))
                            {
                                direction = TurnLeft(direction);
                            }
                            else if (CanGo(maze, explorer, TurnRight(direction)))
                            {
                                direction = TurnRight(direction);
                            }

                            ++distance;
                            explorer += Offset(direction);
                            exitCount = CountExits(maze, explorer);
                        }

                        edges[i] = new MazeEdge(explorer, distance);

                        if (2 < exitCount)
                        {
                            var explorerEdges = new MazeEdge[4];
                            explorerEdges[TurnAround(direction)] = new MazeEdge(position, distance);
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