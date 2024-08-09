using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;

namespace PolygonBazooka.Screens;

public class MainMenuScreen : GameScreen
{
    private readonly SpriteBatch _spriteBatch;

    private readonly PolygonBazookaGame _game;

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
    // TODO: implement these
    private readonly Texture2D _switchAccountButton;
    private readonly Texture2D _switchAccountButtonHover;
    private Rectangle _switchAccountButtonBounds;
    private bool _switchAccountButtonHovered;

    private readonly Texture2D _accountIndicator;
    private Rectangle _accountIndicatorBounds;

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

        _accountIndicator = Game.Content.Load<Texture2D>("Textures/ui/you_are_logged_in_as");
    }

    public override void Update(GameTime gameTime)
    {
        var mouseState = Mouse.GetState();

        // Singleplayer Button
        if (_singleplayerButtonBounds.Intersects(new Rectangle(mouseState.X, mouseState.Y, 0, 0)))
        {
            _singleplayerButtonHovered = true;

            if (mouseState.LeftButton == ButtonState.Pressed)
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
        if (_multiplayerButtonBounds.Intersects(new Rectangle(mouseState.X, mouseState.Y, 0, 0)))
        {
            _multiplayerButtonHovered = true;

            if (mouseState.LeftButton == ButtonState.Pressed)
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
                // multiplayer menu
            }
        }

        // Config Button
        if (_configButtonBounds.Intersects(new Rectangle(mouseState.X, mouseState.Y, 0, 0)))
        {
            _configButtonHovered = true;

            if (mouseState.LeftButton == ButtonState.Pressed)
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
                // TODO: make a config menu
            }
        }
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

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

        _spriteBatch.End();

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
        }
    }
}