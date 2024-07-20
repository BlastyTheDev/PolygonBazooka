using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace PolygonBazooka.Game.Elements;

public partial class Tile : CompositeDrawable
{
    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        Texture texture = textures.Get("");
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
                }
            }
        };
    }
}
