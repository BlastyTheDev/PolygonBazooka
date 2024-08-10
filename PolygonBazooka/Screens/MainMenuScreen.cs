using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;

namespace PolygonBazooka.Screens;

public enum Menus
{
    MainMenu,
    MultiplayerMenu,
    RankedMenu,
    ConfigMenu,
}

public class MainMenuScreen : GameScreen
{
    private readonly SpriteBatch _spriteBatch;

    private readonly PolygonBazookaGame _game;

    private Menus _currentMenu = Menus.MainMenu;

    private const int ButtonYGap = 10;
    private const int ButtonYOffset = 40;

    // first level buttons
    private readonly Texture2D _singleplayerButton;
    private readonly Texture2D _singleplayerButtonHover;
    private readonly Texture2D _singleplayerButtonPress;
    private Rectangle _singleplayerButtonBounds;
    private bool _singleplayerButtonHovered;
    private bool _singleplayerButtonPressed;

    private readonly Texture2D _multiplayerButton;
    private readonly Texture2D _multiplayerButtonHover;
    private readonly Texture2D _multiplayerButtonPress;
    private Rectangle _multiplayerButtonBounds;
    private bool _multiplayerButtonHovered;
    private bool _multiplayerButtonPressed;

    private readonly Texture2D _configButton;
    private readonly Texture2D _configButtonHover;
    private readonly Texture2D _configButtonPress;
    private Rectangle _configButtonBounds;
    private bool _configButtonHovered;
    private bool _configButtonPressed;

    // second level buttons
    private readonly Texture2D _rankedButton;
    private readonly Texture2D _rankedButtonHover;
    private readonly Texture2D _rankedButtonPress;
    private Rectangle _rankedButtonBounds;
    private bool _rankedButtonHovered;
    private bool _rankedButtonPressed;

    // other non main buttons/elements
    private readonly Texture2D _switchAccountButton;
    private readonly Texture2D _switchAccountButtonHover;
    private Rectangle _switchAccountButtonBounds;
    private bool _switchAccountButtonHovered;
    private bool _switchAccountButtonPressed;

    private readonly Texture2D _accountIndicator;
    private Rectangle _accountIndicatorBounds;

    private readonly Texture2D _sliderTab;
    private readonly Texture2D _sliderTabHover;
    private Rectangle _sliderTabBounds;
    private bool _sliderTabHovered;
    private bool _sliderTabPressed;

    private readonly Texture2D _sliderGuide;
    private Rectangle _sliderGuideBounds;

    private int _lastWindowWidth;
    private int _lastWindowHeight;

    public MainMenuScreen(PolygonBazookaGame game) : base(game)
    {
        _game = game;

        _spriteBatch = new SpriteBatch(game.GraphicsDevice);

        _singleplayerButton = Game.Content.Load<Texture2D>("Textures/ui/singleplayer_button");
        _singleplayerButtonHover = Game.Content.Load<Texture2D>("Textures/ui/singleplayer_button_hover");
        _singleplayerButtonPress = Game.Content.Load<Texture2D>("Textures/ui/singleplayer_button_pressed");

        _multiplayerButton = Game.Content.Load<Texture2D>("Textures/ui/multiplayer_button");
        _multiplayerButtonHover = Game.Content.Load<Texture2D>("Textures/ui/multiplayer_button_hover");
        _multiplayerButtonPress = Game.Content.Load<Texture2D>("Textures/ui/multiplayer_button_pressed");

        _configButton = Game.Content.Load<Texture2D>("Textures/ui/config_button");
        _configButtonHover = Game.Content.Load<Texture2D>("Textures/ui/config_button_hover");
        _configButtonPress = Game.Content.Load<Texture2D>("Textures/ui/config_button_pressed");

        _rankedButton = Game.Content.Load<Texture2D>("Textures/ui/ranked_button");
        _rankedButtonHover = Game.Content.Load<Texture2D>("Textures/ui/ranked_button_hover");
        _rankedButtonPress = Game.Content.Load<Texture2D>("Textures/ui/ranked_button_pressed");

        _switchAccountButton = Game.Content.Load<Texture2D>("Textures/ui/switch_account_button");
        _switchAccountButtonHover = Game.Content.Load<Texture2D>("Textures/ui/switch_account_button_hover");

        _sliderTab = Game.Content.Load<Texture2D>("Textures/ui/config_slider_tab");
        _sliderTabHover = Game.Content.Load<Texture2D>("Textures/ui/config_slider_tab_hover");

        _sliderGuide = Game.Content.Load<Texture2D>("Textures/ui/config_slider_guide");

        _accountIndicator = Game.Content.Load<Texture2D>("Textures/ui/you_are_logged_in_as");
    }

    private void UpdateMainMenu(GameTime gameTime, MouseState mouseState, Rectangle mousePosition)
    {
        // Singleplayer Button
        if (_singleplayerButtonBounds.Intersects(mousePosition))
        {
            _singleplayerButtonHovered = true;

            if (mouseState.LeftButton == ButtonState.Pressed && !_multiplayerButtonPressed && !_configButtonPressed)
            {
                _singleplayerButtonPressed = true;
            }
        }
        else
        {
            _singleplayerButtonHovered = false;
        }

        if (mouseState.LeftButton == ButtonState.Released && _singleplayerButtonPressed)
        {
            _singleplayerButtonPressed = false;

            if (_singleplayerButtonHovered)
            {
                _game.ChangeGameState(GameState.SoloPlaying);
            }
        }

        // Multiplayer Button
        if (_multiplayerButtonBounds.Intersects(mousePosition))
        {
            _multiplayerButtonHovered = true;

            if (mouseState.LeftButton == ButtonState.Pressed && !_singleplayerButtonPressed & !_configButtonPressed)
            {
                _multiplayerButtonPressed = true;
            }
        }
        else
        {
            _multiplayerButtonHovered = false;
        }

        if (mouseState.LeftButton == ButtonState.Released && _multiplayerButtonPressed)
        {
            _multiplayerButtonPressed = false;

            if (_multiplayerButtonHovered)
            {
                _currentMenu = Menus.MultiplayerMenu;
            }
        }

        // Config Button
        if (_configButtonBounds.Intersects(mousePosition))
        {
            _configButtonHovered = true;

            if (mouseState.LeftButton == ButtonState.Pressed && !_singleplayerButtonPressed &&
                !_multiplayerButtonPressed)
            {
                _configButtonPressed = true;
            }
        }
        else
        {
            _configButtonHovered = false;
        }

        if (mouseState.LeftButton == ButtonState.Released && _configButtonPressed)
        {
            _configButtonPressed = false;

            if (_configButtonHovered)
            {
                _currentMenu = Menus.ConfigMenu;
            }
        }
    }

    private void UpdateMultiplayerMenu(GameTime gameTime, MouseState mouseState, Rectangle mousePosition)
    {
        if (_rankedButtonBounds.Intersects(mousePosition))
        {
            _rankedButtonHovered = true;

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                _rankedButtonPressed = true;
            }
        }
        else
        {
            _rankedButtonHovered = false;
        }

        if (mouseState.LeftButton == ButtonState.Released && _rankedButtonPressed)
        {
            _rankedButtonPressed = false;

            if (_rankedButtonHovered)
            {
                _currentMenu = Menus.RankedMenu;
            }
        }
    }

    private void UpdateRankedMenu(GameTime gameTime, MouseState mouseState, Rectangle mousePosition)
    {
    }

    private void UpdateConfigMenu(GameTime gameTime, MouseState mouseState, Rectangle mousePosition)
    {
    }

    public override void Update(GameTime gameTime)
    {
        var mouseState = Mouse.GetState();
        Rectangle mousePosition = new(mouseState.X, mouseState.Y, 0, 0);

        // Switch Account Button
        if (_switchAccountButtonBounds.Intersects(mousePosition))
        {
            _switchAccountButtonHovered = true;

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                _switchAccountButtonPressed = true;
            }
        }
        else
        {
            _switchAccountButtonHovered = false;
        }

        if (mouseState.LeftButton == ButtonState.Released && _switchAccountButtonPressed)
        {
            _switchAccountButtonPressed = false;

            if (_switchAccountButtonHovered)
            {
            }
        }

        switch (_currentMenu)
        {
            case Menus.MainMenu:
                UpdateMainMenu(gameTime, mouseState, mousePosition);
                break;

            case Menus.MultiplayerMenu:
                UpdateMultiplayerMenu(gameTime, mouseState, mousePosition);
                break;

            case Menus.RankedMenu:
                UpdateRankedMenu(gameTime, mouseState, mousePosition);
                break;

            case Menus.ConfigMenu:
                UpdateConfigMenu(gameTime, mouseState, mousePosition);
                break;
        }
    }

    private void DrawMainMenu(GameTime gameTime)
    {
        // Singleplayer button
        if (_singleplayerButtonHovered && !_singleplayerButtonPressed)
            _spriteBatch.Draw(_singleplayerButtonHover, _singleplayerButtonBounds, Color.White);
        else if (_singleplayerButtonPressed)
            _spriteBatch.Draw(_singleplayerButtonPress, _singleplayerButtonBounds, Color.White);
        else _spriteBatch.Draw(_singleplayerButton, _singleplayerButtonBounds, Color.White);

        // Multiplayer button
        if (_multiplayerButtonHovered && !_multiplayerButtonPressed)
            _spriteBatch.Draw(_multiplayerButtonHover, _multiplayerButtonBounds, Color.White);
        else if (_multiplayerButtonPressed)
            _spriteBatch.Draw(_multiplayerButtonPress, _multiplayerButtonBounds, Color.White);
        else _spriteBatch.Draw(_multiplayerButton, _multiplayerButtonBounds, Color.White);

        // Config button
        if (_configButtonHovered && !_configButtonPressed)
            _spriteBatch.Draw(_configButtonHover, _configButtonBounds, Color.White);
        else if (_configButtonPressed)
            _spriteBatch.Draw(_configButtonPress, _configButtonBounds, Color.White);
        else _spriteBatch.Draw(_configButton, _configButtonBounds, Color.White);
    }

    private void DrawMultiplayerMenu(GameTime gameTime)
    {
        // Ranked button
        if (_rankedButtonHovered && !_rankedButtonPressed)
            _spriteBatch.Draw(_rankedButtonHover, _rankedButtonBounds, Color.White);
        else if (_rankedButtonPressed)
            _spriteBatch.Draw(_rankedButtonPress, _rankedButtonBounds, Color.White);
        else _spriteBatch.Draw(_rankedButton, _rankedButtonBounds, Color.White);
    }

    private void DrawRankedMenu(GameTime gameTime)
    {
    }

    private void DrawConfigMenu(GameTime gameTime)
    {
        _spriteBatch.Draw(_sliderGuide, _sliderGuideBounds, Color.White);
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Account indicator
        _spriteBatch.Draw(_accountIndicator, _accountIndicatorBounds, Color.White);

        // Switch account button
        _spriteBatch.Draw(_switchAccountButtonHovered ? _switchAccountButtonHover : _switchAccountButton,
            _switchAccountButtonBounds, Color.White);

        switch (_currentMenu)
        {
            case Menus.MainMenu:
                DrawMainMenu(gameTime);
                break;

            case Menus.MultiplayerMenu:
                DrawMultiplayerMenu(gameTime);
                break;

            case Menus.RankedMenu:
                DrawRankedMenu(gameTime);
                break;

            case Menus.ConfigMenu:
                DrawConfigMenu(gameTime);
                break;
        }


        _spriteBatch.End();

        // TODO: update these separately
        if (_lastWindowHeight != Game.Window.ClientBounds.Height || _lastWindowWidth != Game.Window.ClientBounds.Width)
        {
            _lastWindowWidth = Game.Window.ClientBounds.Width;
            _lastWindowHeight = Game.Window.ClientBounds.Height;

            _singleplayerButtonBounds = new Rectangle(
                _lastWindowWidth / 2 - (int)(_singleplayerButton.Width * _game.Scale) / 2,
                _lastWindowHeight / 2 - (int)(_singleplayerButton.Height * _game.Scale) * 2 +
                (int)(1 * ButtonYGap * _game.Scale + ButtonYOffset * _game.Scale),
                (int)(_singleplayerButton.Width * _game.Scale), (int)(_singleplayerButton.Height * _game.Scale));

            _multiplayerButtonBounds = new Rectangle(
                _lastWindowWidth / 2 - (int)(_singleplayerButton.Width * _game.Scale) / 2,
                _lastWindowHeight / 2 - (int)(_singleplayerButton.Height * _game.Scale) +
                (int)(2 * ButtonYGap * _game.Scale + ButtonYOffset * _game.Scale),
                (int)(_singleplayerButton.Width * _game.Scale), (int)(_singleplayerButton.Height * _game.Scale));

            _configButtonBounds = new Rectangle(
                _lastWindowWidth / 2 - (int)(_singleplayerButton.Width * _game.Scale) / 2,
                _lastWindowHeight / 2 + (int)(3 * ButtonYGap * _game.Scale + ButtonYOffset * _game.Scale),
                (int)(_singleplayerButton.Width * _game.Scale), (int)(_singleplayerButton.Height * _game.Scale));

            _accountIndicatorBounds = new Rectangle(
                _lastWindowWidth / 2 - (int)(_accountIndicator.Width * _game.Scale) / 2,
                (int)(_accountIndicator.Height * _game.Scale),
                (int)(_accountIndicator.Width * _game.Scale), (int)(_accountIndicator.Height * _game.Scale));

            _switchAccountButtonBounds = new Rectangle(
                _lastWindowWidth / 2 - (int)(_switchAccountButton.Width * _game.Scale) / 2,
                (int)(_accountIndicator.Height * _game.Scale + 50 * _game.Scale),
                (int)(_switchAccountButton.Width * _game.Scale), (int)(_switchAccountButton.Height * _game.Scale));

            // ranked button in same position as singleplayer button, subject to change
            _rankedButtonBounds = _singleplayerButtonBounds;

            _sliderTabBounds = new Rectangle(
                _lastWindowWidth / 2 - (int)(_sliderTab.Width * _game.Scale) / 2,
                _lastWindowHeight / 2 - (int)(_sliderTab.Height * _game.Scale) / 2,
                (int)(_sliderTab.Width * _game.Scale), (int)(_sliderTab.Height * _game.Scale));

            // TODO: needs fixing
            _sliderGuideBounds = new Rectangle(
                _lastWindowWidth / 2 - (int)(_sliderTab.Width * _game.Scale),
                _lastWindowHeight / 2 - (int)(_sliderTab.Height * _game.Scale) / 2,
                (int)(_sliderGuide.Width * _game.Scale), (int)(_sliderGuide.Height * _game.Scale));
        }
    }
}