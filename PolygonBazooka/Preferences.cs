using Microsoft.Xna.Framework.Input;

namespace PolygonBazooka;

public class Preferences
{
    public float DelayedAutoShift { get; set; } = 127;
    public float AutoRepeatRate { get; set; } = 0;
    public float SoftDropRate { get; set; } = 100;

    public Keys LeftKey { get; set; } = Keys.A;
    public Keys RightKey { get; set; } = Keys.D;

    public Keys CwRotateKey { get; set; } = Keys.Right;
    public Keys CcwRotateKey { get; set; } = Keys.Left;
    public Keys FlipKey { get; set; } = Keys.Up;

    public Keys HardDropKey { get; set; } = Keys.Space;
    public Keys SoftDropKey { get; set; } = Keys.S;

    public Keys RetryKey { get; set; } = Keys.R;
    public Keys ForfeitKey { get; set; } = Keys.F;
    public Keys PauseKey { get; set; } = Keys.Escape;

    public Preferences()
    {
    }
    
    private void Load()
    {
    }
    
    public void Save()
    {
    }
}