using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using PolygonBazooka.Elements;
using PolygonBazooka.Networking;

namespace PolygonBazooka.Screens;

public class RankedMatchScreen : GameScreen
{
    private readonly PolygonBazookaGame _game;
    private readonly SpriteBatch _spriteBatch;

    private readonly RankedSocket _socket;

    private readonly Player _localPlayer;
    private readonly Player _enemyPlayer;

    private readonly Texture2D _boards;
    private Rectangle _boardsBounds;

    private int _lastWindowWidth;
    private int _lastWindowHeight;

    private bool _leftPressed;
    private bool _rightPressed;
    private bool _cwRotatePressed;
    private bool _ccwRotatePressed;
    private bool _flipPressed;
    private bool _hardDropPressed;

    private bool _leftDasActive;
    private bool _rightDasActive;

    private long _leftPressStart;
    private long _rightPressStart;

    private long _lastLeftAutoRepeat;
    private long _lastRightAutoRepeat;
    private long _lastDownAutoRepeat;

    private long _lastFallingBlockGravityTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();

    public RankedMatchScreen(PolygonBazookaGame game) : base(game)
    {
        _game = game;
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _socket = _game.RankedSocket;

        _lastWindowWidth = Game.Window.ClientBounds.Width;
        _lastWindowHeight = Game.Window.ClientBounds.Height;

        _boards = Game.Content.Load<Texture2D>("Textures/board_vs");

        _boardsBounds = new Rectangle((int)(_lastWindowWidth / 2f - _boards.Width * _game.Scale / 2),
            (int)(_lastWindowHeight / 2f - _boards.Height * _game.Scale / 2 + 12 * _game.Scale),
            (int)(_boards.Width * _game.Scale), (int)(_boards.Height * _game.Scale));

        _localPlayer = new Player(game, false, false, _spriteBatch)
        {
            RenderPosition = new(_boardsBounds.X + 28 * _game.Scale, _boardsBounds.Y + 12 * _game.Scale),
        };

        _enemyPlayer = new Player(game, false, true, _spriteBatch)
        {
            RenderPosition = new(_boardsBounds.X + (_boardsBounds.Width - 148 * _game.Scale),
                _boardsBounds.Y + 12 * game.Scale),
        };
    }

    public override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();

        if (_localPlayer.Failed)
        {
            // TODO: implement fail logic
        }
        else
        {
            if (keyboardState.IsKeyDown(_game.Preferences.LeftKey) && !_leftPressed)
            {
                _leftPressed = true;
                _leftPressStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                _localPlayer.MoveLeft();

                Task.Run(() => _socket.SendAsync(RankedSocket.MoveLeft));
            }

            if (keyboardState.IsKeyUp(_game.Preferences.LeftKey) && _leftPressed)
            {
                _leftPressed = false;
                _leftDasActive = false;
            }

            if (keyboardState.IsKeyDown(_game.Preferences.RightKey) && !_rightPressed)
            {
                _rightPressed = true;
                _rightPressStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                _localPlayer.MoveRight();
                
                Task.Run(() => _socket.SendAsync(RankedSocket.MoveRight));
            }

            if (keyboardState.IsKeyUp(_game.Preferences.RightKey) && _rightPressed)
            {
                _rightPressed = false;
                _rightDasActive = false;
            }

            // DAS Start ----------------------------------------------------------
            if (!_leftPressed)
                _leftDasActive = false;

            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _leftPressStart >= _game.Preferences.DelayedAutoShift &&
                _leftPressed
                && !_leftDasActive)
                _leftDasActive = true;

            if (_leftDasActive && _leftPressed
                               && DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastLeftAutoRepeat >=
                               _game.Preferences.AutoRepeatRate)
            {
                _lastLeftAutoRepeat = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                if (_game.Preferences.AutoRepeatRate == 0)
                {
                    _localPlayer.MoveLeftFully();
                    
                    Task.Run(() => _socket.SendAsync(RankedSocket.MoveLeftFully));
                }
                else
                {
                    _localPlayer.MoveLeft();
                    
                    Task.Run(() => _socket.SendAsync(RankedSocket.MoveLeft));
                }
            }

            if (!_rightPressed)
                _rightDasActive = false;

            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _rightPressStart >= _game.Preferences.DelayedAutoShift &&
                _rightPressed
                && !_rightDasActive)
                _rightDasActive = true;

            if (_rightDasActive && _rightPressed
                                && DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastRightAutoRepeat >=
                                _game.Preferences.AutoRepeatRate)
            {
                _lastRightAutoRepeat = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                if (_game.Preferences.AutoRepeatRate == 0)
                {
                    _localPlayer.MoveRightFully();
                    
                    Task.Run(() => _socket.SendAsync(RankedSocket.MoveRightFully));
                }
                else
                {
                    _localPlayer.MoveRight();
                    
                    Task.Run(() => _socket.SendAsync(RankedSocket.MoveRight));
                }
            }
            // DAS End ----------------------------------------------------------

            if (keyboardState.IsKeyDown(_game.Preferences.CcwRotateKey) && !_ccwRotatePressed)
            {
                _ccwRotatePressed = true;
                _localPlayer.RotateCcw();
                
                Task.Run(() => _socket.SendAsync(RankedSocket.CcwRotate));
            }

            if (keyboardState.IsKeyUp(_game.Preferences.CcwRotateKey) && _ccwRotatePressed)
                _ccwRotatePressed = false;

            if (keyboardState.IsKeyDown(_game.Preferences.CwRotateKey) && !_cwRotatePressed)
            {
                _cwRotatePressed = true;
                _localPlayer.RotateCw();
                
                Task.Run(() => _socket.SendAsync(RankedSocket.CwRotate));
            }

            if (keyboardState.IsKeyUp(_game.Preferences.CwRotateKey) && _cwRotatePressed)
                _cwRotatePressed = false;

            if (keyboardState.IsKeyDown(_game.Preferences.FlipKey) && !_flipPressed)
            {
                _flipPressed = true;
                _localPlayer.Flip();
                
                Task.Run(() => _socket.SendAsync(RankedSocket.Flip));
            }

            if (keyboardState.IsKeyUp(_game.Preferences.FlipKey) && _flipPressed)
                _flipPressed = false;

            if (keyboardState.IsKeyDown(_game.Preferences.HardDropKey) && !_hardDropPressed)
            {
                _hardDropPressed = true;
                _localPlayer.HardDrop();
                
                Task.Run(() => _socket.SendAsync(RankedSocket.HardDrop));
            }

            if (keyboardState.IsKeyUp(_game.Preferences.HardDropKey) && _hardDropPressed)
                _hardDropPressed = false;

            // Falling Block Gravity
            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastFallingBlockGravityTick >= 1000)
            {
                _lastFallingBlockGravityTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                _localPlayer.MoveDown();
                
                Task.Run(() => _socket.SendAsync(RankedSocket.MoveDown));
            }

            if (!_localPlayer.IsClearing())
                _localPlayer.ProcessGravity();

            _localPlayer.Clear();
        }

        if (_lastWindowHeight != Game.Window.ClientBounds.Height || _lastWindowWidth != Game.Window.ClientBounds.Width)
        {
            _lastWindowWidth = Game.Window.ClientBounds.Width;
            _lastWindowHeight = Game.Window.ClientBounds.Height;

            _localPlayer.RenderPosition = new(_lastWindowWidth / 2f - 78 * _game.Scale,
                _lastWindowHeight / 2f - 78 * _game.Scale);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _spriteBatch.Draw(_boards, _boardsBounds, Color.White);

        _localPlayer.Draw(gameTime);
        _enemyPlayer.Draw(gameTime);

        _spriteBatch.End();

        if (_lastWindowWidth != Game.Window.ClientBounds.Width || _lastWindowHeight != Game.Window.ClientBounds.Height)
        {
            _lastWindowWidth = Game.Window.ClientBounds.Width;
            _lastWindowHeight = Game.Window.ClientBounds.Height;

            _boardsBounds = new Rectangle((int)(_lastWindowWidth / 2f - _boards.Width * _game.Scale / 2),
                (int)(_lastWindowHeight / 2f - _boards.Height * _game.Scale / 2 + 12 * _game.Scale),
                (int)(_boards.Width * _game.Scale), (int)(_boards.Height * _game.Scale));

            _localPlayer.RenderPosition = new(_boardsBounds.X + 28 * _game.Scale, _boardsBounds.Y + 12 * _game.Scale);

            _enemyPlayer.RenderPosition = new(_boardsBounds.X + (_boardsBounds.Width - 148 * _game.Scale),
                _boardsBounds.Y + 12 * _game.Scale);
        }
    }
}