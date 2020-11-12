using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using KellySharp;

namespace KonsoleApp
{
    class Program
    {
        static void OutputMaze(Maze maze)
        {
            for (int x = 0; x < maze.Width; ++x)
            {
                Console.Write("+--");
            }

            Console.WriteLine("+");

            for (int y = 0; y < maze.Height; ++y)
            {
                for (int x = 0; x < maze.Width; ++x)
                {
                    Console.Write(maze.CanGoLeft(x, y) ? " " : "|");
                    Console.Write("  ");
                }

                Console.WriteLine("|");

                for (int x = 0; x < maze.Width; ++x)
                {
                    Console.Write("+");
                    Console.Write(maze.CanGoDown(x, y) ? "  " : "--");
                }

                Console.WriteLine("+");
            }
        }

        static void RandomizeMaze(Maze maze, Random random)
        {
            maze.SetAllWalls(true);
            var visited = new BitArray(maze.Width * maze.Height);
            int VisitedIndex(int x, int y) => y * maze.Width + x;
            bool WasVisited(int x, int y) => visited[VisitedIndex(x, y)];
            void Visit(int x, int y) => visited[VisitedIndex(x, y)] = true;
            var trail = new Stack<(int x, int y)>();
            var lastX = maze.Width - 1;
            var lastY = maze.Height - 1;
            trail.Push((random.Next(maze.Width), random.Next(maze.Height)));

            while (trail.TryPop(out var position))
            {
                Visit(position.x, position.y);
                var nextPosition = position;
                var direction = random.Next(4);
                bool proceed = false;

                for (int i = 0; !proceed && i < 4; ++i)
                {
                    direction = (direction + i) & 3;
                    switch (direction)
                    {
                        case 0:
                            if (0 < position.x && !WasVisited(position.x - 1, position.y))
                            {
                                --nextPosition.x;
                                maze.SetWallLeft(position.x, position.y, false);
                                proceed = true;
                            }
                            break;
                        
                        case 1:
                            if (position.x < lastX && !WasVisited(position.x + 1, position.y))
                            {
                                ++nextPosition.x;
                                maze.SetWallRight(position.x, position.y, false);
                                proceed = true;
                            }
                            break;
                        
                        case 2:
                            if (0 < position.y && !WasVisited(position.x, position.y - 1))
                            {
                                --nextPosition.y;
                                maze.SetWallUp(position.x, position.y, false);
                                proceed = true;
                            }
                            break;
                        
                        case 3:
                            if (position.y < lastY && !WasVisited(position.x, position.y + 1))
                            {
                                ++nextPosition.y;
                                maze.SetWallDown(position.x, position.y, false);
                                proceed = true;
                            }
                            break;
                    }
                }

                if (proceed)
                {
                    trail.Push(position);
                    trail.Push(nextPosition);
                }
            }
        }

        static void Main(string[] args)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var maze = new Maze(16);
                var random = new Random();
                RandomizeMaze(maze, random);

                Console.WriteLine($"Generated {maze.Width}x{maze.Height} maze in {stopwatch.Elapsed}.");

                OutputMaze(maze);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
