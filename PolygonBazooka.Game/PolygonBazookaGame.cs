﻿using System;
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
    protected GameState State = GameState.Playing;

    private Player player;

    private bool retryHeld;
    private long retryHeldStart;

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

        screenStack.Push(new MainScreen(player = new Player
        {
            Origin = Anchor.Centre,
            Anchor = Anchor.TopLeft,
            Position = new(500, 200)
        }));
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        State = player.Failed ? GameState.GameOver : GameState.Playing;
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (State == GameState.Playing && !e.Repeat)
        {
            switch (e.Key)
            {
                case Key.A:
                    player.MoveLeftInputDown();
                    return true;

                case Key.D:
                    player.MoveRightInputDown();
                    return true;

                // useless
                case Key.S:
                    player.SoftDrop(true);
                    return true;
            }

            if (!player.IsClearing())
            {
                switch (e.Key)
                {
                    case Key.Right:
                        player.RotateCw();
                        return true;

                    case Key.Left:
                        player.RotateCcw();
                        return true;

                    case Key.Up:
                        player.Flip();
                        return true;

                    case Key.Space:
                        player.HardDrop();
                        return true;
                }
            }
        }

        if (e.Key == Key.R)
        {
            if (player.Failed)
            {
                if (!retryHeld)
                    retryHeldStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                retryHeld = true;

                if (retryHeld && DateTimeOffset.Now.ToUnixTimeMilliseconds() - retryHeldStart >= 1000)
                {
                    player.Reset();
                    retryHeld = false;
                }

                return true;
            }
        }

        return false;
    }

    protected override void OnKeyUp(KeyUpEvent e)
    {
        if (State == GameState.Playing)
        {
            switch (e.Key)
            {
                case Key.A:
                    player.MoveLeftInputUp();
                    break;

                case Key.D:
                    player.MoveRightInputUp();
                    break;

                // useless
                case Key.S:
                    player.SoftDrop(false);
                    break;

                case Key.R:
                    retryHeld = false;
                    break;
            }
        }
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        return false;
    }
}
