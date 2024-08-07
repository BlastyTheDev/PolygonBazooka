using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using PolygonBazooka.Elements;

// ReSharper disable PossibleLossOfFraction

namespace PolygonBazooka.Screens;

public class PlayingScreen(PolygonBazookaGame game) : GameScreen(game)
{
    private SpriteBatch _spriteBatch;
    private Player _localPlayer;

    private bool _leftPressed;
    private bool _rightPressed;
    private bool _cwRotatePressed;
    private bool _ccwRotatePressed;
    private bool _flipPressed;
    private bool _hardDropPressed;

    private bool _retryPressed;
    private long _retryPressStart;

    private bool _leftDasActive;
    private bool _rightDasActive;

    private long _leftPressStart;
    private long _rightPressStart;

    private long _lastLeftAutoRepeat;
    private long _lastRightAutoRepeat;
    private long _lastDownAutoRepeat;

    private long _lastFallingBlockGravityTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();

    public float DelayedAutoShift = 127;
    public float AutoRepeatRate = 0;
    public float SoftDropRate = 100;

    // render player at the centre of screen and adjust size based on it
    private int _lastWindowWidth;
    private int _lastWindowHeight;

    public override void LoadContent()
    {
        _spriteBatch = new(Game.GraphicsDevice);

        _localPlayer = new(game, true)
        {
            RenderPosition = new(_lastWindowWidth / 2 - 78 * game.Scale, _lastWindowHeight / 2 - 78 * game.Scale),
        };

        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();

        if (_localPlayer.Failed)
        {
            game.ChangeGameState(GameState.SoloGameOver);

            if (keyboardState.IsKeyDown(Keys.R))
            {
                if (!_retryPressed)
                    _retryPressStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                _retryPressed = true;

                if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _retryPressStart >= 1000)
                {
                    _localPlayer = new(game, true)
                    {
                        RenderPosition = new(_lastWindowWidth / 2 - 78 * game.Scale, _lastWindowHeight / 2 - 78 * game.Scale),
                    };
                }
            }

            if (keyboardState.IsKeyUp(Keys.R) && _retryPressed)
                _retryPressed = false;
        }
        else
        {
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

            // DAS Start ----------------------------------------------------------
            if (!_leftPressed)
                _leftDasActive = false;

            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _leftPressStart >= DelayedAutoShift && _leftPressed
                && !_leftDasActive)
                _leftDasActive = true;

            if (_leftDasActive && _leftPressed
                               && DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastLeftAutoRepeat >= AutoRepeatRate)
            {
                _lastLeftAutoRepeat = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                if (AutoRepeatRate == 0)
                    _localPlayer.MoveLeftFully();
                else
                    _localPlayer.MoveLeft();
            }

            if (!_rightPressed)
                _rightDasActive = false;

            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _rightPressStart >= DelayedAutoShift && _rightPressed
                && !_rightDasActive)
                _rightDasActive = true;

            if (_rightDasActive && _rightPressed
                                && DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastRightAutoRepeat >= AutoRepeatRate)
            {
                _lastRightAutoRepeat = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                if (AutoRepeatRate == 0)
                    _localPlayer.MoveRightFully();
                else
                    _localPlayer.MoveRight();
            }
            // DAS End ----------------------------------------------------------

            if (keyboardState.IsKeyDown(Keys.S))
            {
                if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastDownAutoRepeat >= SoftDropRate)
                {
                    _localPlayer.MoveDown();
                    _lastDownAutoRepeat = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                }
            }

            if (keyboardState.IsKeyDown(Keys.Left) && !_ccwRotatePressed)
            {
                _ccwRotatePressed = true;
                _localPlayer.RotateCcw();
            }

            if (keyboardState.IsKeyUp(Keys.Left) && _ccwRotatePressed)
                _ccwRotatePressed = false;

            if (keyboardState.IsKeyDown(Keys.Right) && !_cwRotatePressed)
            {
                _cwRotatePressed = true;
                _localPlayer.RotateCw();
            }

            if (keyboardState.IsKeyUp(Keys.Right) && _cwRotatePressed)
                _cwRotatePressed = false;

            if (keyboardState.IsKeyDown(Keys.Up) && !_flipPressed)
            {
                _flipPressed = true;
                _localPlayer.Flip();
            }

            if (keyboardState.IsKeyUp(Keys.Up) && _flipPressed)
                _flipPressed = false;

            if (keyboardState.IsKeyDown(Keys.Space) && !_hardDropPressed)
            {
                _hardDropPressed = true;
                _localPlayer.HardDrop();
            }

            if (keyboardState.IsKeyUp(Keys.Space) && _hardDropPressed)
                _hardDropPressed = false;

            // Falling Block Gravity
            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastFallingBlockGravityTick >= 1000)
            {
                _lastFallingBlockGravityTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                _localPlayer.MoveDown();
            }

            if (!_localPlayer.IsClearing())
                _localPlayer.ProcessGravity();

            _localPlayer.Clear();
        }

        if (_lastWindowHeight != Game.Window.ClientBounds.Height || _lastWindowWidth != Game.Window.ClientBounds.Width)
        {
            _lastWindowWidth = Game.Window.ClientBounds.Width;
            _lastWindowHeight = Game.Window.ClientBounds.Height;

            _localPlayer.RenderPosition = new(_lastWindowWidth / 2 - 78 * game.Scale,
                _lastWindowHeight / 2 - 78 * game.Scale);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        _localPlayer.Draw(gameTime);

        if (_localPlayer.Failed)
        {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(Game.Content.Load<SpriteFont>("Fonts/GameOver"), "Game Over. Hold R to retry",
                new(50, 50), Color.Red);
            _spriteBatch.End();
        }
    }
}