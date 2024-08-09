using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using PolygonBazooka.Screens;

namespace PolygonBazooka
{
    public enum GameState
    {
        MainMenu,
        Playing,
        SoloGameOver,
    }

    public class PolygonBazookaGame : Game
    {
        private readonly ScreenManager _screenManager = new();
        private readonly Dictionary<ScreenName, GameScreen> _screens = new();

        public readonly DiscordRichPresence DiscordRpc = new();

        public GameState State { get; private set; } = GameState.MainMenu;

        public float Scale { get; private set; } = 1;

        private int _lastWindowWidth;
        private int _lastWindowHeight;

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

        public void ChangeGameState(GameState newState)
        {
            State = newState;
            DiscordRpc.UpdateStatus(newState);

            switch (newState)
            {
                case GameState.MainMenu:
                    LoadScreen(ScreenName.MainMenu);
                    break;
                case GameState.Playing:
                    LoadScreen(ScreenName.Playing);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        protected override void LoadContent()
        {
            LoadScreen(ScreenName.MainMenu);
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

            if (_lastWindowWidth != Window.ClientBounds.Width || _lastWindowHeight != Window.ClientBounds.Height)
            {
                Scale = (float)Window.ClientBounds.Height / 300;
                _lastWindowWidth = Window.ClientBounds.Width;
                _lastWindowHeight = Window.ClientBounds.Height;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}