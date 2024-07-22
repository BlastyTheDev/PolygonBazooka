using osu.Framework.Graphics;
using osuTK;
using PolygonBazooka.Game.Elements;

namespace PolygonBazooka.Game.Tests.Visual;

public partial class TestScenePlayer : PolygonBazookaTestScene
{
    public TestScenePlayer()
    {
        Add(new Player
        {
            Origin = Anchor.Centre,
            Anchor = Anchor.TopLeft,
            Position = new Vector2(100, 200),
        });
    }
}
