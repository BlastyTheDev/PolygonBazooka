using osu.Framework.Platform;
using osu.Framework;
using PolygonBazooka.Game;

namespace PolygonBazooka.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost(@"PolygonBazooka"))
            using (osu.Framework.Game game = new PolygonBazookaGame())
                host.Run(game);
        }
    }
}
