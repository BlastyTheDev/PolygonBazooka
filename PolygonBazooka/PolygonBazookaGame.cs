using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using PolygonBazooka.Networking;
using PolygonBazooka.Screens;
using PolygonBazooka.Util;

namespace PolygonBazooka
{
    public enum GameState
    {
        MainMenu,
        SoloPlaying,
        SoloGameOver,
        RankedQueuing,
        RankedGameStart,
        RankedPlaying,
        RankedGameOver,
    }

    public class PolygonBazookaGame : Game
    {
        private readonly ScreenManager _screenManager = new();
        private readonly Dictionary<ScreenName, GameScreen> _screens = new();

        public readonly DiscordRichPresence DiscordRpc = new();

        public readonly Preferences Preferences = new();

        public readonly Authentication Authentication = new();
        public readonly RankedSocket RankedSocket;

        public readonly Textures Textures;

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
            Textures = new(this);
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            IsFixedTimeStep = false;

            Components.Add(_screenManager);
            
            RankedSocket = new(this);

            // DEBUG
            // Authentication.LoginAsync("test", "test", true).Wait();
            // Authentication.RegisterAsync("test", "test", "test@test.test").Wait();

            // RankedSocket.ConnectAsync().Wait();
        }

        protected override void Initialize()
        {
            _screens.Add(ScreenName.MainMenu, new MainMenuScreen(this));
            _screens.Add(ScreenName.Playing, new PlayingScreen(this));
            _screens.Add(ScreenName.RankedMenu, new RankedMenuScreen(this));
            _screens.Add(ScreenName.RankedPlay, new RankedMatchScreen(this));

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

                case GameState.SoloPlaying:
                    LoadScreen(ScreenName.Playing);
                    break;

                case GameState.RankedPlaying:
                    LoadScreen(ScreenName.RankedPlay);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }
        protected override void LoadContent()
        {
            LoadScreen(ScreenName.MainMenu);
        }

        public void LoadScreen(ScreenName screen)
        {
            _screenManager.LoadScreen(_screens[screen]);
        }

        protected override void Update(GameTime gameTime)
        {
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            RankedSocket.DisconnectAsync().Wait();

            Preferences.Save();
            Authentication.Dispose();
        }
    }
}