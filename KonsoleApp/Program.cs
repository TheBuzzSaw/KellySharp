using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using KellySharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace KonsoleApp
{
    class Program
    {
        static void GenerateImages(
            Maze maze,
            IDictionary<Point32, MazeEdge[]> edges,
            int wallThickness,
            int openSpace)
        {
            var cellSize = wallThickness + openSpace;
            using var image = new Image<Rgb24>(
                maze.Width * cellSize + wallThickness,
                maze.Height * cellSize + wallThickness,
                new Rgb24(255, 255, 255));
            
            var black = new Rgb24();
            var lastCellX = maze.Width * cellSize;
            var lastCellY = maze.Height * cellSize;
            var lastX = maze.Width - 1;
            var lastY = maze.Height - 1;
            
            for (int y = 0; y < maze.Height; ++y)
            {
                var imageY = y * cellSize;
                for (int x = 0; x < maze.Width; ++x)
                {
                    var imageX = x * cellSize;
                    image.PaintBox(
                        imageX,
                        imageY,
                        wallThickness,
                        wallThickness,
                        black);

                    if (!maze.CanGoUp(x, y))
                    {
                        image.PaintBox(
                            imageX + wallThickness,
                            imageY,
                            openSpace,
                            wallThickness,
                            black);
                    }

                    if (!maze.CanGoLeft(x, y))
                    {
                        image.PaintBox(
                            imageX,
                            imageY + wallThickness,
                            wallThickness,
                            openSpace,
                            black);
                    }
                }

                if (!maze.CanGoRight(lastX, y))
                {
                    image.PaintBox(
                        lastCellX,
                        imageY,
                        wallThickness,
                        cellSize,
                        black);
                }
            }

            for (int x = 0; x < maze.Width; ++x)
            {
                if (!maze.CanGoDown(x, lastY))
                {
                    image.PaintBox(
                        cellSize * x,
                        lastCellY,
                        cellSize,
                        wallThickness,
                        black);
                }
            }

            image.PaintBox(
                lastCellX,
                lastCellY,
                wallThickness,
                wallThickness,
                black);
                
            var encoder = new PngEncoder
            {
                BitDepth = PngBitDepth.Bit1
            };

            image.SaveAsPng("maze.original.png");

            foreach (var position in edges.Keys)
            {
                image.PaintBox(
                    position.X * cellSize + wallThickness,
                    position.Y * cellSize + wallThickness,
                    openSpace,
                    openSpace,
                    new Rgb24(255, 0, 0));
            }

            image.SaveAsPng("maze.nodes.png");
        }
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
            var randomX = random.Next(maze.Width);
            var randomY = random.Next(maze.Height);
            trail.Push((randomX, randomY));
            Visit(randomX, randomY);

            while (trail.TryPop(out var position))
            {
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
                    Visit(nextPosition.x, nextPosition.y);
                }
            }
        }

        static void Main(string[] args)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var maze = new Maze(64);
                var random = new Random();
                RandomizeMaze(maze, random);

                Console.WriteLine($"Generated {maze.Width}x{maze.Height} maze in {stopwatch.Elapsed}.");

                stopwatch.Restart();
                var edges = MazeGraph.Create(maze);

                if (edges is not null)
                {
                    var word = edges.Count == 1 ? "edge" : "edges";
                    Console.WriteLine($"Generated graph with {edges.Count} {word} in {stopwatch.Elapsed}.");
                }

                // OutputMaze(maze);
                stopwatch.Restart();
                GenerateImages(maze, edges, 8, 24);
                Console.WriteLine($"Generated image in {stopwatch.Elapsed}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
