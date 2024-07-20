using NUnit.Framework;
using osu.Framework.Graphics;
using PolygonBazooka.Game.Elements;

namespace PolygonBazooka.Game.Tests.Visual;

[TestFixture]
public partial class TestSceneLocalPlayer : PolygonBazookaTestScene
{
    public TestSceneLocalPlayer()
    {
        Add(new LocalPlayer
        {
            Anchor = Anchor.Centre,
        });
    }
}
