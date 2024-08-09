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
        State = "Existing",
        Details = "In the Menu",
        // Timestamps = Timestamps.Now,
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

        // TODO: changing start time isnt working properly
        switch (state)
        {
            case GameState.MainMenu:
                _client.UpdateDetails("In the Menu");
                _client.UpdateState("Existing");
                _client.UpdateClearTime();
                break;

            case GameState.SoloPlaying:
                _client.UpdateDetails("Stacking blocks");
                _client.UpdateState("Playing Solo");
                _client.UpdateClearTime();
                _client.UpdateStartTime(DateTime.Now);
                break;

            case GameState.SoloGameOver:
                _client.UpdateDetails("Topped out");
                _client.UpdateState("Game Over");
                _client.UpdateClearTime();
                break;

            case GameState.RankedQueuing:
                _client.UpdateDetails("Playing Ranked");
                _client.UpdateState("In Queue");
                _client.UpdateClearTime();
                _client.UpdateStartTime(DateTime.Now);
                break;

            // TODO: set statuses for ranked
            case GameState.RankedGameStart:
                break;

            case GameState.RankedPlaying:
                break;

            case GameState.RankedGameOver:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}