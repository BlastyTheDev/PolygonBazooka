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
    private TileType[,] board { get; }

    // origin tile of the falling block
    private TileType fallingBlockOrigin { get; set; } = TileType.Empty;

    // tile that orbits around the origin when rotated
    private TileType fallingBlockOrbit { get; set; } = TileType.Empty;

    public Player()
    {
        board = new TileType[Const.ROWS, Const.COLS];
        resetBoard();
    }

    [Resolved]
    private TextureStore textures { get; set; }

    private TextureAnimation boardAnimation;
    private TextureAnimation blueTileAnimation;
    private TextureAnimation greenTileAnimation;
    private TextureAnimation redTileAnimation;
    private TextureAnimation yellowTileAnimation;
    private TextureAnimation bonusTileAnimation;

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
        renderTiles();
    }

    private void renderTiles()
    {
        for (int row = 0; row < Const.ROWS - 1; row++)
        {
            for (int col = 0; col < Const.COLS - 1; col++)
            {
                if (board[row, col] != TileType.Empty)
                {
                    AddInternal(new Sprite
                    {
                        Texture = getTileAnimation(board[row, col]).CurrentFrame,
                        Position = new Vector2(col, row) * Scale,
                    });
                }
            }
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

    // position of the tile in board coordinates
    protected int XOrigin { get; set; }
    protected int YOrigin { get; set; }

    protected int XOrbit { get; set; }
    protected int YOrbit { get; set; }

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
            for (int col = 0; col < Const.COLS - 1; col++)
            {
                int currentRow = row;

                // while the row being checked is above the bottom row and the tile below is empty
                while (currentRow < Const.ROWS - 1 && board[currentRow + 1, col] == TileType.Empty)
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

    public void MoveLeft()
    {
        XOrigin -= 1;
        XOrbit -= 1;
    }

    public void MoveRight()
    {
        XOrigin += 1;
        XOrbit += 1;
    }

    public void RotateCw()
    {
        if (XOrbit + 1 > Const.COLS - 1)
            MoveLeft();
        XOrigin = XOrbit + 1;
        YOrigin = YOrbit;
    }

    public void RotateCcw()
    {
        if (XOrbit - 1 < 0)
            MoveRight();
        XOrigin = XOrbit - 1;
        YOrigin = YOrbit;
    }

    public void Flip()
    {
        if (isSideways()) (XOrigin, XOrbit) = (XOrbit, XOrigin);
        else (YOrigin, YOrbit) = (YOrbit, YOrigin);
    }

    private bool isSideways()
    {
        return XOrigin == XOrbit;
    }
}
