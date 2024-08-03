using PolygonBazooka.Game;

namespace PolygonBazooka.Desktop;

public partial class PolygonBazookaGameDesktop : PolygonBazookaGame
{
    private DiscordRichPresence discordRpc = null!;

    protected override void LoadComplete()
    {
        base.LoadComplete();

        LoadComponentAsync(discordRpc = new DiscordRichPresence(), Add);
    }

    private GameState previousState;

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        if (State != previousState)
        {
            discordRpc.ScheduleStatusUpdate(State);
            previousState = State;
        }
    }
}
