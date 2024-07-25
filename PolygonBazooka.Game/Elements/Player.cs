using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace PolygonBazooka.Game.Elements;

public partial class Player : CompositeDrawable
{
    private readonly TileType[,] board;
    private TileType[,] oldBoard;

    private readonly Sprite[,] renderedTiles;
    private readonly Sprite[] renderedFallingTiles;
    private readonly Sprite[] renderedShadowTiles;
    private readonly Sprite[] renderedQueueTile;

    private const int cw = -1;
    private const int ccw = 1;

    // origin tile of the falling block
    private TileType fallingBlockOrigin { get; set; }

    // tile that orbits around the origin when rotated
    private TileType fallingBlockOrbit { get; set; }

    // position of the tile in board coordinates
    private int xOrigin;
    private int yOrigin;

    private int xOrbit;
    private int yOrbit;

    // blocks in queue
    private TileType nextBlockOrigin { get; set; } = TileType.Blue;
    private TileType nextBlockOrbit { get; set; } = TileType.Bonus;

    private TileType nextNextBlockOrigin { get; set; } = TileType.Green;
    private TileType nextNextBlockOrbit { get; set; } = TileType.Red;

    // control handling things in ms
    public float DelayedAutoShift = 200;
    public float AutoRepeatRate = 0;

    private bool leftPressed;
    private long leftPressStart;
    private bool rightPressed;
    private long rightPressStart;
    private bool downPressed;

    private bool leftDasActive;
    private long lastLeftAutoRepeat;
    private bool rightDasActive;
    private long lastRightAutoRepeat;

    public Player()
    {
        board = new TileType[Const.ROWS, Const.COLS];
        oldBoard = board.Clone() as TileType[,];
        renderedTiles = new Sprite[Const.ROWS, Const.COLS];
        renderedFallingTiles = new Sprite[2];
        renderedShadowTiles = new Sprite[2];
        renderedQueueTile = new Sprite[4];
        resetBoard();
        SetFallingBlock(TileType.Yellow, TileType.Green);
    }

    [Resolved]
    private TextureStore textures { get; set; }

    private TextureAnimation boardAnimation;
    private TextureAnimation blueTileAnimation;
    private TextureAnimation blueShadowAnimation;
    private TextureAnimation greenTileAnimation;
    private TextureAnimation greenShadowAnimation;
    private TextureAnimation redTileAnimation;
    private TextureAnimation redShadowAnimation;
    private TextureAnimation yellowTileAnimation;
    private TextureAnimation yellowShadowAnimation;
    private TextureAnimation bonusTileAnimation;
    private TextureAnimation bonusShadowAnimation;
    private TextureAnimation garbageTileAnimation;

    public void SetFallingBlock(TileType origin, TileType orbit)
    {
        fallingBlockOrigin = origin;
        fallingBlockOrbit = orbit;
        xOrigin = 3;
        yOrigin = -2;
        xOrbit = 3;
        yOrbit = -3;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        // initialise textures
        Scale = Const.SCALE_ADJUST;
        boardAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre, };
        boardAnimation.AddFrame(textures.Get("board"));

        blueTileAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre, };
        blueTileAnimation.AddFrame(textures.Get("blue"));

        blueShadowAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre };
        blueShadowAnimation.AddFrame(textures.Get("blue_shadow"));

        greenTileAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre, };
        greenTileAnimation.AddFrame(textures.Get("green"));

        greenShadowAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre };
        greenShadowAnimation.AddFrame(textures.Get("green_shadow"));

        redTileAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre, };
        redTileAnimation.AddFrame(textures.Get("red"));

        redShadowAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre };
        redShadowAnimation.AddFrame(textures.Get("red_shadow"));

        yellowTileAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre, };
        yellowTileAnimation.AddFrame(textures.Get("yellow"));

        yellowShadowAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre };
        yellowShadowAnimation.AddFrame(textures.Get("yellow_shadow"));

        bonusTileAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre, };
        bonusTileAnimation.AddFrame(textures.Get("bonus"));

        bonusShadowAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre };
        bonusShadowAnimation.AddFrame(textures.Get("bonus_shadow"));

        garbageTileAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre };
        garbageTileAnimation.AddFrame(textures.Get("garbage"));

        // render the board at the bottom layer
        AddInternal(boardAnimation);
    }

    protected override void Update()
    {
        base.Update();

        // TODO: implement instant ARR (0 ms)

        // left DAS
        if (!leftPressed)
            leftDasActive = false;

        if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - leftPressStart >= DelayedAutoShift && leftPressed && !leftDasActive)
            leftDasActive = true;

        if (leftDasActive && leftPressed && DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastLeftAutoRepeat >= AutoRepeatRate)
        {
            if (AutoRepeatRate == 0)
                moveLeftFully();
            else
                moveLeft();
            lastLeftAutoRepeat = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        // right DAS
        if (!rightPressed)
            rightDasActive = false;

        if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - rightPressStart >= DelayedAutoShift && rightPressed && !rightDasActive)
            rightDasActive = true;

        if (rightDasActive && rightPressed && DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastRightAutoRepeat >= AutoRepeatRate)
        {
            if (AutoRepeatRate == 0)
                moveRightFully();
            else
                moveRight();
            lastRightAutoRepeat = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        clear();
        processGravity();

        renderTiles();

        oldBoard = board.Clone() as TileType[,];
    }

    // TODO: this currently does not consider bonus or garbage. it should be implemented
    private void clear()
    {
        List<Vector2> clearedTiles = new List<Vector2>();

        // clear horizontally
        for (int row = 0; row < Const.ROWS; row++)
        {
            for (int col = 0; col < Const.COLS; col++)
            {
                if (board[row, col] == TileType.Empty)
                    continue;

                if (col + 2 < Const.COLS)
                {
                    if (board[row, col] == board[row, col + 1] && board[row, col] == board[row, col + 2])
                    {
                        clearedTiles.Add(new Vector2(col, row));
                        clearedTiles.Add(new Vector2(col + 1, row));
                        clearedTiles.Add(new Vector2(col + 2, row));
                    }
                }
            }
        }

        // clear vertically
        for (int col = 0; col < Const.COLS; col++)
        {
            for (int row = 0; row < Const.ROWS; row++)
            {
                if (board[row, col] == TileType.Empty)
                    continue;

                if (row + 2 < Const.ROWS)
                {
                    if (board[row, col] == board[row + 1, col] && board[row, col] == board[row + 2, col])
                    {
                        clearedTiles.Add(new Vector2(col, row));
                        clearedTiles.Add(new Vector2(col, row + 1));
                        clearedTiles.Add(new Vector2(col, row + 2));
                    }
                }
            }
        }

        // clear tiles
        foreach (Vector2 tile in clearedTiles)
        {
            board[(int)tile.Y, (int)tile.X] = TileType.Empty;
        }
    }

    private Sprite createTileSprite(int row, int col, TileType type, bool shadow = false, bool queue = false, int offset = 0)
    {
        return new Sprite
        {
            Texture = shadow ? getShadowAnimation(type).CurrentFrame : getTileAnimation(type).CurrentFrame,
            Position = queue ? new Vector2(4 + 8 * 7, offset) : new Vector2(2 + col * 8, 2 + row * 8)
        };
    }

    private void renderTiles()
    {
        // render board tiles
        for (int row = 0; row < Const.ROWS; row++)
        {
            for (int col = 0; col < Const.COLS; col++)
            {
                if (board[row, col] == oldBoard[row, col])
                    continue;

                if (board[row, col] == TileType.Empty)
                {
                    if (renderedTiles[row, col] != null)
                    {
                        RemoveInternal(renderedTiles[row, col], false);
                        renderedTiles[row, col].Dispose();
                        renderedTiles[row, col] = null;
                    }

                    continue;
                }

                if (renderedTiles[row, col] != null)
                    renderedTiles[row, col].Dispose();

                Sprite sprite = createTileSprite(row, col, board[row, col]);
                renderedTiles[row, col] = sprite;
                AddInternal(sprite);
            }
        }

        // render falling block tiles and shadow
        if (fallingBlockOrigin != TileType.Empty && fallingBlockOrbit != TileType.Empty)
        {
            // tiles
            Sprite origin = createTileSprite(yOrigin, xOrigin, fallingBlockOrigin);
            Sprite oldOrigin = renderedFallingTiles[0];
            if (oldOrigin != null)
                RemoveInternal(oldOrigin, false);
            renderedFallingTiles[0] = origin;
            AddInternal(origin);

            Sprite orbit = createTileSprite(yOrbit, xOrbit, fallingBlockOrbit);
            Sprite oldOrbit = renderedFallingTiles[1];
            if (oldOrbit != null)
                RemoveInternal(oldOrbit, false);
            renderedFallingTiles[1] = orbit;
            AddInternal(orbit);

            // shadows
            int originShadowYOffset = 0;
            int orbitShadowYOffset = 0;

            if (yOrigin < yOrbit)
                originShadowYOffset = -1;
            else if (yOrigin > yOrbit)
                orbitShadowYOffset = -1;

            Sprite originShadow = createTileSprite(getLowestEmptyCell(xOrigin) + originShadowYOffset, xOrigin, fallingBlockOrigin, true);
            Sprite oldOriginShadow = renderedShadowTiles[0];
            if (oldOriginShadow != null)
                RemoveInternal(oldOriginShadow, false);
            renderedShadowTiles[0] = originShadow;
            AddInternal(originShadow);

            Sprite orbitShadow = createTileSprite(getLowestEmptyCell(xOrbit) + orbitShadowYOffset, xOrbit, fallingBlockOrbit, true);
            Sprite oldOrbitShadow = renderedShadowTiles[1];
            if (oldOrbitShadow != null)
                RemoveInternal(oldOrbitShadow, false);
            renderedShadowTiles[1] = orbitShadow;
            AddInternal(orbitShadow);
        }

        // render next in queue
        if (nextBlockOrigin != TileType.Empty && nextBlockOrbit != TileType.Empty)
        {
            renderQueueTile(nextBlockOrigin, 7, 0);
            renderQueueTile(nextBlockOrbit, 15, 1);
        }

        if (nextNextBlockOrigin != TileType.Empty && nextNextBlockOrbit != TileType.Empty)
        {
            renderQueueTile(nextNextBlockOrigin, 25, 2);
            renderQueueTile(nextNextBlockOrbit, 33, 3);
        }
    }

    private void renderQueueTile(TileType type, int offset, int renderedIndex)
    {
        Sprite sprite = createTileSprite(0, 0, type, false, true, offset);
        Sprite oldSprite = renderedQueueTile[renderedIndex];
        if (oldSprite != null)
            RemoveInternal(oldSprite, false);
        renderedQueueTile[renderedIndex] = sprite;
        AddInternal(sprite);
    }

    private TextureAnimation getTileAnimation(TileType type)
    {
        return type switch
        {
            TileType.Blue => blueTileAnimation,
            TileType.Green => greenTileAnimation,
            TileType.Red => redTileAnimation,
            TileType.Yellow => yellowTileAnimation,
            TileType.Bonus => bonusTileAnimation,
            TileType.Garbage => garbageTileAnimation,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }

    private TextureAnimation getShadowAnimation(TileType type)
    {
        return type switch
        {
            TileType.Blue => blueShadowAnimation,
            TileType.Green => greenShadowAnimation,
            TileType.Red => redShadowAnimation,
            TileType.Yellow => yellowShadowAnimation,
            TileType.Bonus => bonusShadowAnimation,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }

    private void resetBoard()
    {
        for (int row = 0; row < 12; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                board[row, col] = TileType.Empty;
            }
        }
    }

    // TODO: this still doesnt work
    private void processGravity()
    {
        for (int col = 0; col < Const.COLS; col++)
        {
            // skip the bottom row, it cant fall
            for (int row = Const.ROWS - 2; row >= 0; row--)
            {
                if (board[row, col] == TileType.Empty)
                    continue;

                if (row + 1 >= Const.ROWS)
                    continue;

                int lowestEmptyCell = row;
                while (board[lowestEmptyCell + 1, col] == TileType.Empty)
                    lowestEmptyCell++;

                board[lowestEmptyCell, col] = board[row, col];
                board[row, col] = TileType.Empty;
            }
        }

        // skip the lowest row, it cant fall
        // for (int row = Const.ROWS - 2; row >= 0; row--)
        // {
        //     for (int col = 0; col < Const.COLS; col++)
        //     {
        //         if (board[row, col] == TileType.Empty)
        //             continue;
        //
        //         int rowToMoveTo = row;
        //
        //         for (int i = row; i < Const.ROWS; i++)
        //         {
        //             if (board[i, col] == TileType.Empty)
        //                 rowToMoveTo = i;
        //             else
        //                 break;
        //         }
        //
        //         if (rowToMoveTo != row)
        //         {
        //             board[rowToMoveTo, col] = board[row, col];
        //             board[row, col] = TileType.Empty;
        //         }
        //     }
        // }
    }

    public void HardDrop()
    {
        // drop origin first if lower
        // they are flipped and i have no idea why but it works so..
        if (yOrigin <= yOrbit)
        {
            board[getLowestEmptyCell(xOrbit), xOrbit] = fallingBlockOrbit;
            board[getLowestEmptyCell(xOrigin), xOrigin] = fallingBlockOrigin;
        }
        else
        {
            board[getLowestEmptyCell(xOrigin), xOrigin] = fallingBlockOrigin;
            board[getLowestEmptyCell(xOrbit), xOrbit] = fallingBlockOrbit;
        }
    }

    private int getLowestEmptyCell(int col)
    {
        for (int row = Const.ROWS - 1; row >= 0; row--)
        {
            if (board[row, col] == TileType.Empty)
                return row;
        }

        return -1;
    }

    public void MoveLeftInputDown()
    {
        rightPressed = false;

        if (!leftPressed)
        {
            leftPressStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            moveLeft();
        }

        leftPressed = true;
    }

    public void MoveRightInputDown()
    {
        leftPressed = false;

        if (!rightPressed)
        {
            rightPressStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            moveRight();
        }

        rightPressed = true;
    }

    public void MoveLeftInputUp()
    {
        leftPressed = false;
    }

    public void MoveRightInputUp()
    {
        rightPressed = false;
    }

    private void moveLeft()
    {
        if (xOrigin - 1 < 0 || xOrbit - 1 < 0)
            return;

        xOrigin -= 1;
        xOrbit -= 1;
    }

    private void moveLeftFully()
    {
        while (xOrigin - 1 >= 0 && xOrbit - 1 >= 0)
        {
            xOrigin -= 1;
            xOrbit -= 1;
        }
    }

    private void moveRight()
    {
        if (xOrigin + 1 > Const.COLS - 1 || xOrbit + 1 > Const.COLS - 1)
            return;

        xOrigin += 1;
        xOrbit += 1;
    }

    private void moveRightFully()
    {
        while (xOrigin + 1 <= Const.COLS - 1 && xOrbit + 1 <= Const.COLS - 1)
        {
            xOrigin += 1;
            xOrbit += 1;
        }
    }

    private bool isCellToLeftNotEmpty()
    {
        int lowestEmptyCell = getLowestEmptyCell(xOrigin);
        return xOrigin <= 0 || lowestEmptyCell == -1 || board[lowestEmptyCell, xOrigin - 1] != TileType.Empty;
    }

    private bool isCellToRightNotEmpty()
    {
        int lowestEmptyCell = getLowestEmptyCell(xOrigin);
        return xOrigin >= Const.COLS - 1 || lowestEmptyCell == -1 || board[lowestEmptyCell, xOrigin + 1] != TileType.Empty;
    }

    private void rotate(int dir)
    {
        // origin above orbit
        if (yOrigin > yOrbit)
        {
            if (isCellToLeftNotEmpty() && dir == ccw)
                moveRight();
            if (isCellToRightNotEmpty() && dir == cw)
                moveLeft();
            xOrbit = xOrigin - dir;
            yOrbit = yOrigin;
        }
        // origin below orbit
        else if (yOrigin < yOrbit)
        {
            if (isCellToLeftNotEmpty() && dir == cw)
                moveRight();
            if (isCellToRightNotEmpty() && dir == ccw)
                moveLeft();
            xOrbit = xOrigin + dir;
            yOrbit = yOrigin;
        }
        // origin left of orbit
        else if (xOrigin < xOrbit)
        {
            xOrbit = xOrigin;
            yOrbit = yOrigin - dir;
        }
        // origin right of orbit
        else if (xOrigin > xOrbit)
        {
            xOrbit = xOrigin;
            yOrbit = yOrigin + dir;
        }
    }

    public void RotateCw()
    {
        rotate(cw);
    }

    public void RotateCcw()
    {
        rotate(ccw);
    }

    // TODO: this should just rotate 180 degrees
    public void Flip()
    {
        if (isSideways()) (xOrigin, xOrbit) = (xOrbit, xOrigin);
        else (yOrigin, yOrbit) = (yOrbit, yOrigin);
    }

    private bool isSideways()
    {
        return xOrigin == xOrbit;
    }
}
