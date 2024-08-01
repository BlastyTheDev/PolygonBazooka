using osuTK;
using PolygonBazooka.Game.Elements;

namespace PolygonBazooka.Game;

public class Const
{
    public const int ROWS = 12;
    public const int COLS = 7;

    public static readonly Vector2 SCALE_ADJUST = new(5f);

    public static readonly TileType[] QUEUE_TILE_TYPES =
    [
        TileType.Blue,
        TileType.Green,
        TileType.Red,
        TileType.Yellow,
        TileType.Bonus,
    ];

    public const int CLEAR_TIME = 50 * 8;
}
