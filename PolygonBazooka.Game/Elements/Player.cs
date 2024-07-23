using System;
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

    // origin tile of the falling block
    private TileType fallingBlockOrigin { get; set; }

    // tile that orbits around the origin when rotated
    private TileType fallingBlockOrbit { get; set; }

    private readonly Sprite[] renderedFallingTiles;

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
    public float DelayedAutoShift = 500;
    public float AutoRepeatRate = 100;

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
        resetBoard();
        SetFallingBlock(TileType.Yellow, TileType.Green);
    }

    [Resolved]
    private TextureStore textures { get; set; }

    private TextureAnimation boardAnimation;
    private TextureAnimation blueTileAnimation;
    private TextureAnimation greenTileAnimation;
    private TextureAnimation redTileAnimation;
    private TextureAnimation yellowTileAnimation;
    private TextureAnimation bonusTileAnimation;

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

        greenTileAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre, };
        greenTileAnimation.AddFrame(textures.Get("green"));

        redTileAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre, };
        redTileAnimation.AddFrame(textures.Get("red"));

        yellowTileAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre, };
        yellowTileAnimation.AddFrame(textures.Get("yellow"));

        bonusTileAnimation = new TextureAnimation { Origin = Anchor.TopLeft, Anchor = Anchor.Centre, };
        bonusTileAnimation.AddFrame(textures.Get("bonus"));

        // render the board at the bottom layer
        AddInternal(boardAnimation);
    }

    protected override void Update()
    {
        base.Update();
        // Console.WriteLine("Origin X: " + xOrigin + " Y: " + yOrigin);
        // Console.WriteLine("Orbit X: " + xOrbit + " Y: " + yOrbit);

        // left DAS
        if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - leftPressStart >= DelayedAutoShift && leftPressed && !leftDasActive)
        {
            moveLeft();
            leftDasActive = true;
            lastLeftAutoRepeat = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        if ((leftDasActive && leftPressed) && DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastLeftAutoRepeat > AutoRepeatRate)
        {
            moveLeft();
            lastLeftAutoRepeat = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        // right DAS
        if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - rightPressStart >= DelayedAutoShift && rightPressed && !rightDasActive)
        {
            moveRight();
            rightDasActive = true;
            lastRightAutoRepeat = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        if ((rightDasActive && rightPressed) && DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastRightAutoRepeat > AutoRepeatRate)
        {
            moveRight();
            lastRightAutoRepeat = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        // clear

        processGravity();

        renderTiles();
        Console.WriteLine("sprite count: " + (renderedTiles.Length + renderedFallingTiles.Length));

        oldBoard = board.Clone() as TileType[,];
    }

    private Sprite createTileSprite(int row, int col, TileType type, bool queue = false, int offset = 0)
    {
        return new Sprite
        {
            Texture = getTileAnimation(type).CurrentFrame,
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
                if (board[row, col] == TileType.Empty)
                    continue;

                if (board[row, col] == oldBoard[row, col])
                    continue;

                if (renderedTiles[row, col] != null)
                    renderedTiles[row, col].Dispose();

                Sprite sprite = createTileSprite(row, col, board[row, col]);
                renderedTiles[row, col] = sprite;
                AddInternal(sprite);
            }
        }

        // render falling block tiles
        if (fallingBlockOrigin != TileType.Empty && fallingBlockOrbit != TileType.Empty)
        {
            Sprite origin = createTileSprite(yOrigin, xOrigin, fallingBlockOrigin);
            // Sprite oldOrigin = renderedFallingTiles[0];
            renderedFallingTiles[0] = origin;
            // oldOrigin?.Dispose();
            AddInternal(origin);

            Sprite orbit = createTileSprite(yOrbit, xOrbit, fallingBlockOrbit);
            // Sprite oldOrbit = renderedFallingTiles[1];
            renderedFallingTiles[1] = orbit;
            // oldOrbit?.Dispose();
            AddInternal(orbit);
        }

        // render next in queue
        if (nextBlockOrigin != TileType.Empty && nextBlockOrbit != TileType.Empty)
        {
            Sprite origin = createTileSprite(0, 0, nextBlockOrigin, true, 7);
            AddInternal(origin);

            Sprite orbit = createTileSprite(1, 0, nextBlockOrbit, true, 15);
            AddInternal(orbit);
        }

        if (nextNextBlockOrigin != TileType.Empty && nextNextBlockOrbit != TileType.Empty)
        {
            Sprite origin = createTileSprite(0, 0, nextNextBlockOrigin, true, 25);
            AddInternal(origin);

            Sprite orbit = createTileSprite(1, 0, nextNextBlockOrbit, true, 33);
            AddInternal(orbit);
        }
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
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }

    private void resetBoard()
    {
        for (int row = 0; row < 12; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                // set the bottom row to blue for debug purposes
                board[row, col] = TileType.Blue;
            }
        }
    }

    private void processGravity()
    {
        // start at second to bottom row
        for (int row = Const.ROWS - 2; row >= 0; row--)
        {
            for (int col = 0; col < Const.COLS; col++)
            {
                int currentRow = row;

                // while the row being checked is above the bottom row and the tile below is empty
                while (currentRow < Const.ROWS && board[currentRow + 1, col] == TileType.Empty)
                {
                    // move the tile down
                    board[currentRow + 1, col] = board[currentRow, col];
                    board[currentRow, col] = TileType.Empty;
                    // check the row below
                    currentRow++;
                }
            }
        }
    }

    public void MoveLeftInputDown()
    {
        rightPressed = false;
        leftPressed = true;
        leftPressStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        moveLeft();
    }

    public void MoveRightInputDown()
    {
        leftPressed = false;
        rightPressed = true;
        rightPressStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        moveRight();
    }

    private void moveLeft()
    {
        xOrigin -= 1;
        xOrbit -= 1;
    }

    private void moveRight()
    {
        xOrigin += 1;
        xOrbit += 1;
    }

    public void RotateCw()
    {
        if (xOrbit + 1 > Const.COLS - 1)
            MoveLeftInputDown();
        xOrigin = xOrbit + 1;
        yOrigin = yOrbit;
    }

    public void RotateCcw()
    {
        if (xOrbit - 1 < 0)
            MoveRightInputDown();
        xOrigin = xOrbit - 1;
        yOrigin = yOrbit;
    }

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
