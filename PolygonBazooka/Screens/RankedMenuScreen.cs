using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace PolygonBazooka.Screens;

public class RankedMenuScreen : GameScreen
{
    private readonly PolygonBazookaGame _game;
    
    private bool _inQueue;
    
    public RankedMenuScreen(PolygonBazookaGame game) : base(game)
    {
        _game = game;
        
        game.RankedSocket.JoinQueueAsync().Wait();
        _inQueue = true;
    }

    public override void Update(GameTime gameTime)
    {
        Task.Run(ListenForMatch);
    }
    
    private async void ListenForMatch()
    {
        var response = await _game.RankedSocket.ReceiveAsync();
        
        Console.WriteLine(response);
    }

    public override void Draw(GameTime gameTime)
    {
    }
}