using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;
using PolygonBazooka.Game.Elements;

namespace PolygonBazooka.Game;

public partial class MainScreen(Player player) : Screen
{
    private readonly SpriteText failedText = new()
    {
        Text = "Game Over! Hold R to try again.",
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre,
        Font = new FontUsage(size: 64),
        Colour = Colour4.Red,
    };

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            player,
            failedText,
        };
        failedText.MoveToY(-1000);
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        if (player.Failed)
            failedText.MoveToY(50);
        else if (failedText.Position.Y > -1000)
            failedText.MoveToY(-1000);
    }
}
