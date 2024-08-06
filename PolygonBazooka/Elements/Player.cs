using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonBazooka.Util;
using Vector2 = System.Numerics.Vector2;

namespace PolygonBazooka.Elements;

public class Player : DrawableGameComponent
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

    public Vector2 RenderPosition;

    private readonly SpriteBatch _spriteBatch;

    private readonly Texture2D _boardTexture;

    private readonly Texture2D _blueTile;
    private readonly Texture2D _greenTile;
    private readonly Texture2D _redTile;
    private readonly Texture2D _yellowTile;
    private readonly Texture2D _bonusTile;
    private readonly Texture2D _garbageTile;

    private readonly Texture2D _blueShadow;
    private readonly Texture2D _greenShadow;
    private readonly Texture2D _redShadow;
    private readonly Texture2D _yellowShadow;
    private readonly Texture2D _bonusShadow;


    public Player(Game game, bool localPlayer) : base(game)
    {
        _board = new TileType[Const.Rows, Const.Cols];
        _oldBoard = new TileType[Const.Rows, Const.Cols];

        _fallingBlockOrigin = TileType.Empty;
        _fallingBlockOrbit = TileType.Empty;

        if (localPlayer)
        {
            NextFallingBlock();
            NextFallingBlock();
            NextFallingBlock();
        }

        _board[0, 0] = TileType.Blue;
        _board[8, 2] = TileType.Yellow;
        _board[1, 1] = TileType.Green;

        _spriteBatch = new(GraphicsDevice);

        _boardTexture = Game.Content.Load<Texture2D>("Textures/board");
        _blueTile = Game.Content.Load<Texture2D>("Textures/blue");
        _greenTile = Game.Content.Load<Texture2D>("Textures/green");
        _redTile = Game.Content.Load<Texture2D>("Textures/red");
        _yellowTile = Game.Content.Load<Texture2D>("Textures/yellow");
        _bonusTile = Game.Content.Load<Texture2D>("Textures/bonus");
        _garbageTile = Game.Content.Load<Texture2D>("Textures/garbage");

        _blueShadow = Game.Content.Load<Texture2D>("Textures/blue_shadow");
        _greenShadow = Game.Content.Load<Texture2D>("Textures/green_shadow");
        _redShadow = Game.Content.Load<Texture2D>("Textures/red_shadow");
        _yellowShadow = Game.Content.Load<Texture2D>("Textures/yellow_shadow");
        _bonusShadow = Game.Content.Load<Texture2D>("Textures/bonus_shadow");
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // TODO: make the sprites scale based on the window size
        _spriteBatch.Draw(_boardTexture, new Rectangle((int)RenderPosition.X, (int)RenderPosition.Y,
            (int)(_boardTexture.Width * Const.Scale), (int)(_boardTexture.Height * Const.Scale)), Color.White);

        for (int row = 0; row < Const.Rows; row++)
        {
            for (int col = 0; col < Const.Cols; col++)
            {
                if (_board[row, col] == TileType.Empty)
                    continue;

                int x = (int)(RenderPosition.X + col * (16 * Const.Scale) + 4 * Const.Scale);
                int y = (int)(RenderPosition.Y + row * (16 * Const.Scale) + 4 * Const.Scale);
                RenderTile(_board[row, col], x, y);
            }
        }

        if (_fallingBlockOrigin != TileType.Empty)
        {
            int shadowOffset = 0;

            if (_yOrigin < _yOrbit)
                shadowOffset = -1;

            int shadowX = (int)(RenderPosition.X + _xOrigin * (16 * Const.Scale) + 4 * Const.Scale);
            int shadowY = (int)(RenderPosition.Y + (GetLowestEmptyCell(_xOrigin) + shadowOffset) * (16 * Const.Scale)
                                                 + 4 * Const.Scale);
            RenderTile(_fallingBlockOrigin, shadowX, shadowY, true);

            int tileX = (int)(RenderPosition.X + _xOrigin * (16 * Const.Scale) + 4 * Const.Scale);
            int tileY = (int)(RenderPosition.Y + _yOrigin * (16 * Const.Scale) + 4 * Const.Scale);
            RenderTile(_fallingBlockOrigin, tileX, tileY);
        }

        if (_fallingBlockOrbit != TileType.Empty)
        {
            int shadowOffset = 0;

            if (_yOrigin > _yOrbit)
                shadowOffset = -1;

            int shadowX = (int)(RenderPosition.X + _xOrbit * (16 * Const.Scale) + 4 * Const.Scale);
            int shadowY = (int)(RenderPosition.Y + (GetLowestEmptyCell(_xOrbit) + shadowOffset) * (16 * Const.Scale)
                                                 + 4 * Const.Scale);
            RenderTile(_fallingBlockOrbit, shadowX, shadowY, true);

            int tileX = (int)(RenderPosition.X + _xOrbit * (16 * Const.Scale) + 4 * Const.Scale);
            int tileY = (int)(RenderPosition.Y + _yOrbit * (16 * Const.Scale) + 4 * Const.Scale);
            RenderTile(_fallingBlockOrbit, tileX, tileY);
        }

        if (_nextBlockOrigin != TileType.Empty)
        {
            int x = (int)(RenderPosition.X + 7 * (16 * Const.Scale) + 8 * Const.Scale);
            int y = (int)(RenderPosition.Y + 0 * (16 * Const.Scale) + 14 * Const.Scale);
            RenderTile(_nextBlockOrigin, x, y);
        }

        if (_nextBlockOrbit != TileType.Empty)
        {
            int x = (int)(RenderPosition.X + 7 * (16 * Const.Scale) + 8 * Const.Scale);
            int y = (int)(RenderPosition.Y + 1 * (16 * Const.Scale) + 14 * Const.Scale);
            RenderTile(_nextBlockOrbit, x, y);
        }

        if (_nextNextBlockOrigin != TileType.Empty)
        {
            int x = (int)(RenderPosition.X + 7 * (16 * Const.Scale) + 8 * Const.Scale);
            int y = (int)(RenderPosition.Y + 2 * (16 * Const.Scale) + 18 * Const.Scale);
            RenderTile(_nextNextBlockOrigin, x, y);
        }

        if (_nextNextBlockOrbit != TileType.Empty)
        {
            int x = (int)(RenderPosition.X + 7 * (16 * Const.Scale) + 8 * Const.Scale);
            int y = (int)(RenderPosition.Y + 3 * (16 * Const.Scale) + 18 * Const.Scale);
            RenderTile(_nextNextBlockOrbit, x, y);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void RenderTile(TileType type, int x, int y, bool shadow = false)
    {
        Texture2D texture = shadow ? GetShadowTexture(type) : GetTileTexture(type);

        if (texture != null)
        {
            _spriteBatch.Draw(texture, new Rectangle(x, y,
                (int)(texture.Width * Const.Scale), (int)(texture.Height * Const.Scale)), Color.White);
        }
    }

    private Texture2D GetTileTexture(TileType type)
    {
        return type switch
        {
            TileType.Blue => _blueTile,
            TileType.Green => _greenTile,
            TileType.Red => _redTile,
            TileType.Yellow => _yellowTile,
            TileType.Bonus => _bonusTile,
            TileType.Garbage => _garbageTile,
            _ => null
        };
    }

    private Texture2D GetShadowTexture(TileType type)
    {
        return type switch
        {
            TileType.Blue => _blueShadow,
            TileType.Green => _greenShadow,
            TileType.Red => _redShadow,
            TileType.Yellow => _yellowShadow,
            TileType.Bonus => _bonusShadow,
            _ => null
        };
    }

    public void HardDrop()
    {
        // drop origin first if lower
        // they are flipped and i have no idea why but it works so..
        if (_yOrigin <= _yOrbit)
        {
            if (_fallingBlockOrbit != TileType.Empty)
                DropPiece(false, GetLowestEmptyCell(_xOrbit));
            if (_fallingBlockOrigin != TileType.Empty)
                DropPiece(true, GetLowestEmptyCell(_xOrigin));
        }
        else
        {
            if (_fallingBlockOrigin != TileType.Empty)
                DropPiece(true, GetLowestEmptyCell(_xOrigin));
            if (_fallingBlockOrbit != TileType.Empty)
                DropPiece(false, GetLowestEmptyCell(_xOrigin));
        }

        NextFallingBlock();
    }

    private void NextFallingBlock()
    {
        _fallingBlockOrigin = _nextBlockOrigin;
        _fallingBlockOrbit = _nextBlockOrbit;

        _xOrigin = 3;
        _yOrigin = -2;
        _xOrbit = 3;
        _yOrbit = -3;

        _nextBlockOrigin = _nextNextBlockOrigin;
        _nextBlockOrbit = _nextNextBlockOrbit;

        _nextNextBlockOrigin = Const.QueueTileTypes[RNG.Next(0, Const.QueueTileTypes.Length)];
        _nextNextBlockOrbit = Const.QueueTileTypes[RNG.Next(0, Const.QueueTileTypes.Length)];
    }

    private int GetLowestEmptyCell(int col)
    {
        for (int row = Const.Rows - 1; row >= 0; row--)
        {
            if (_board[row, col] == TileType.Empty)
                return row;
        }

        return -1;
    }

    private void DropPiece(bool origin, int destY)
    {
        // if (destY < 0)
        // {
        //     Failed = true;
        // }
        if (origin)
        {
            _board[destY, _xOrigin] = _fallingBlockOrigin;
            _fallingBlockOrigin = TileType.Empty;
        }
        else
        {
            _board[destY, _xOrbit] = _fallingBlockOrbit;
            _fallingBlockOrbit = TileType.Empty;
        }
    }

    public void MoveLeft()
    {
        // if (IsClearing())
        //     return;

        if (_xOrigin - 1 < 0 || _xOrbit - 1 < 0)
            return;

        if (IsCellToLeftNotEmpty())
            return;

        _xOrigin -= 1;
        _xOrbit -= 1;
    }

    public void MoveLeftFully()
    {
        while (_xOrigin - 1 >= 0 && _xOrbit - 1 >= 0 && !IsCellToLeftNotEmpty() /* && !IsClearing()*/)
        {
            _xOrigin -= 1;
            _xOrbit -= 1;
        }
    }

    public void MoveRight()
    {
        // if (IsClearing())
        //     return;

        if (_xOrigin + 1 > Const.Cols - 1 || _xOrbit + 1 > Const.Cols - 1)
            return;

        if (IsCellToRightNotEmpty())
            return;

        _xOrigin += 1;
        _xOrbit += 1;
    }

    public void MoveRightFully()
    {
        while (_xOrigin + 1 <= Const.Cols - 1 && _xOrbit + 1 <= Const.Cols - 1 &&
               !IsCellToRightNotEmpty() /* && !IsClearing()*/)
        {
            _xOrigin += 1;
            _xOrbit += 1;
        }
    }

    public void MoveDown()
    {
        int originLowestEmptyCell = GetLowestEmptyCell(_xOrigin);
        int orbitLowestEmptyCell = GetLowestEmptyCell(_xOrbit);

        if (_yOrigin < _yOrbit && !IsSideways()) originLowestEmptyCell--;
        if (_yOrbit < _yOrigin && !IsSideways()) orbitLowestEmptyCell--;

        if (_yOrigin < originLowestEmptyCell)
            _yOrigin += 1;
        if (_yOrbit < orbitLowestEmptyCell)
            _yOrbit += 1;
    }

    private bool IsCellToLeftNotEmpty()
    {
        if (_xOrigin > 0 && _xOrbit > 0)
        {
            if (_yOrigin < 0 && _yOrbit < 0)
                return false;

            if (_yOrigin >= 0)
            {
                if (_board[_yOrigin, _xOrigin - 1] != TileType.Empty)
                    return true;
            }

            if (_yOrbit >= 0)
            {
                if (_board[_yOrbit, _xOrbit - 1] != TileType.Empty)
                    return true;
            }

            return false;
        }

        return true;
    }

    private bool IsCellToRightNotEmpty()
    {
        if (_xOrigin < Const.Cols - 1 && _xOrbit < Const.Cols - 1)
        {
            if (_yOrigin < 0 && _yOrbit < 0)
                return false;

            if (_yOrigin >= 0)
            {
                if (_board[_yOrigin, _xOrigin + 1] != TileType.Empty)
                    return true;
            }

            if (_yOrbit >= 0)
            {
                if (_board[_yOrbit, _xOrbit + 1] != TileType.Empty)
                    return true;
            }

            return false;
        }

        return true;
    }

    private void Rotate(int dir)
    {
        if (IsCellToLeftNotEmpty() && IsCellToRightNotEmpty() && !IsSideways())
            return;

        // origin above orbit
        if (_yOrigin > _yOrbit)
        {
            if (IsCellToLeftNotEmpty() && dir == Const.CcwRotation)
                MoveRight();
            if (IsCellToRightNotEmpty() && dir == Const.CwRotation)
                MoveLeft();
            _xOrbit = _xOrigin - dir;
            _yOrbit = _yOrigin;
        }
        // origin below orbit
        else if (_yOrigin < _yOrbit)
        {
            if (IsCellToLeftNotEmpty() && dir == Const.CwRotation)
                MoveRight();
            if (IsCellToRightNotEmpty() && dir == Const.CcwRotation)
                MoveLeft();
            _xOrbit = _xOrigin + dir;
            _yOrbit = _yOrigin;
        }
        // origin left of orbit
        else if (_xOrigin < _xOrbit)
        {
            _xOrbit = _xOrigin;
            _yOrbit = _yOrigin - dir;
        }
        // origin right of orbit
        else if (_xOrigin > _xOrbit)
        {
            _xOrbit = _xOrigin;
            _yOrbit = _yOrigin + dir;
        }
    }

    public void RotateCw()
    {
        Rotate(Const.CwRotation);
    }

    public void RotateCcw()
    {
        Rotate(Const.CcwRotation);
    }

    // TODO: Flip currently does not kick when sideways adjacent to the wall when y < 0
    public void Flip()
    {
        if (IsSideways())
        {
            // orbit left of origin (to move to right)
            if (_xOrigin > _xOrbit)
            {
                if (IsCellToLeftNotEmpty() && IsCellToRightNotEmpty())
                    (_xOrigin, _xOrbit) = (_xOrbit, _xOrigin);
                else if (IsCellToRightNotEmpty())
                    MoveLeft();
                _xOrbit = _xOrigin + 1;
            }
            // orbit right of origin (to move to left)
            else if (_xOrigin < _xOrbit)
            {
                if (IsCellToLeftNotEmpty() && IsCellToRightNotEmpty())
                    (_xOrigin, _xOrbit) = (_xOrbit, _xOrigin);
                else if (IsCellToLeftNotEmpty())
                    MoveRight();
                _xOrbit = _xOrigin - 1;
            }
        }
        else
        {
            // for some reason these are flipped and i have no idea why but it works
            // orbit above origin
            if (_yOrbit > _yOrigin)
            {
                _yOrbit = _yOrigin - 1;
            }
            // orbit below origin
            else if (_yOrbit < _yOrigin)
            {
                // ReSharper disable once ReplaceWithSingleAssignment.False
                bool kick = false;

                // ReSharper disable once ConvertIfToOrExpression
                if (_yOrigin >= Const.Rows - 1)
                    kick = true;

                if (_yOrigin >= 0 && !kick)
                {
                    if (_board[_yOrigin + 1, _xOrigin] != TileType.Empty)
                        kick = true;
                }

                if (kick)
                {
                    _yOrigin--;
                    _yOrbit--;
                }

                _yOrbit = _yOrigin + 1;
            }
        }
    }

    private bool IsSideways()
    {
        return _yOrigin == _yOrbit;
    }

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