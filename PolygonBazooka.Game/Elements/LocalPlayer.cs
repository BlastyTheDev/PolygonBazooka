using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace PolygonBazooka.Game.Elements;

/// <summary>
/// The board of the local player.
/// </summary>
public partial class LocalPlayer : CompositeDrawable
{
    public LocalPlayer()
    {
        AutoSizeAxes = Axes.Both;
        Origin = Anchor.Centre;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        Texture texture = textures.Get("board");
        texture.ScaleAdjust = 0.5f;
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
