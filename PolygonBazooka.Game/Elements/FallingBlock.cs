using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace PolygonBazooka.Game.Elements;

/// <summary>
/// A falling block composed of 2 tiles stuck together, one on top of the other.
/// The rotation origin is the bottom tile.
/// </summary>
public partial class FallingBlock : CompositeDrawable
{
    private readonly TileType[] tiles = new TileType[2];

    private readonly LocalPlayer player = LocalPlayer.GetInstance();

    private int xTop { get; set; } = 3;
    private int yTop { get; set; } = -3;
    private int xBot { get; set; } = 3;
    private int yBot { get; set; } = -2;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new Tile(tiles[0], xTop, yTop),
            new Tile(tiles[1], xBot, yBot),
        };
    }

    public void MoveLeft()
    {
        xTop -= 1;
        xBot -= 1;
    }

    public void MoveRight()
    {
        xTop += 1;
        xBot += 1;
    }

    public void RotateCw()
    {
        if (xBot + 1 > Const.COLS - 1)
            MoveLeft();
        xTop = xBot + 1;
        yTop = yBot;
    }

    public void RotateCcw()
    {
        if (xBot - 1 < 0)
            MoveRight();
        xTop = xBot - 1;
        yTop = yBot;
    }

    public void Flip()
    {
        if (isSideways()) (xTop, xBot) = (xBot, xTop);
        else (yTop, yBot) = (yBot, yTop);
    }

    public void Drop()
    {
        // if bot (origin) is lower than top or the same
        if (yBot <= yTop)
        {
            dropBotTile();
            dropTopTile();
        }
        // if top is lower than bot (origin)
        else if (yTop < yBot)
        {
            dropTopTile();
            dropBotTile();
        }
    }

    public void NextFallingBlock()
    {
        randomiseTiles();
        // reset location
        xTop = 3;
        yTop = -3;
        xBot = 3;
        yBot = -2;
    }

    private void dropBotTile()
    {
        player.SetTile(xBot, player.GetLowestEmptyYInCol(xBot), tiles[1]);
    }

    private void dropTopTile()
    {
        player.SetTile(xTop, player.GetLowestEmptyYInCol(xTop), tiles[0]);
    }

    public FallingBlock(TileType top, TileType bottom)
    {
        tiles[0] = top;
        tiles[1] = bottom;
        load();
    }

    /// <summary>
    /// Create a falling block with random tiles
    /// </summary>
    public FallingBlock()
    {
        randomiseTiles();
    }

    private void randomiseTiles()
    {
        Random random = new();
        TileType[] types = (TileType[])Enum.GetValues(typeof(TileType));
        tiles[0] = types[random.Next(1, types.Length)];
        tiles[1] = types[random.Next(1, types.Length)];
        load();
    }

    private bool isSideways()
    {
        return xTop == xBot;
    }
}
