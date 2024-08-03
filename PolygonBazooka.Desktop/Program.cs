using osu.Framework.Platform;
using osu.Framework;

namespace PolygonBazooka.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost(@"PolygonBazooka"))
            using (osu.Framework.Game game = new PolygonBazookaGameDesktop())
                host.Run(game);
        }
    }
}
