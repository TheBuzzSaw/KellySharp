using System;
using System.Collections;

namespace KellySharp
{
    public class Maze
    {
        private readonly BitArray _walls;
        private readonly int _verticalWallWidth;
        private readonly int _verticalWallHeight;
        private readonly int _verticalWallCount;
        private readonly int _horizontalWallWidth;
        private readonly int _horizontalWallHeight;
        private readonly int _horizontalWallCount;

        public int Width { get; }
        public int Height { get; }

        public Maze(int edge) : this(edge, edge)
        {
        }

        public Maze(int width, int height)
        {
            if (width < 1)
                throw new ArgumentException("Must be at least 1.", nameof(width));
            
            if (height < 1)
                throw new ArgumentException("Must be at least 1", nameof(height));
            
            Width = width;
            Height = height;
            _verticalWallWidth = width - 1;
            _verticalWallHeight = height;
            _verticalWallCount = _verticalWallWidth * _verticalWallHeight;
            _horizontalWallWidth = width;
            _horizontalWallHeight = height - 1;
            _horizontalWallCount = _horizontalWallWidth * _horizontalWallHeight;
            _walls = new BitArray(_verticalWallCount + _horizontalWallCount);
        }

        private int VerticalWallIndex(int x, int y) => _verticalWallWidth * y + x;
        private int HorizontalWallIndex(int x, int y) => _verticalWallCount + _horizontalWallWidth * y + x;

        public bool CanGoDown(int x, int y) => CanGoUp(x, y + 1);
        public bool CanGoUp(int x, int y)
        {
            return 0 <= x && x < Width && 0 < y && y < Height && !_walls[HorizontalWallIndex(x, y - 1)];
        }

        public bool CanGoRight(int x, int y) => CanGoLeft(x + 1, y);
        public bool CanGoLeft(int x, int y)
        {
            return 0 <= y && y < Height && 0 < x && x < Width && !_walls[VerticalWallIndex(x - 1, y)];
        }

        public void SetWallDown(int x, int y, bool wall) => SetWallUp(x, y + 1, wall);
        public void SetWallUp(int x, int y, bool wall)
        {
            if (0 <= x && x < Width && 0 < y && y < Height)
            {
                _walls[HorizontalWallIndex(x, y - 1)] = wall;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public void SetWallRight(int x, int y, bool wall) => SetWallLeft(x + 1, y, wall);
        public void SetWallLeft(int x, int y, bool wall)
        {
            if (0 <= y && y < Height && 0 < x && x < Width)
            {
                _walls[VerticalWallIndex(x - 1, y)] = wall;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}