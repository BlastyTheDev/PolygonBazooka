using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Input;
using PolygonBazooka.Game.Elements;

namespace PolygonBazooka.Game;

public partial class PolygonBazookaGame : PolygonBazookaGameBase
{
    private ScreenStack screenStack;

    // Start as playing for now
    private GameState gameState = GameState.Playing;

    private readonly Player player = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        // Add your top-level game components here.
        // A screen stack and sample screen has been provided for convenience, but you can replace it if you don't want to use screens.
        Child = screenStack = new ScreenStack { RelativeSizeAxes = Axes.Both };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        screenStack.Push(new MainScreen());
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (gameState == GameState.Playing)
        {
            switch (e.Key)
            {
                case Key.Left:
                    player.MoveLeftInputDown();
                    return true;

                case Key.Right:
                    player.MoveRightInputDown();
                    return true;

                // case Key.Down:
                //     player.();
                //     break;

                case Key.Up:
                    player.RotateCw();
                    return true;

                // case Key.Space:
                //     player.();
                //     break;
            }
        }

        return false;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        return false;
    }
}
