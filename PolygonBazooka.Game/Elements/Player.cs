using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;

namespace PolygonBazooka.Game.Elements;

public partial class Player : CompositeDrawable
{
    private TileType[,] board { get; }
    private readonly List<Tile> renderedTiles = new();

    // origin tile of the falling block
    private TileType fallingBlockOrigin { get; set; } = TileType.Yellow;

    // tile that orbits around the origin when rotated
    private TileType fallingBlockOrbit { get; set; } = TileType.Green;

    // position of the tile in board coordinates
    private int xOrigin = 3;
    private int yOrigin = -2;

    private int xOrbit = 3;
    private int yOrbit = -3;

    // blocks in queue
    private TileType nextBlockOrigin { get; set; } = TileType.Blue;
    private TileType nextBlockOrbit { get; set; } = TileType.Bonus;

    private TileType nextNextBlockOrigin { get; set; } = TileType.Green;
    private TileType nextNextBlockOrbit { get; set; } = TileType.Red;

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

    protected override void Update()
    {
        base.Update();
        Console.WriteLine("Origin X: " + xOrigin + " Y: " + yOrigin);
        Console.WriteLine("Orbit X: " + xOrbit + " Y: " + yOrbit);
        // renderTiles();
    }

    private void renderTiles()
    {
        // render board tiles
        for (int row = 0; row < Const.ROWS; row++)
        {
            for (int col = 0; col < Const.COLS; col++)
            {
                if (board[row, col] != TileType.Empty)
                {
                    var tile = new Tile(col, row, getTileAnimation(board[row, col]));
                    AddInternal(tile);
                    renderedTiles.Add(tile);
                }
            }
        }

        // render falling block tiles
        // if (fallingBlockOrigin != TileType.Empty && fallingBlockOrbit != TileType.Empty)
        // {
        //     AddInternal(new Sprite
        //     {
        //         Texture = getTileAnimation(fallingBlockOrigin).CurrentFrame,
        //         Position = new Vector2(2 + xOrigin * 8, 2 + yOrigin * 8),
        //     });
        //     AddInternal(new Sprite
        //     {
        //         Texture = getTileAnimation(fallingBlockOrbit).CurrentFrame,
        //         Position = new Vector2(2 + xOrbit * 8, 2 + yOrbit * 8),
        //     });
        // }
        //
        // // render next in queue
        // if (nextBlockOrigin != TileType.Empty && nextBlockOrbit != TileType.Empty)
        // {
        //     AddInternal(new Sprite
        //     {
        //         Texture = getTileAnimation(nextBlockOrigin).CurrentFrame,
        //         Position = new Vector2(4 + 8 * 7, 7),
        //     });
        //     AddInternal(new Sprite
        //     {
        //         Texture = getTileAnimation(nextBlockOrbit).CurrentFrame,
        //         Position = new Vector2(4 + 8 * 7, 15),
        //     });
        // }
        //
        // if (nextNextBlockOrigin != TileType.Empty && nextNextBlockOrbit != TileType.Empty)
        // {
        //     AddInternal(new Sprite
        //     {
        //         Texture = getTileAnimation(nextNextBlockOrigin).CurrentFrame,
        //         Position = new Vector2(4 + 8 * 7, 25),
        //     });
        //     AddInternal(new Sprite
        //     {
        //         Texture = getTileAnimation(nextNextBlockOrbit).CurrentFrame,
        //         Position = new Vector2(4 + 8 * 7, 33),
        //     });
        // }
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

    public void MoveLeft()
    {
        Console.WriteLine("MoveLeft");
        xOrigin -= 1;
        xOrbit -= 1;
    }

    public void MoveRight()
    {
        Console.WriteLine("MoveRight");
        xOrigin += 1;
        xOrbit += 1;
    }

    public void RotateCw()
    {
        if (xOrbit + 1 > Const.COLS - 1)
            MoveLeft();
        xOrigin = xOrbit + 1;
        yOrigin = yOrbit;
    }

    public void RotateCcw()
    {
        if (xOrbit - 1 < 0)
            MoveRight();
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
