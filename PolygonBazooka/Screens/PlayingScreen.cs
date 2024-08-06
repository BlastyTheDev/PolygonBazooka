using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using PolygonBazooka.Elements;

namespace PolygonBazooka.Screens;

public class PlayingScreen(Game game) : GameScreen(game)
{
    private SpriteBatch _spriteBatch;
    private Player _localPlayer;

    private bool _leftPressed;
    private bool _rightPressed;
    private bool _downPressed;
    private bool _hardDropPressed;

    private bool _leftDasActive;
    private bool _rightDasActive;

    private long _leftPressStart;
    private long _rightPressStart;

    private long _lastLeftAutoRepeat;
    private long _lastRightAutoRepeat;
    private long _lastDownAutoRepeat;

    public float DelayedAutoShift = 127;
    public float AutoRepeatRate = 50;

    public override void LoadContent()
    {
        _spriteBatch = new(Game.GraphicsDevice);

        _localPlayer = new(Game, true)
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
            _leftPressStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            _localPlayer.MoveLeft();
        }

        if (keyboardState.IsKeyUp(Keys.A) && _leftPressed)
        {
            _leftPressed = false;
            _leftDasActive = false;
        }

        if (keyboardState.IsKeyDown(Keys.D) && !_rightPressed)
        {
            _rightPressed = true;
            _rightPressStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            _localPlayer.MoveRight();
        }

        if (keyboardState.IsKeyUp(Keys.D) && _rightPressed)
        {
            _rightPressed = false;
            _rightDasActive = false;
        }

        if (!_leftPressed)
            _leftDasActive = false;

        if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _leftPressStart >= DelayedAutoShift && _leftPressed
            && !_leftDasActive)
            _leftDasActive = true;

        if (_leftDasActive && _leftPressed
                           && DateTimeOffset.Now.ToUnixTimeMilliseconds() - _leftPressStart >= AutoRepeatRate)
        {
            if (AutoRepeatRate == 0)
                _localPlayer.MoveLeftFully();
            else
                _localPlayer.MoveLeft();

            _lastLeftAutoRepeat = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        if (!_rightPressed)
            _rightDasActive = false;

        if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _rightPressStart >= DelayedAutoShift && _rightPressed
            && !_rightDasActive)
            _rightDasActive = true;

        if (_rightDasActive && _rightPressed
                            && DateTimeOffset.Now.ToUnixTimeMilliseconds() - _rightPressStart >= AutoRepeatRate)
        {
            if (AutoRepeatRate == 0)
                _localPlayer.MoveRightFully();
            else
                _localPlayer.MoveRight();

            _lastRightAutoRepeat = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        if (keyboardState.IsKeyDown(Keys.Space) && !_hardDropPressed)
        {
            _hardDropPressed = true;
            _localPlayer.HardDrop();
        }

        if (keyboardState.IsKeyUp(Keys.Space) && _hardDropPressed)
        {
            _hardDropPressed = false;
        }

        _localPlayer.ProcessGravity();
        _localPlayer.Clear();
    }

    public override void Draw(GameTime gameTime)
    {
        _localPlayer.Draw(gameTime);
    }
}