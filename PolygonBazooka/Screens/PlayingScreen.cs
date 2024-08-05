using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using PolygonBazooka.Elements;

namespace PolygonBazooka.Screens;

public class PlayingScreen : GameScreen
{
    private SpriteBatch _spriteBatch;
    private Player _localPlayer;

    private bool _leftPressed;
    private bool _rightPressed;
    private bool _downPressed;

    private bool _leftDasActive;
    private bool _rightDasActive;

    private long _leftPressStart;
    private long _rightPressStart;

    private long _lastLeftAutoRepeat;
    private long _lastRightAutoRepeat;
    private long _lastDownAutoRepeat;

    public PlayingScreen(Game game) : base(game)
    {
    }

    public override void LoadContent()
    {
        _spriteBatch = new(Game.GraphicsDevice);

        _localPlayer = new(Game)
        {
            RenderPosition = new(100, 100),
        };

        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        
        if (keyboardState.IsKeyDown(Keys.A) && !_leftPressed)
        {
            _leftPressed = true;
            _leftPressStart = gameTime.TotalGameTime.Ticks;
        }
        else if (keyboardState.IsKeyUp(Keys.A) && _leftPressed)
        {
            _leftPressed = false;
            _leftDasActive = false;
        }
        
        if (keyboardState.IsKeyDown(Keys.D) && !_rightPressed)
        {
            _rightPressed = true;
            _rightPressStart = gameTime.TotalGameTime.Ticks;
        }
        else if (keyboardState.IsKeyUp(Keys.D) && _rightPressed)
        {
            _rightPressed = false;
            _rightDasActive = false;
        }
    }

    public override void Draw(GameTime gameTime)
    {
        _localPlayer.Draw(gameTime);
    }
}