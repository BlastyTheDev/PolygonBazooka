using NUnit.Framework;
using PolygonBazooka.Game.Elements;

namespace PolygonBazooka.Game.Tests.Visual;

[TestFixture]
public partial class TestSceneLocalPlayer : PolygonBazookaTestScene
{
    public TestSceneLocalPlayer()
    {
        Add(LocalPlayer.INSTANCE);
    }
}
