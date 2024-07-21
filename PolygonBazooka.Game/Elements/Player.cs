namespace PolygonBazooka.Game.Elements;

public abstract class Player
{
    protected abstract TileType[,] Tiles { get; }

    // origin tile of the falling block
    protected abstract TileType FallingBlockOrigin { get; }

    // tile that orbits around the origin when rotated
    protected abstract TileType FallingBlockOrbit { get; }

    // position of the tile in board coordinates
    protected int XOrigin { get; set; }
    protected int YOrigin { get; set; }

    protected int XOrbit { get; set; }
    protected int YOrbit { get; set; }

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
