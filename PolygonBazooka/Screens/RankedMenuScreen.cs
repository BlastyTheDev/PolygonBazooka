using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using PolygonBazooka.Networking;

namespace PolygonBazooka.Screens;

public class RankedMenuScreen : GameScreen
{
    private readonly PolygonBazookaGame _game;
    private readonly SpriteBatch _spriteBatch;
    
    private bool _inQueue;
    
    private readonly SpriteFont _font;
    
    public RankedMenuScreen(PolygonBazookaGame game) : base(game)
    {
        _game = game;
        _spriteBatch = new(GraphicsDevice);
        
        _font = game.Content.Load<SpriteFont>("Fonts/Tiny5");
        
        game.RankedSocket.SendAsync(RankedSocket.JoinQueue).Wait();
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
        GraphicsDevice.Clear(Color.Black);
        
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        
        // player is in queue when loading this screen
        _spriteBatch.DrawString(_font, "You are in the queue. (UNRANKED)", new(10, 10), Color.White, 0f,
            Vector2.Zero, _game.Scale, SpriteEffects.None, 0);
        
        _spriteBatch.End();
    }
}