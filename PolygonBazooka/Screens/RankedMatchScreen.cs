using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using PolygonBazooka.Elements;
using PolygonBazooka.Util;

namespace PolygonBazooka.Screens;

public class RankedMatchScreen : GameScreen
{
    private readonly PolygonBazookaGame _game;
    private readonly SpriteBatch _spriteBatch;

    private readonly Player _enemyPlayer;
    private bool _enemyInitialized;

    private readonly TileType[,] _enemyBoard;
    private int _enemyBoardOffsetX;

    private readonly Texture2D _boards;
    private Rectangle _boardsBounds;

    private int _lastWindowWidth;
    private int _lastWindowHeight;

    public RankedMatchScreen(PolygonBazookaGame game) : base(game)
    {
        _game = game;
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _lastWindowWidth = Game.Window.ClientBounds.Width;
        _lastWindowHeight = Game.Window.ClientBounds.Height;

        _enemyPlayer = new Player(game, false, true)
        {
            // TODO: render correctly
            RenderPosition = new(242 * game.Scale, 0),
        };

        _enemyBoard = ResetBoard();

        _boards = Game.Content.Load<Texture2D>("Textures/board_vs");
    }

    private TileType[,] ResetBoard()
    {
        TileType[,] board = new TileType[Const.Rows, Const.Cols];

        for (int row = 0; row < Const.Rows; row++)
        {
            for (int col = 0; col < Const.Cols; col++)
            {
                board[row, col] = TileType.Empty;
            }
        }

        return board;
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _spriteBatch.Draw(_boards, _boardsBounds, Color.White);

        // render enemy tiles
        // for (int row = 0; row < Const.Rows; row++)
        // {
        //     for (int col = 0; col < Const.Cols; col++)
        //     {
        //         if (_enemyBoard[row, col] == TileType.Empty)
        //             continue;
        //
        //         int x = (int)(_boardsBounds.X + _boardsBounds.Width - (_enemyBoardOffsetX - col * 16 * _game.Scale));
        //         int y = (int)(_boardsBounds.Y + (row + 1) * (16 * _game.Scale));
        //
        //         _spriteBatch.Draw(_game.Textures.GetTile(_enemyBoard[row, col]),
        //             new Rectangle(x, y, (int)(16 * _game.Scale), (int)(16 * _game.Scale)), Color.White);
        //     }
        // }

        _enemyPlayer.Draw(gameTime);

        _spriteBatch.End();

        if (_lastWindowWidth != Game.Window.ClientBounds.Width || _lastWindowHeight != Game.Window.ClientBounds.Height)
        {
            _lastWindowWidth = Game.Window.ClientBounds.Width;
            _lastWindowHeight = Game.Window.ClientBounds.Height;

            _boardsBounds = new Rectangle((int)(_lastWindowWidth / 2f - _boards.Width * _game.Scale / 2),
                (int)(_lastWindowHeight / 2f - _boards.Height * _game.Scale / 2),
                (int)(_boards.Width * _game.Scale), (int)(_boards.Height * _game.Scale));

            _enemyBoardOffsetX = (int)(_game.Scale * 144);
        }
    }
}