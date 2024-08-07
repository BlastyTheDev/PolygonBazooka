using System;
using DiscordRPC;
using DiscordRPC.Message;

namespace PolygonBazooka;

public class DiscordRichPresence
{
    private const string AppId = "1268898034746392587";

    private readonly DiscordRpcClient _client;

    private readonly RichPresence _presence = new()
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

    public DiscordRichPresence()
    {
        _client = new DiscordRpcClient(AppId)
        {
            SkipIdenticalPresence = true
        };

        _client.OnReady += OnReady;

        _client.Initialize();
    }

    private void OnReady(object sender, ReadyMessage args)
    {
        if (!_client.IsInitialized)
            return;

        _client.SetPresence(_presence);
    }

    public void UpdateStatus(GameState state)
    {
        if (_client == null)
            return;

        if (!_client.IsInitialized)
            return;

        if (_client.CurrentPresence == null)
            return;

        switch (state)
        {
            case GameState.MainMenu:
                _client.UpdateDetails("In the Menu");
                _client.UpdateState("Existing");
                break;

            case GameState.Playing:
                _client.UpdateDetails("Stacking blocks");
                _client.UpdateState("Playing Solo");
                break;

            case GameState.SoloGameOver:
                _client.UpdateDetails("Topped out");
                _client.UpdateState("Game Over");
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}