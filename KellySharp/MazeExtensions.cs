using System;

namespace KellySharp
{
    public static class MazeExtensions
    {
        public static MazeDirection TurnLeft(this MazeDirection mazeDirection)
        {
            return (MazeDirection)(((int)mazeDirection + 3) & 3);
        }

        public static MazeDirection TurnRight(this MazeDirection mazeDirection)
        {
            return (MazeDirection)(((int)mazeDirection + 1) & 3);
        }

        public static MazeDirection TurnAround(this MazeDirection mazeDirection)
        {
            return (MazeDirection)(((int)mazeDirection + 2) & 3);
        }

        public static Point32 AsOffset(this MazeDirection mazeDirection)
        {
            return mazeDirection switch
            {
                MazeDirection.Right => new Point32(1, 0),
                MazeDirection.Down => new Point32(0, 1),
                MazeDirection.Left => new Point32(-1, 0),
                MazeDirection.Up => new Point32(0, -1),
                _ => new Point32()
            };
        }

        public static bool CanGo(this Maze maze, MazeDirection mazeDirection, Point32 position)
        {
            return mazeDirection switch
            {
                MazeDirection.Right => maze.CanGoRight(position.X, position.Y),
                MazeDirection.Down => maze.CanGoDown(position.X, position.Y),
                MazeDirection.Left => maze.CanGoLeft(position.X, position.Y),
                MazeDirection.Up => maze.CanGoUp(position.X, position.Y),
                _ => false
            };
        }

        public static void SetWall(
            this Maze maze,
            Point32 position,
            MazeDirection mazeDirection,
            bool wall)
        {
            switch (mazeDirection)
            {
                case MazeDirection.Right: maze.SetWallRight(position.X, position.Y, wall); break;
                case MazeDirection.Down: maze.SetWallDown(position.X, position.Y, wall); break;
                case MazeDirection.Left: maze.SetWallLeft(position.X, position.Y, wall); break;
                case MazeDirection.Up: maze.SetWallUp(position.X, position.Y, wall); break;
                default: break;
            }
        }

        public static int CountExitsAt(this Maze maze, Point32 point)
        {
            return
                Convert.ToInt32(maze.CanGoUp(point.X, point.Y)) +
                Convert.ToInt32(maze.CanGoDown(point.X, point.Y)) +
                Convert.ToInt32(maze.CanGoLeft(point.X, point.Y)) +
                Convert.ToInt32(maze.CanGoRight(point.X, point.Y));
        }
    }
}
