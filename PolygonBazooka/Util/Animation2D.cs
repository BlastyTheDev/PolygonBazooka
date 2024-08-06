using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PolygonBazooka.Util;

/// <summary>
/// Deprecated. Will be removed soon
/// </summary>
public class Animation2D : IDisposable
{
    private readonly List<Texture2D> _frames = new();
    private double _elapsedTime;
    private bool _disposed;

    public int CurrentFrameIndex { get; set; }
    public int FramesPerSecond { get; set; } = 4; // unsure
    public Texture2D CurrentFrame => _frames[CurrentFrameIndex];

    public void AddFrame(Texture2D frame)
    {
        _frames.Add(frame);
    }

    public void Update(GameTime gameTime)
    {
        if (_disposed)
            return;

        if (_frames.Count == 0)
            return;

        _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

        if (_elapsedTime >= 1.0 / FramesPerSecond)
        {
            CurrentFrameIndex = (CurrentFrameIndex + 1) % _frames.Count;
            _elapsedTime = 0;
        }

        // Console.WriteLine("cft" + CurrentFrameIndex);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        foreach (var frame in _frames)
            frame.Dispose();

        _frames.Clear();
        _elapsedTime = 0;
        _disposed = true;
    }
}