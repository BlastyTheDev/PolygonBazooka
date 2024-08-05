using System;
using System.Collections.Generic;
using System.Numerics;
using PolygonBazooka.Util;

namespace PolygonBazooka.Elements;

public class Player
{
    private readonly TileType[,] _board;
    private TileType[,] _oldBoard;

    private TileType _fallingBlockOrigin;
    private TileType _fallingBlockOrbit;

    private int _xOrigin;
    private int _yOrigin;
    private int _xOrbit;
    private int _yOrbit;

    private TileType _nextBlockOrigin;
    private TileType _nextBlockOrbit;
    private TileType _nextNextBlockOrigin;
    private TileType _nextNextBlockOrbit;

    public long LastClear;

    public void Clear()
    {
        List<Vector2> clearedTiles = new();

        // clear horizontally
        for (int row = 0; row < Const.Rows; row++)
        {
            for (int col = 0; col < Const.Cols; col++)
            {
                if (_board[row, col] == TileType.Empty || _board[row, col] == TileType.Bonus ||
                    _board[row, col] == TileType.Garbage)
                    continue;

                int matchLength = 1;
                int lastColourMatch = col;
                TileType current = _board[row, col];

                for (int nextCol = col + 1; nextCol < Const.Cols; nextCol++)
                {
                    if (_board[row, nextCol] == current || _board[row, nextCol] == TileType.Bonus)
                    {
                        matchLength++;
                        if (_board[row, nextCol] == current)
                            lastColourMatch = nextCol;
                    }
                    else
                        break;
                }

                if (_board[row, col + matchLength - 1] == TileType.Bonus)
                    matchLength--;

                if (matchLength >= 3 && lastColourMatch > col + 1)
                {
                    for (int i = 0; i < matchLength; i++)
                    {
                        if (col + i <= lastColourMatch)
                            clearedTiles.Add(new Vector2(col + i, row));
                    }
                }
            }
        }

        // clear vertically
        for (int col = 0; col < Const.Cols; col++)
        {
            for (int row = 0; row < Const.Rows; row++)
            {
                if (_board[row, col] == TileType.Empty || _board[row, col] == TileType.Bonus ||
                    _board[row, col] == TileType.Garbage)
                    continue;

                int matchLength = 1;
                int lastColourMatch = row;
                TileType current = _board[row, col];

                for (int nextRow = row + 1; nextRow < Const.Rows; nextRow++)
                {
                    if (_board[nextRow, col] == current || _board[nextRow, col] == TileType.Bonus)
                    {
                        matchLength++;
                        if (_board[nextRow, col] == current)
                            lastColourMatch = nextRow;
                    }
                    else
                        break;
                }

                if (_board[row + matchLength - 1, col] == TileType.Bonus)
                    matchLength--;

                if (matchLength >= 3 && lastColourMatch > row + 1)
                {
                    for (int i = 0; i < matchLength; i++)
                    {
                        if (row + i <= lastColourMatch)
                            clearedTiles.Add(new Vector2(col, row + i));
                    }
                }
            }
        }

        // clear tiles and adjacent garbage
        foreach (Vector2 tile in clearedTiles)
        {
            LastClear = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            _board[(int)tile.Y, (int)tile.X] = TileType.Empty;

            // if tile below is garbage
            if ((int)tile.Y - 1 >= 0 && _board[(int)tile.Y - 1, (int)tile.X] == TileType.Garbage)
                _board[(int)tile.Y - 1, (int)tile.X] = TileType.Empty;

            // if tile above is garbage
            if ((int)tile.Y + 1 < Const.Rows && _board[(int)tile.Y + 1, (int)tile.X] == TileType.Garbage)
                _board[(int)tile.Y + 1, (int)tile.X] = TileType.Empty;

            // if tile to the left is garbage
            if ((int)tile.X - 1 >= 0 && _board[(int)tile.Y, (int)tile.X - 1] == TileType.Garbage)
                _board[(int)tile.Y, (int)tile.X - 1] = TileType.Empty;

            // if tile to the right is garbage
            if ((int)tile.X + 1 < Const.Cols && _board[(int)tile.Y, (int)tile.X + 1] == TileType.Garbage)
                _board[(int)tile.Y, (int)tile.X + 1] = TileType.Empty;
        }
    }

    public void ProcessGravity()
    {
        for (int col = 0; col < Const.Cols; col++)
        {
            int bottomEmptyRow = Const.Rows - 1;

            for (int row = Const.Rows - 1; row >= 0; row--)
            {
                if (_board[row, col] != TileType.Empty)
                    _board[bottomEmptyRow--, col] = _board[row, col];
            }

            for (int row = bottomEmptyRow; row >= 0; row--)
            {
                _board[row, col] = TileType.Empty;
            }
        }
    }
}
