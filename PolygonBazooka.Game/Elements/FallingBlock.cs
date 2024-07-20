namespace PolygonBazooka.Game.Elements;

/// <summary>
/// A falling block composed of 2 tiles stuck together, one on top of the other.
/// The rotation origin is the bottom tile.
/// </summary>
public class FallingBlock
{
    private readonly TileType[] tiles = new TileType[2];

    private readonly LocalPlayer player = LocalPlayer.GetInstance();

    private int xTop { get; set; } = 3;
    private int yTop { get; set; } = -3;
    private int xBot { get; set; } = 3;
    private int yBot { get; set; } = -2;

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

    // TODO: drop the lower block first
    public void Drop()
    {
        player.SetTile(xTop, player.GetLowestEmptyYInCol(xTop), tiles[0]);
        player.SetTile(xBot, player.GetLowestEmptyYInCol(xBot), tiles[1]);
    }

    public FallingBlock(TileType top, TileType bottom)
    {
        tiles[0] = top;
        tiles[1] = bottom;
    }

    private bool isSideways()
    {
        return xTop == xBot;
    }
}
