using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace PolygonBazooka.Game.Elements;

/// <summary>
/// The board of the local player.
/// </summary>
public partial class LocalPlayer : CompositeDrawable
{
    /// <summary>
    /// 12x7 grid of tiles.
    /// [row, col]
    /// </summary>
    private readonly TileType[,] tiles = new TileType[Const.ROWS, Const.COLS];

    private static LocalPlayer instance;

    public static LocalPlayer GetInstance()
    {
        return instance ??= new LocalPlayer();
    }

    public LocalPlayer()
    {
        AutoSizeAxes = Axes.Both;
        Origin = Anchor.Centre;
        resetBoard();
    }

    public void SetTile(int row, int col, TileType type)
    {
        tiles[row, col] = type;
    }

    public int GetLowestEmptyYInCol(int col)
    {
        for (int row = 0; row < Const.ROWS - 1; row++)
        {
            if (tiles[row, col] == TileType.Empty)
                return row;
        }

        return Const.ROWS - 1;
    }

    private void processGravity()
    {
        // start at second to bottom row
        for (int row = Const.ROWS - 2; row >= 0; row--)
        {
            for (int col = 0; col < Const.COLS - 1; col++)
            {
                int currentRow = row;

                // while the row being checked is above the bottom row and the tile below is empty
                while (currentRow < Const.ROWS - 1 && tiles[currentRow + 1, col] == TileType.Empty)
                {
                    // move the tile down
                    tiles[currentRow + 1, col] = tiles[currentRow, col];
                    tiles[currentRow, col] = TileType.Empty;
                    // check the row below
                    currentRow++;
                }
            }
        }
    }

    private void resetBoard()
    {
        for (int row = 0; row < 12; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                tiles[row, col] = TileType.Empty;
            }
        }
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        Texture texture = textures.Get("board");
        texture.ScaleAdjust = Const.SCALE_ADJUST;
        InternalChild = new Container
        {
            AutoSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Children = new Drawable[]
            {
                new Sprite
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Texture = texture,
                }
            }
        };
    }
}
