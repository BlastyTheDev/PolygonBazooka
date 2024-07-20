using osu.Framework.Testing;

namespace PolygonBazooka.Game.Tests.Visual
{
    public abstract partial class PolygonBazookaTestScene : TestScene
    {
        protected override ITestSceneTestRunner CreateRunner() => new PolygonBazookaTestSceneTestRunner();

        private partial class PolygonBazookaTestSceneTestRunner : PolygonBazookaGameBase, ITestSceneTestRunner
        {
            private TestSceneTestRunner.TestRunner runner;

            protected override void LoadAsyncComplete()
            {
                base.LoadAsyncComplete();
                Add(runner = new TestSceneTestRunner.TestRunner());
            }

            public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
        }
    }
}
