using osu.Framework.Graphics;

namespace PolygonBazooka.Game.Elements;

public partial class Tile(TileType type) : Drawable
{
    public TileType Type { get; set; } = type;
}
