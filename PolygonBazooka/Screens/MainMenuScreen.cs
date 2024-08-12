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

    private bool _configInitialized;
    
    private readonly Texture2D _sliderTab;
    private readonly Texture2D _sliderTabHover;

    private Rectangle _dasSliderTabBounds;
    private Rectangle _arrSliderTabBounds;
    private Rectangle _dcdSliderTabBounds;
    private bool _dasSliderTabHovered;
    private bool _arrSliderTabHovered;
    private bool _dcdSliderTabHovered;
    private bool _dasSliderTabPressed;
    private bool _arrSliderTabPressed;
    private bool _dcdSliderTabPressed;

    private int _configSliderMaxValue;

    private int _dasSliderRawValue;
    private int _arrSliderRawValue;
    private int _dcdSliderRawValue;
    private float _dasSliderPercentage;
    private float _arrSliderPercentage;
    private float _dcdSliderPercentage;

    private readonly Texture2D _sliderGuide;
    private Rectangle _dasSliderGuideBounds;
    private Rectangle _arrSliderGuideBounds;
    private Rectangle _dcdSliderGuideBounds;

    private readonly Texture2D _backButton;
    private readonly Texture2D _backButtonHover;
    private Rectangle _backButtonBounds;
    private bool _backButtonHovered;
    private bool _backButtonPressed;

    private int _lastWindowWidth;
    private int _lastWindowHeight;

    private readonly SpriteFont _font;

    public MainMenuScreen(PolygonBazookaGame game) : base(game)
    {
        _game = game;

        _spriteBatch = new SpriteBatch(game.GraphicsDevice);

        _font = game.Content.Load<SpriteFont>("Fonts/Tiny5");

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

        _backButton = Game.Content.Load<Texture2D>("Textures/ui/back_button");
        _backButtonHover = Game.Content.Load<Texture2D>("Textures/ui/back_button_hover");
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
        if (!_configInitialized)
        {
            _configInitialized = true;
            
            _dasSliderPercentage = Math.Abs((_game.Preferences.DelayedAutoShift - 300) / 300f);
            _arrSliderPercentage = Math.Abs((_game.Preferences.AutoRepeatRate - 300) / 300f);
            _dcdSliderPercentage = Math.Abs((_game.Preferences.DasCutDelay - 300) / 300f);
        }
        
        int sliderTabWidth = _sliderTab.Width;
        int sliderTabHeight = _sliderTab.Height;

        _configSliderMaxValue = (int)(_sliderGuide.Width * _game.Scale - sliderTabWidth * _game.Scale);

        _dasSliderTabBounds = new Rectangle(
            (int)(_dasSliderGuideBounds.X +
                  (_dasSliderGuideBounds.Width - _dasSliderTabBounds.Width) * _dasSliderPercentage),
            (int)(100 * _game.Scale),
            (int)(_sliderTab.Width * _game.Scale), (int)(_sliderTab.Height * _game.Scale));

        _arrSliderTabBounds = new Rectangle(
            (int)(_arrSliderGuideBounds.X +
                  (_arrSliderGuideBounds.Width - _arrSliderTabBounds.Width) * _arrSliderPercentage),
            (int)((100 + sliderTabHeight * 2) * _game.Scale),
            (int)(_sliderTab.Width * _game.Scale), (int)(_sliderTab.Height * _game.Scale));

        _dcdSliderTabBounds = new Rectangle(
            (int)(_dcdSliderGuideBounds.X +
                  (_dcdSliderGuideBounds.Width - _dcdSliderTabBounds.Width) * _dcdSliderPercentage),
            (int)((100 + sliderTabHeight * 4) * _game.Scale),
            (int)(_sliderTab.Width * _game.Scale), (int)(_sliderTab.Height * _game.Scale));

        if (_dasSliderTabBounds.Intersects(mousePosition))
        {
            _dasSliderTabHovered = true;

            if (mouseState.LeftButton == ButtonState.Pressed)
                _dasSliderTabPressed = true;
        }
        else
            _dasSliderTabHovered = false;

        if (mouseState.LeftButton == ButtonState.Released && _dasSliderTabPressed)
            _dasSliderTabPressed = false;

        if (_arrSliderTabBounds.Intersects(mousePosition))
        {
            _arrSliderTabHovered = true;

            if (mouseState.LeftButton == ButtonState.Pressed)
                _arrSliderTabPressed = true;
        }
        else
            _arrSliderTabHovered = false;

        if (mouseState.LeftButton == ButtonState.Released && _arrSliderTabPressed)
            _arrSliderTabPressed = false;

        if (_dcdSliderTabBounds.Intersects(mousePosition))
        {
            _dcdSliderTabHovered = true;

            if (mouseState.LeftButton == ButtonState.Pressed)
                _dcdSliderTabPressed = true;
        }
        else
            _dcdSliderTabHovered = false;

        if (mouseState.LeftButton == ButtonState.Released && _dcdSliderTabPressed)
            _dcdSliderTabPressed = false;

        if (_dasSliderTabPressed && !_arrSliderTabPressed && !_dcdSliderTabPressed ||
            (_dasSliderGuideBounds.Intersects(mousePosition) &&
             mouseState.LeftButton == ButtonState.Pressed))
        {
            _dasSliderRawValue = mousePosition.X - _dasSliderGuideBounds.X - _dasSliderTabBounds.Width / 2;

            if (_dasSliderRawValue < 0)
                _dasSliderRawValue = 0;
            else if (_dasSliderRawValue > _configSliderMaxValue)
                _dasSliderRawValue = _configSliderMaxValue;

            _dasSliderPercentage = _dasSliderRawValue / (float)_configSliderMaxValue;
            _game.Preferences.DelayedAutoShift = (int)(-Math.Round(_dasSliderPercentage * 300) + 300);
        }

        if (_arrSliderTabPressed && !_dasSliderTabPressed && !_dcdSliderTabPressed ||
            (_arrSliderGuideBounds.Intersects(mousePosition) &&
             mouseState.LeftButton == ButtonState.Pressed))
        {
            _arrSliderRawValue = mousePosition.X - _arrSliderGuideBounds.X - _arrSliderTabBounds.Width / 2;

            if (_arrSliderRawValue < 0)
                _arrSliderRawValue = 0;
            else if (_arrSliderRawValue > _configSliderMaxValue)
                _arrSliderRawValue = _configSliderMaxValue;

            _arrSliderPercentage = _arrSliderRawValue / (float)_configSliderMaxValue;
            _game.Preferences.AutoRepeatRate = (int)(-Math.Round(_arrSliderPercentage * 300) + 300);
        }

        if (_dcdSliderTabPressed && !_dasSliderTabPressed && !_arrSliderTabPressed ||
            (_dcdSliderGuideBounds.Intersects(mousePosition) &&
             mouseState.LeftButton == ButtonState.Pressed))
        {
            _dcdSliderRawValue = mousePosition.X - _dcdSliderGuideBounds.X - _dcdSliderTabBounds.Width / 2;

            if (_dcdSliderRawValue < 0)
                _dcdSliderRawValue = 0;
            else if (_dcdSliderRawValue > _configSliderMaxValue)
                _dcdSliderRawValue = _configSliderMaxValue;

            _dcdSliderPercentage = _dcdSliderRawValue / (float)_configSliderMaxValue;
            _game.Preferences.DasCutDelay = (int)(-Math.Round(_dcdSliderPercentage * 300) + 300);
        }
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

        // Back Button
        if (_backButtonBounds.Intersects(mousePosition))
        {
            _backButtonHovered = true;

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                _backButtonPressed = true;
            }
        }
        else
        {
            _backButtonHovered = false;
        }

        if (mouseState.LeftButton == ButtonState.Released && _backButtonPressed)
        {
            _backButtonPressed = false;

            if (_backButtonHovered)
            {
                switch (_currentMenu)
                {
                    case Menus.MultiplayerMenu:
                        _currentMenu = Menus.MainMenu;
                        break;

                    case Menus.RankedMenu:
                        _currentMenu = Menus.MultiplayerMenu;
                        break;

                    case Menus.ConfigMenu:
                        _currentMenu = Menus.MainMenu;
                        break;
                }
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
        _spriteBatch.Draw(_sliderGuide, _dasSliderGuideBounds, Color.White);
        _spriteBatch.Draw(_sliderGuide, _arrSliderGuideBounds, Color.White);
        _spriteBatch.Draw(_sliderGuide, _dcdSliderGuideBounds, Color.White);

        if (_dasSliderTabHovered && !_dasSliderTabPressed)
            _spriteBatch.Draw(_sliderTabHover, _dasSliderTabBounds, Color.White);
        else if (_dasSliderTabPressed)
            _spriteBatch.Draw(_sliderTabHover, _dasSliderTabBounds, Color.White);
        else _spriteBatch.Draw(_sliderTab, _dasSliderTabBounds, Color.White);

        if (_arrSliderTabHovered && !_arrSliderTabPressed)
            _spriteBatch.Draw(_sliderTabHover, _arrSliderTabBounds, Color.White);
        else if (_arrSliderTabPressed)
            _spriteBatch.Draw(_sliderTabHover, _arrSliderTabBounds, Color.White);
        else _spriteBatch.Draw(_sliderTab, _arrSliderTabBounds, Color.White);

        if (_dcdSliderTabHovered && !_dcdSliderTabPressed)
            _spriteBatch.Draw(_sliderTabHover, _dcdSliderTabBounds, Color.White);
        else if (_dcdSliderTabPressed)
            _spriteBatch.Draw(_sliderTabHover, _dcdSliderTabBounds, Color.White);
        else _spriteBatch.Draw(_sliderTab, _dcdSliderTabBounds, Color.White);

        _spriteBatch.DrawString(_font, "DAS: " + _game.Preferences.DelayedAutoShift + " ms",
            new Vector2(_dasSliderGuideBounds.X, _dasSliderGuideBounds.Y - _dasSliderTabBounds.Height / 2f),
            Color.White, 0f, Vector2.Zero, _game.Scale / 2, SpriteEffects.None, 0f);

        _spriteBatch.DrawString(_font, "ARR: " + _game.Preferences.AutoRepeatRate + " ms",
            new Vector2(_arrSliderGuideBounds.X, _arrSliderGuideBounds.Y - _arrSliderTabBounds.Height / 2f),
            Color.White, 0f, Vector2.Zero, _game.Scale / 2, SpriteEffects.None, 0f);

        _spriteBatch.DrawString(_font, "DCD: " + _game.Preferences.DasCutDelay + " ms",
            new Vector2(_dcdSliderGuideBounds.X, _dcdSliderGuideBounds.Y - _dcdSliderTabBounds.Height / 2f),
            Color.White, 0f, Vector2.Zero, _game.Scale / 2, SpriteEffects.None, 0f);
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

        // Back button
        if (_currentMenu != Menus.MainMenu)
            _spriteBatch.Draw(_backButtonHovered ? _backButtonHover : _backButton, _backButtonBounds, Color.White);

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

            // sliders
            int sliderGuideHeight = _sliderGuide.Height;

            _dasSliderGuideBounds = new Rectangle(_lastWindowWidth / 2 - (int)(_sliderGuide.Width * _game.Scale) / 2,
                (int)(100 * _game.Scale),
                (int)(_sliderGuide.Width * _game.Scale), (int)(_sliderGuide.Height * _game.Scale));

            _arrSliderGuideBounds = new Rectangle(_lastWindowWidth / 2 - (int)(_sliderGuide.Width * _game.Scale) / 2,
                (int)((100 + sliderGuideHeight * 2) * _game.Scale),
                (int)(_sliderGuide.Width * _game.Scale), (int)(_sliderGuide.Height * _game.Scale));

            _dcdSliderGuideBounds = new Rectangle(_lastWindowWidth / 2 - (int)(_sliderGuide.Width * _game.Scale) / 2,
                (int)((100 + sliderGuideHeight * 4) * _game.Scale),
                (int)(_sliderGuide.Width * _game.Scale), (int)(_sliderGuide.Height * _game.Scale));

            // back button
            _backButtonBounds = new Rectangle(0, (int)(_backButton.Height * _game.Scale),
                (int)(_backButton.Width * (_game.Scale / 2)), (int)(_backButton.Height * (_game.Scale / 2)));
        }
    }
}