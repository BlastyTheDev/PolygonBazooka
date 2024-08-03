using DiscordRPC;
using DiscordRPC.Message;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using PolygonBazooka.Game;

namespace PolygonBazooka.Desktop;

internal partial class DiscordRichPresence : Component
{
    private const string app_id = "1268898034746392587";

    private DiscordRpcClient client = null!;

    private readonly RichPresence presence = new()
    {
        State = "Playing Solo",
        Details = "Stacking blocks",
        Timestamps = Timestamps.Now,
        Assets = new Assets
        {
            LargeImageKey = "greentile",
            LargeImageText = "Polygon Bazooka"
        }
    };

    [BackgroundDependencyLoader]
    private void load()
    {
        client = new DiscordRpcClient(app_id)
        {
            SkipIdenticalPresence = true
        };

        client.OnReady += onReady;
        client.OnError += (_, e) => Logger.Log("Discord RPC Client error: " + e.Message, LoggingTarget.Network, LogLevel.Error);

        client.Initialize();
    }

    private void onReady(object sender, ReadyMessage args)
    {
        Logger.Log("Discord RPC Client ready", LoggingTarget.Network, LogLevel.Debug);

        if (client.CurrentPresence != null)
            client.SetPresence(null);

        Scheduler.AddDelayed(() =>
        {
            if (!client.IsInitialized)
                return;

            client.SetPresence(presence);
        }, 200);
    }

    public void ScheduleStatusUpdate(GameState state)
    {
        if (client == null)
            return;

        if (!client.IsInitialized)
            return;

        if (client.CurrentPresence == null)
            return;

        Scheduler.AddDelayed(() =>
        {
            switch (state)
            {
                case GameState.Playing:
                    client.UpdateState("Playing Solo");
                    client.UpdateDetails("Stacking blocks");
                    break;

                case GameState.Paused:
                    client.UpdateState("Paused");
                    client.UpdateDetails("Taking a break");
                    break;

                case GameState.GameOver:
                    client.UpdateState("Game Over");
                    client.UpdateDetails("Topped out");
                    break;
            }
        }, 200);
    }

    protected override void Dispose(bool isDisposing)
    {
        client.Dispose();
        base.Dispose(isDisposing);
    }
}
