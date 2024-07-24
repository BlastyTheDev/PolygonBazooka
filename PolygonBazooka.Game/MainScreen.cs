using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using PolygonBazooka.Game.Elements;

namespace PolygonBazooka.Game;

public partial class MainScreen(Player player) : Screen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            player
            // new Player
            // {
            //     Origin = Anchor.Centre,
            //     Anchor = Anchor.TopLeft,
            //     Position = new Vector2(100, 200),
            // }
        };
    }
}
