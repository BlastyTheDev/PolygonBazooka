using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using PolygonBazooka.Game.Elements;

namespace PolygonBazooka.Game;

public partial class MainScreen : Screen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new LocalPlayer
            {
                // Anchor = Anchor.Centre,
            },
        };
    }
}
