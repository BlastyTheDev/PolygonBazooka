﻿using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Input;
using PolygonBazooka.Game.Elements;

namespace PolygonBazooka.Game;

public partial class PolygonBazookaGame : PolygonBazookaGameBase
{
    private ScreenStack screenStack;

    private readonly LocalPlayer player = LocalPlayer.GetInstance();

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
        switch (e.Key)
        {
            case Key.Left:
                player.FallingBlock.MoveLeft();
                break;

            case Key.Right:
                player.FallingBlock.MoveRight();
                break;

            case Key.Down:
                player.HardDropFallingBlock();
                break;

            case Key.Up:
                player.FallingBlock.RotateCw();
                break;

            case Key.Space:
                player.FallingBlock.Drop();
                break;
        }

        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        return false;
    }
}
