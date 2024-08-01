using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Utils;
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

    private readonly List<TextureAnimation> renderedClearAnimations;
    public long LastClear { get; private set; }

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
    public float DelayedAutoShift = 127;
    public float AutoRepeatRate = 0;

    public float SoftDropRate = 100;

    private bool leftPressed;
    private long leftPressStart;
    private bool rightPressed;

    private long rightPressStart;

    // soft dropping is kinda pointless in this kind of game
    private bool downPressed;

    private bool leftDasActive;
    private long lastLeftAutoRepeat;
    private bool rightDasActive;
    private long lastRightAutoRepeat;
    private long lastDownAutoRepeat;

    // falling block gravity
    public float GravityRate = 1000;
    private long lastFallingGravityMovement;

    public Player()
    {
        board = new TileType[Const.ROWS, Const.COLS];
        oldBoard = board.Clone() as TileType[,];
        renderedTiles = new Sprite[Const.ROWS, Const.COLS];
        renderedFallingTiles = new Sprite[2];
        renderedShadowTiles = new Sprite[2];
        renderedQueueTile = new Sprite[4];

        renderedClearAnimations = new();

        resetBoard();
        SetFallingBlock(TileType.Yellow, TileType.Green);
    }

    [Resolved]
    private TextureStore textures { get; set; }

    private Texture clearSpriteSheet;

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

        clearSpriteSheet = textures.Get("clear_sprite_sheet");

        // render the board at the bottom layer
        AddInternal(boardAnimation);
    }

    protected override void Update()
    {
        base.Update();

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

        if (!IsClearing())
        {
            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastFallingGravityMovement >= GravityRate)
            {
                lastFallingGravityMovement = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                int originLowestEmptyCell = getLowestEmptyCell(xOrigin);
                int orbitLowestEmptyCell = getLowestEmptyCell(xOrbit);

                if (yOrigin < yOrbit && !isSideways()) originLowestEmptyCell--;
                if (yOrbit < yOrigin && !isSideways()) orbitLowestEmptyCell--;

                if (yOrigin < originLowestEmptyCell && fallingBlockOrigin != TileType.Empty)
                    yOrigin += 1;
                else if (fallingBlockOrigin != TileType.Empty)
                {
                    board[yOrigin, xOrigin] = fallingBlockOrigin;
                    fallingBlockOrigin = TileType.Empty;
                }

                if (yOrbit < orbitLowestEmptyCell && fallingBlockOrbit != TileType.Empty)
                    yOrbit += 1;
                else if (fallingBlockOrbit != TileType.Empty)
                {
                    board[yOrbit, xOrbit] = fallingBlockOrbit;
                    fallingBlockOrbit = TileType.Empty;
                }

                if (fallingBlockOrigin == TileType.Empty && fallingBlockOrbit == TileType.Empty)
                    nextFallingBlock();
            }

            // useless
            if (downPressed)
            {
                if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastDownAutoRepeat >= SoftDropRate)
                {
                    lastDownAutoRepeat = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                    int originLowestEmptyCell = getLowestEmptyCell(xOrigin);
                    int orbitLowestEmptyCell = getLowestEmptyCell(xOrbit);

                    if (yOrigin < yOrbit && !isSideways()) originLowestEmptyCell--;
                    if (yOrbit < yOrigin && !isSideways()) orbitLowestEmptyCell--;

                    if (yOrigin < originLowestEmptyCell)
                        yOrigin += 1;
                    if (yOrbit < orbitLowestEmptyCell)
                        yOrbit += 1;
                }
            }

            processGravity();
        }

        clear();

        renderTiles();

        oldBoard = board.Clone() as TileType[,];
    }

    public bool IsClearing()
    {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds() - LastClear < Const.CLEAR_TIME;
    }

    private void clear()
    {
        List<Vector2> clearedTiles = new List<Vector2>();

        // clear horizontally
        for (int row = 0; row < Const.ROWS; row++)
        {
            for (int col = 0; col < Const.COLS; col++)
            {
                if (board[row, col] == TileType.Empty || board[row, col] == TileType.Bonus || board[row, col] == TileType.Garbage)
                    continue;

                int matchLength = 1;
                TileType current = board[row, col];

                for (int nextCol = col + 1; nextCol < Const.COLS; nextCol++)
                {
                    if (board[row, nextCol] == current || board[row, nextCol] == TileType.Bonus)
                        matchLength++;
                    else
                        break;
                }

                if (board[row, col + matchLength - 1] == TileType.Bonus)
                    matchLength--;

                if (matchLength >= 3 && board[row, col + matchLength - 1] == current)
                {
                    for (int i = 0; i < matchLength; i++)
                        clearedTiles.Add(new Vector2(col + i, row));
                }
            }
        }

        // clear vertically
        for (int col = 0; col < Const.COLS; col++)
        {
            for (int row = 0; row < Const.ROWS; row++)
            {
                if (board[row, col] == TileType.Empty || board[row, col] == TileType.Bonus || board[row, col] == TileType.Garbage)
                    continue;

                int matchLength = 1;
                TileType current = board[row, col];

                for (int nextRow = row + 1; nextRow < Const.ROWS; nextRow++)
                {
                    if (board[nextRow, col] == current || board[nextRow, col] == TileType.Bonus)
                        matchLength++;
                    else
                        break;
                }

                if (board[row + matchLength - 1, col] == TileType.Bonus)
                    matchLength--;

                if (matchLength >= 3 && board[row + matchLength - 1, col] == current)
                {
                    for (int i = 0; i < matchLength; i++)
                        clearedTiles.Add(new Vector2(col, row + i));
                }
            }
        }

        // clear tiles and adjacent garbage
        foreach (Vector2 tile in clearedTiles)
        {
            LastClear = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            board[(int)tile.Y, (int)tile.X] = TileType.Empty;
            renderClearAnimation(tile.X, tile.Y);

            // if tile below is garbage
            if ((int)tile.Y - 1 >= 0 && board[(int)tile.Y - 1, (int)tile.X] == TileType.Garbage)
            {
                board[(int)tile.Y - 1, (int)tile.X] = TileType.Empty;
                renderClearAnimation(tile.X, tile.Y);
            }

            // if tile above is garbage
            if ((int)tile.Y + 1 < Const.ROWS && board[(int)tile.Y + 1, (int)tile.X] == TileType.Garbage)
            {
                board[(int)tile.Y + 1, (int)tile.X] = TileType.Empty;
                renderClearAnimation(tile.X, tile.Y);
            }

            // if tile to the left is garbage
            if ((int)tile.X - 1 >= 0 && board[(int)tile.Y, (int)tile.X - 1] == TileType.Garbage)
            {
                board[(int)tile.Y, (int)tile.X - 1] = TileType.Empty;
                renderClearAnimation(tile.X, tile.Y);
            }

            // if tile to the right is garbage
            if ((int)tile.X + 1 < Const.COLS && board[(int)tile.Y, (int)tile.X + 1] == TileType.Garbage)
            {
                board[(int)tile.Y, (int)tile.X + 1] = TileType.Empty;
                renderClearAnimation(tile.X, tile.Y);
            }
        }
    }

    private void renderClearAnimation(float x, float y)
    {
        TextureAnimation clearAnimation = new TextureAnimation
        {
            Origin = Anchor.Centre,
            Anchor = Anchor.Centre,
            DefaultFrameLength = 50,
            Position = new Vector2(6 + x * 8, 6 + y * 8),
            Loop = false,
        };
        clearAnimation.AddFrame(clearSpriteSheet.Crop(new RectangleF(0, 0, 48, 48)));
        clearAnimation.AddFrame(clearSpriteSheet.Crop(new RectangleF(48, 0, 48, 48)));
        clearAnimation.AddFrame(clearSpriteSheet.Crop(new RectangleF(48 * 2, 0, 48, 48)));
        clearAnimation.AddFrame(clearSpriteSheet.Crop(new RectangleF(48 * 3, 0, 48, 48)));
        clearAnimation.AddFrame(clearSpriteSheet.Crop(new RectangleF(48 * 4, 0, 48, 48)));
        clearAnimation.AddFrame(clearSpriteSheet.Crop(new RectangleF(48 * 5, 0, 48, 48)));
        clearAnimation.AddFrame(clearSpriteSheet.Crop(new RectangleF(48 * 6, 0, 48, 48)));
        clearAnimation.AddFrame(clearSpriteSheet.Crop(new RectangleF(48 * 7, 0, 48, 48)));
        // add an extra frame for deletion of animation
        clearAnimation.AddFrame(clearSpriteSheet.Crop(new RectangleF(48 * 7, 0, 48, 48)));
        clearAnimation.Scale /= 2;

        renderedClearAnimations.Add(clearAnimation);
        AddInternal(clearAnimation);
    }

    private Sprite createTileSprite(int row, int col, TileType type, bool shadow = false, bool queue = false, int offset = 0)
    {
        return new Sprite
        {
            Texture = shadow ? getShadowAnimation(type).CurrentFrame : getTileAnimation(type).CurrentFrame,
            Position = queue ? new Vector2(4 + 8 * 7, offset) : new Vector2(2 + col * 8, 2 + row * 8)
        };
    }

    /// <summary>
    /// Both shadows are rendered first to have the falling blocks on top
    /// </summary>
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
                {
                    RemoveInternal(renderedTiles[row, col], false);
                    renderedTiles[row, col].Dispose();
                }

                Sprite sprite = createTileSprite(row, col, board[row, col]);
                renderedTiles[row, col] = sprite;
                AddInternal(sprite);
            }
        }

        // render falling block tiles and shadow
        bool renderFallingOrigin = fallingBlockOrigin != TileType.Empty;
        bool renderFallingOrbit = fallingBlockOrbit != TileType.Empty;

        // render origin shadow
        if (renderFallingOrigin)
        {
            int originShadowYOffset = 0;
            if (yOrigin < yOrbit)
                originShadowYOffset = -1;

            Sprite originShadow = createTileSprite(getLowestEmptyCell(xOrigin) + originShadowYOffset, xOrigin, fallingBlockOrigin, true);
            Sprite oldOriginShadow = renderedShadowTiles[0];
            if (oldOriginShadow != null)
                RemoveInternal(oldOriginShadow, false);
            renderedShadowTiles[0] = originShadow;
            AddInternal(originShadow);
        }

        // render orbit shadow
        if (renderFallingOrbit)
        {
            int orbitShadowYOffset = 0;
            if (yOrigin > yOrbit)
                orbitShadowYOffset = -1;

            Sprite orbitShadow = createTileSprite(getLowestEmptyCell(xOrbit) + orbitShadowYOffset, xOrbit, fallingBlockOrbit, true);
            Sprite oldOrbitShadow = renderedShadowTiles[1];
            if (oldOrbitShadow != null)
                RemoveInternal(oldOrbitShadow, false);
            renderedShadowTiles[1] = orbitShadow;
            AddInternal(orbitShadow);
        }

        // render origin
        if (renderFallingOrigin)
        {
            Sprite origin = createTileSprite(yOrigin, xOrigin, fallingBlockOrigin);
            Sprite oldOrigin = renderedFallingTiles[0];
            if (oldOrigin != null)
                RemoveInternal(oldOrigin, false);
            renderedFallingTiles[0] = origin;
            AddInternal(origin);
        }

        // render orbit
        if (renderFallingOrbit)
        {
            Sprite orbit = createTileSprite(yOrbit, xOrbit, fallingBlockOrbit);
            Sprite oldOrbit = renderedFallingTiles[1];
            if (oldOrbit != null)
                RemoveInternal(oldOrbit, false);
            renderedFallingTiles[1] = orbit;
            AddInternal(orbit);
        }

        if (!renderFallingOrigin)
        {
            Sprite shadow = renderedShadowTiles[0];
            if (shadow != null)
                RemoveInternal(shadow, true);
            renderedShadowTiles[0] = null;

            Sprite fallingTile = renderedFallingTiles[0];
            if (fallingTile != null)
                RemoveInternal(fallingTile, true);
            renderedFallingTiles[0] = null;
        }

        if (!renderFallingOrbit)
        {
            Sprite shadow = renderedShadowTiles[1];
            if (shadow != null)
                RemoveInternal(shadow, true);
            renderedShadowTiles[1] = null;

            Sprite sprite = renderedFallingTiles[1];
            if (sprite != null)
                RemoveInternal(sprite, true);
            renderedFallingTiles[1] = null;
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

        foreach (TextureAnimation animation in renderedClearAnimations)
        {
            if (animation.CurrentFrameIndex != 8)
                continue;

            RemoveInternal(animation, true);
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
            // TileType.Garbage => bonusShadowAnimation, // temp
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

    private void processGravity()
    {
        for (int col = 0; col < Const.COLS; col++)
        {
            int bottomEmptyRow = Const.ROWS - 1;

            for (int row = Const.ROWS - 1; row >= 0; row--)
            {
                if (board[row, col] != TileType.Empty)
                    board[bottomEmptyRow--, col] = board[row, col];
            }

            for (int row = bottomEmptyRow; row >= 0; row--)
            {
                board[row, col] = TileType.Empty;
            }
        }
    }

    public void HardDrop()
    {
        // drop origin first if lower
        // they are flipped and i have no idea why but it works so..
        if (yOrigin <= yOrbit)
        {
            if (fallingBlockOrbit != TileType.Empty)
                board[getLowestEmptyCell(xOrbit), xOrbit] = fallingBlockOrbit;
            if (fallingBlockOrigin != TileType.Empty)
                board[getLowestEmptyCell(xOrigin), xOrigin] = fallingBlockOrigin;
        }
        else
        {
            if (fallingBlockOrigin != TileType.Empty)
                board[getLowestEmptyCell(xOrigin), xOrigin] = fallingBlockOrigin;
            if (fallingBlockOrbit != TileType.Empty)
                board[getLowestEmptyCell(xOrbit), xOrbit] = fallingBlockOrbit;
        }

        nextFallingBlock();
    }

    private void nextFallingBlock()
    {
        fallingBlockOrigin = nextBlockOrigin;
        fallingBlockOrbit = nextBlockOrbit;

        xOrigin = 3;
        yOrigin = -2;
        xOrbit = 3;
        yOrbit = -3;

        nextBlockOrigin = nextNextBlockOrigin;
        nextBlockOrbit = nextNextBlockOrbit;

        nextNextBlockOrigin = Const.QUEUE_TILE_TYPES[RNG.Next(0, Const.QUEUE_TILE_TYPES.Length)];
        nextNextBlockOrbit = Const.QUEUE_TILE_TYPES[RNG.Next(0, Const.QUEUE_TILE_TYPES.Length)];
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

    public void SoftDrop(bool pressed)
    {
        downPressed = pressed;
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
        if (IsClearing())
            return;

        if (xOrigin - 1 < 0 || xOrbit - 1 < 0)
            return;

        if (isCellToLeftNotEmpty())
            return;

        xOrigin -= 1;
        xOrbit -= 1;
    }

    private void moveLeftFully()
    {
        while (xOrigin - 1 >= 0 && xOrbit - 1 >= 0 && !isCellToLeftNotEmpty() && !IsClearing())
        {
            xOrigin -= 1;
            xOrbit -= 1;
        }
    }

    private void moveRight()
    {
        if (IsClearing())
            return;

        if (xOrigin + 1 > Const.COLS - 1 || xOrbit + 1 > Const.COLS - 1)
            return;

        if (isCellToRightNotEmpty())
            return;

        xOrigin += 1;
        xOrbit += 1;
    }

    private void moveRightFully()
    {
        while (xOrigin + 1 <= Const.COLS - 1 && xOrbit + 1 <= Const.COLS - 1 && !isCellToRightNotEmpty() && !IsClearing())
        {
            xOrigin += 1;
            xOrbit += 1;
        }
    }

    private bool isCellToLeftNotEmpty()
    {
        if (xOrigin > 0 && xOrbit > 0)
        {
            if (yOrigin < 0 && yOrbit < 0)
                return false;

            if (yOrigin >= 0)
            {
                if (board[yOrigin, xOrigin - 1] != TileType.Empty)
                    return true;
            }

            if (yOrbit >= 0)
            {
                if (board[yOrbit, xOrbit - 1] != TileType.Empty)
                    return true;
            }

            return false;
        }

        return true;
    }

    private bool isCellToRightNotEmpty()
    {
        if (xOrigin < Const.COLS - 1 && xOrbit < Const.COLS - 1)
        {
            if (yOrigin < 0 && yOrbit < 0)
                return false;

            if (yOrigin >= 0)
            {
                if (board[yOrigin, xOrigin + 1] != TileType.Empty)
                    return true;
            }

            if (yOrbit >= 0)
            {
                if (board[yOrbit, xOrbit + 1] != TileType.Empty)
                    return true;
            }

            return false;
        }

        return true;
    }

    private void rotate(int dir)
    {
        if (isCellToLeftNotEmpty() && isCellToRightNotEmpty() && !isSideways())
            return;

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

    public void Flip()
    {
        if (isSideways())
        {
            // orbit left of origin (to move to right)
            if (xOrigin > xOrbit)
            {
                if (isCellToLeftNotEmpty() && isCellToRightNotEmpty())
                    (xOrigin, xOrbit) = (xOrbit, xOrigin);
                else if (isCellToRightNotEmpty())
                    moveLeft();
                xOrbit = xOrigin + 1;
            }
            // orbit right of origin (to move to left)
            else if (xOrigin < xOrbit)
            {
                if (isCellToLeftNotEmpty() && isCellToRightNotEmpty())
                    (xOrigin, xOrbit) = (xOrbit, xOrigin);
                else if (isCellToLeftNotEmpty())
                    moveRight();
                xOrbit = xOrigin - 1;
            }
        }
        else
        {
            // for some reason these are flipped and i have no idea why but it works
            // orbit above origin
            if (yOrbit > yOrigin)
            {
                yOrbit = yOrigin - 1;
            }
            // orbit below origin
            else if (yOrbit < yOrigin)
            {
                // ReSharper disable once ReplaceWithSingleAssignment.False
                bool kick = false;

                // ReSharper disable once ConvertIfToOrExpression
                if (yOrigin >= Const.ROWS - 1)
                    kick = true;

                if (yOrigin >= 0 && !kick)
                {
                    if (board[yOrigin + 1, xOrigin] != TileType.Empty)
                        kick = true;
                }

                if (kick)
                {
                    yOrigin--;
                    yOrbit--;
                }

                yOrbit = yOrigin + 1;
            }
        }
    }

    private bool isSideways()
    {
        return yOrigin == yOrbit;
    }
}
