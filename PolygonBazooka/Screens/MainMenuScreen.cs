using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;

namespace PolygonBazooka.Screens;

public class MainMenuScreen : GameScreen
{
    private readonly SpriteBatch _spriteBatch;

    private readonly PolygonBazookaGame _game;

    private readonly Texture2D _singleplayerButton;
    private readonly Texture2D _singleplayerButtonHover;
    private readonly Texture2D _singleplayerButtonPress;
    private Rectangle _singleplayerButtonBounds;
    private bool _singleplayerButtonHovered;
    private bool _singleplayerButtonPressed;

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

        _switchAccountButton = Game.Content.Load<Texture2D>("Textures/ui/switch_account_button");
        _switchAccountButtonHover = Game.Content.Load<Texture2D>("Textures/ui/switch_account_button_hover");
        
        _accountIndicator = Game.Content.Load<Texture2D>("Textures/ui/you_are_logged_in_as");
    }

    public override void Update(GameTime gameTime)
    {
        var mouseState = Mouse.GetState();

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
                _game.ChangeGameState(GameState.Playing);
            }
        }
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        
        if (_singleplayerButtonHovered && !_singleplayerButtonPressed)
            _spriteBatch.Draw(_singleplayerButtonHover, _singleplayerButtonBounds, Color.White);
        else if (_singleplayerButtonPressed)
            _spriteBatch.Draw(_singleplayerButtonPress, _singleplayerButtonBounds, Color.White);
        else _spriteBatch.Draw(_singleplayerButton, _singleplayerButtonBounds, Color.White);

        _spriteBatch.End();

        if (_lastWindowHeight != Game.Window.ClientBounds.Height || _lastWindowWidth != Game.Window.ClientBounds.Width)
        {
            _lastWindowWidth = Game.Window.ClientBounds.Width;
            _lastWindowHeight = Game.Window.ClientBounds.Height;

            _singleplayerButtonBounds = new Rectangle(
                _lastWindowWidth / 2 - (int)(_singleplayerButton.Width * _game.Scale) / 2,
                _lastWindowHeight / 2 - (int)(_singleplayerButton.Height * _game.Scale) / 2,
                (int)(_singleplayerButton.Width * _game.Scale), (int)(_singleplayerButton.Height * _game.Scale));
        }
    }
}