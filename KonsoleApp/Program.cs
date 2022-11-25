using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KellySharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace KonsoleApp;

#pragma warning disable IDE0051
#pragma warning disable IDE0060

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

        for (int i = 0; i < 256; ++i)
        {
            var p = new Point32(
                random.Next(1, maze.Width - 1),
                random.Next(1, maze.Height - 1));
            var d = (MazeDirection)random.Next(4);
            maze.SetWall(p, d, false);
        }
    }

    static void Main2(string[] args)
    {
        /*
        Sample output from 15 NOV 2020:

        Generated 10000x10000 maze in 00:00:26.0584734.
        Memory: 25069152
        Generated graph with 9795985 edges in 00:00:27.5125985.
        Memory: 1066351808
        */

        void ShowMemoryUsage() => Console.WriteLine("Memory: " + GC.GetTotalMemory(true));
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var maze = new Maze(96, 32);
            var random = new Random();
            RandomizeMaze(maze, random);

            Console.WriteLine($"Generated {maze.Width}x{maze.Height} maze in {stopwatch.Elapsed}.");
            ShowMemoryUsage();

            stopwatch.Restart();
            var edges = MazeGraph.Create(maze);

            if (edges is not null)
            {
                var word = edges.Count == 1 ? "node" : "nodes";
                Console.WriteLine($"Generated graph with {edges.Count} {word} in {stopwatch.Elapsed}.");
            }
            else
            {
                return;
            }
            ShowMemoryUsage();

            // OutputMaze(maze);
            stopwatch.Restart();
            GenerateImages(maze, edges, 8, 24);
            Console.WriteLine($"Generated image in {stopwatch.Elapsed}.");
            ShowMemoryUsage();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    static void Main3(string[] args)
    {
        var sudoku = new SudokuGrid();
        sudoku.TrySetValue(1, 0, 6);
        sudoku.TrySetValue(2, 0, 8);
        sudoku.TrySetValue(3, 0, 0);
        sudoku.TrySetValue(4, 0, 2);
        sudoku.TrySetValue(5, 0, 1);
        sudoku.TrySetValue(7, 0, 7);
        sudoku.TrySetValue(8, 0, 4);

        sudoku.TrySetValue(1, 1, 5);
        sudoku.TrySetValue(3, 1, 4);
        sudoku.TrySetValue(4, 1, 8);
        sudoku.TrySetValue(6, 1, 6);
        sudoku.TrySetValue(0, 2, 4);
        sudoku.TrySetValue(2, 2, 7);
        sudoku.TrySetValue(3, 2, 6);
        sudoku.TrySetValue(6, 2, 1);
        sudoku.TrySetValue(7, 2, 0);

        sudoku.TrySetValue(0, 8, 1);
        sudoku.WriteToConsole();
    }

    static void Main4(string[] args)
    {
        var builder = new RangeSetBuilder();
        while (true)
        {
            Console.Write("LOW: ");
            var lowText = Console.ReadLine();

            if (!int.TryParse(lowText, out var low))
                break;
            
            Console.Write("HIGH: ");
            var highText = Console.ReadLine();

            if (!int.TryParse(highText, out var high))
                break;
            
            builder.AddRange(low, high);
            
            Console.WriteLine(builder);
        }

        Console.WriteLine(builder);
    }

    static async Task ProcessAsync(int value)
    {
        Console.WriteLine($"{DateTime.Now} - Starting {value}");
        await Task.Delay(value);
        Console.WriteLine($"{DateTime.Now} - Ending {value}");
    }

    static async Task Main5(string[] args)
    {
        var values = new int[16];

        for (int i = 0; i < values.Length; ++i)
            values[i] = 1000 + i * 10;
        
        await Async.ChunkAsync(4, values, ProcessAsync);
    }

    static void ShuffleShenanigans(string[] args)
    {
        var boxes = PrisonerRiddle.PrepareBoxes(100, Random.Shared);
        var simplePrisoner = new SimplePrisoner();
        var preparedPrisoner = new PreparedPrisoner();
        for (int i = 0; i < 32; ++i)
        {
            FisherYates.Shuffle(8, boxes, Random.Shared);
            var simpleResult = PrisonerRiddle.CountSuccessfulPrisoners(boxes, simplePrisoner);
            var preparedResult = PrisonerRiddle.CountSuccessfulPrisoners(boxes, preparedPrisoner);

            Console.WriteLine("Simple prisoners: " + simpleResult);
            Console.WriteLine("Prepared prisoners: " + preparedResult);
        }
    }

    static void ChessNonsense(string[] args)
    {
        var piece = ChessPiece.Create(ChessPieceType.Queen, true);
        Console.WriteLine(piece.ToString());
        var board = new ChessBoard();
        var builder = new StringBuilder();
        board.AppendTo(builder);
        Console.WriteLine(builder);
    }

    const string EntryPointEnvVar = "KellySharpEntry";

    static async Task<int> Main(string[] args)
    {
        try
        {
            var entryPoint = Environment.GetEnvironmentVariable(EntryPointEnvVar);

            if (entryPoint is null)
            {
                Console.WriteLine($"Set {EntryPointEnvVar} env var to target method.");
                return 1;
            }

            var methodInfo = typeof(Program).GetMethod(entryPoint, BindingFlags.Static | BindingFlags.NonPublic);

            if (methodInfo is null)
            {
                Console.WriteLine("Failed to locate method: " + entryPoint);
                return 1;
            }

            var result = methodInfo.Invoke(null, new object[] { args });

            if (result is Task<int> taskWithResult)
            {
                return await taskWithResult;
            }
            else if (result is Task task)
            {
                await task;
            }
            
            return 0;
        }
        catch (Exception ex)
        {
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine(ex);
            Console.WriteLine();
            return 1;
        }
    }
}

#pragma warning restore IDE0051
#pragma warning restore IDE0060
