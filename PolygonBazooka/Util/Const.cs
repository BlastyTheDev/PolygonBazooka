using PolygonBazooka.Elements;

namespace PolygonBazooka.Util;

public class Const
{
    public const int CwRotation = -1;
    public const int CcwRotation = 1;

    public const int Rows = 12;
    public const int Cols = 7;

    public static readonly TileType[] QueueTileTypes =
    [
        TileType.Blue,
        TileType.Green,
        TileType.Red,
        TileType.Yellow,
        TileType.Bonus,
    ];
}