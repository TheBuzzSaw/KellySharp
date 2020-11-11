using System;
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
        static void Main(string[] args)
        {
            try
            {
                var maze = new Maze(16);
                var random = new Random();

                bool GetWall() => random.Next(3) == 0;

                var stopwatch = Stopwatch.StartNew();
                for (int x = 1; x < maze.Width; ++x)
                {
                    maze.SetWallLeft(x, 0, GetWall());
                }

                for (int y = 1; y < maze.Height; ++y)
                {
                    maze.SetWallUp(0, y, GetWall());

                    for (int x = 1; x < maze.Width; ++x)
                    {
                        maze.SetWallUp(x, y, GetWall());
                        maze.SetWallLeft(x, y, GetWall());
                    }
                }

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
