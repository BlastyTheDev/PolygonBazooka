using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osuTK;
using PolygonBazooka.Game.Elements;

namespace PolygonBazooka.Game;

public partial class MainScreen : Screen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new Player
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.TopLeft,
                Position = new Vector2(100, 100),
            }
        };
    }
}
