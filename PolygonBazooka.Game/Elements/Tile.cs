using osu.Framework.Allocation;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace PolygonBazooka.Game.Elements;

public partial class Tile(int x, int y, TextureAnimation animation, bool queue = false, int queueOffset = 7) : CompositeDrawable
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Scale = Const.SCALE_ADJUST;
        Position = queue ? new Vector2(4 + 8 * 7, queueOffset) : new Vector2(2 + x * 8, 2 + y * 8);
        AddInternal(animation);
    }
}
