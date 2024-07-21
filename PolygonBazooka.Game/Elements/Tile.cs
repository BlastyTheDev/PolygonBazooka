using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace PolygonBazooka.Game.Elements;

public partial class Tile(TileType type, int x, int y) : CompositeDrawable
{
    // TODO: render the tile with a position on the board
    private readonly Vector2 position = new((x - 1) / Const.SCALE_ADJUST, -y / Const.SCALE_ADJUST);
    // private readonly Vector2 position = new(x, -y);

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        string textureName = type switch
        {
            TileType.Red => "red",
            TileType.Green => "green",
            TileType.Blue => "blue",
            TileType.Yellow => "yellow",
            TileType.Bonus => "bonus",
            _ => throw new ArgumentOutOfRangeException()
        };

        Texture texture = textures.Get(textureName);
        texture.ScaleAdjust = Const.SCALE_ADJUST;
        InternalChild = new Container
        {
            AutoSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Children = new Drawable[]
            {
                new Sprite
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Texture = texture,
                    Position = position,
                }
            }
        };
    }
}
