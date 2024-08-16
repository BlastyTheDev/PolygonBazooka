using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using PolygonBazooka.Elements;

namespace PolygonBazooka.Util;

public class Textures
{
    public readonly Texture2D BlueTile;
    public readonly Texture2D GreenTile;
    public readonly Texture2D RedTile;
    public readonly Texture2D YellowTile;
    public readonly Texture2D BonusTile;
    public readonly Texture2D GarbageTile;

    public readonly Texture2D BlueShadow;
    public readonly Texture2D GreenShadow;
    public readonly Texture2D RedShadow;
    public readonly Texture2D YellowShadow;
    public readonly Texture2D BonusShadow;

    public readonly Texture2DAtlas ClearAnimationAtlas;

    public Textures(PolygonBazookaGame game)
    {
        BlueTile = game.Content.Load<Texture2D>("Textures/blue");
        GreenTile = game.Content.Load<Texture2D>("Textures/green");
        RedTile = game.Content.Load<Texture2D>("Textures/red");
        YellowTile = game.Content.Load<Texture2D>("Textures/yellow");
        BonusTile = game.Content.Load<Texture2D>("Textures/bonus");
        GarbageTile = game.Content.Load<Texture2D>("Textures/garbage");

        BlueShadow = game.Content.Load<Texture2D>("Textures/blue_shadow");
        GreenShadow = game.Content.Load<Texture2D>("Textures/green_shadow");
        RedShadow = game.Content.Load<Texture2D>("Textures/red_shadow");
        YellowShadow = game.Content.Load<Texture2D>("Textures/yellow_shadow");
        BonusShadow = game.Content.Load<Texture2D>("Textures/bonus_shadow");

        Texture2D clearSpriteSheet = game.Content.Load<Texture2D>("Textures/clear_sprite_sheet");
        ClearAnimationAtlas = new(clearSpriteSheet);
        ClearAnimationAtlas.CreateRegion(48 * 0, 0, 48, 48);
        ClearAnimationAtlas.CreateRegion(48 * 1, 0, 48, 48);
        ClearAnimationAtlas.CreateRegion(48 * 2, 0, 48, 48);
        ClearAnimationAtlas.CreateRegion(48 * 3, 0, 48, 48);
        ClearAnimationAtlas.CreateRegion(48 * 4, 0, 48, 48);
        ClearAnimationAtlas.CreateRegion(48 * 5, 0, 48, 48);
        ClearAnimationAtlas.CreateRegion(48 * 6, 0, 48, 48);
        ClearAnimationAtlas.CreateRegion(48 * 7, 0, 48, 48);
    }

    public Texture2D GetTile(TileType type)
    {
        return type switch
        {
            TileType.Blue => BlueTile,
            TileType.Green => GreenTile,
            TileType.Red => RedTile,
            TileType.Yellow => YellowTile,
            TileType.Bonus => BonusTile,
            TileType.Garbage => GarbageTile,
            _ => null
        };
    }

    public Texture2D GetShadow(TileType type)
    {
        return type switch
        {
            TileType.Blue => BlueShadow,
            TileType.Green => GreenShadow,
            TileType.Red => RedShadow,
            TileType.Yellow => YellowShadow,
            TileType.Bonus => BonusShadow,
            _ => null
        };
    }
}