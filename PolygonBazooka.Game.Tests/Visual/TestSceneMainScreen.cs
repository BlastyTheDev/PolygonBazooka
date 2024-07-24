using osu.Framework.Graphics;
using osu.Framework.Screens;
using NUnit.Framework;
using PolygonBazooka.Game.Elements;

namespace PolygonBazooka.Game.Tests.Visual
{
    [TestFixture]
    public partial class TestSceneMainScreen : PolygonBazookaTestScene
    {
        // Add visual tests to ensure correct behaviour of your game: https://github.com/ppy/osu-framework/wiki/Development-and-Testing
        // You can make changes to classes associated with the tests and they will recompile and update immediately.

        public TestSceneMainScreen()
        {
            Add(new ScreenStack(new MainScreen(new Player
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.TopLeft,
                Position = new(100, 200)
            })) { RelativeSizeAxes = Axes.Both });
        }
    }
}
