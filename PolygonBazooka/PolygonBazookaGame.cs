using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using PolygonBazooka.Elements;
using PolygonBazooka.Screens;

namespace PolygonBazooka;

public class PolygonBazookaGame : Game
{
    private readonly ScreenManager _screenManager = new();
    private readonly Dictionary<ScreenName, GameScreen> _screens = new();

    public readonly DiscordRichPresence DiscordRpc = new();

    public PolygonBazookaGame()
    {
        var graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = 1280;
        graphics.PreferredBackBufferHeight = 720;
        graphics.SynchronizeWithVerticalRetrace = false;
        graphics.ApplyChanges();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        IsFixedTimeStep = false;

        Components.Add(_screenManager);
    }

    protected override void Initialize()
    {
        _screens.Add(ScreenName.MainMenu, new MainMenuScreen(this));
        _screens.Add(ScreenName.Playing, new PlayingScreen(this));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // playing screen for now
        LoadScreen(ScreenName.Playing);
    }

    private void LoadScreen(ScreenName screen)
    {
        _screenManager.LoadScreen(_screens[screen]);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        base.Draw(gameTime);
    }
}