using System;
using System.IO;
using Microsoft.Xna.Framework.Input;

namespace PolygonBazooka;

public enum Keybinds
{
    LeftKey,
    RightKey,
    CwRotateKey,
    CcwRotateKey,
    FlipKey,
    HardDropKey,
    SoftDropKey,
    RetryKey,
    ForfeitKey,
    PauseKey,
}

public class Preferences
{
    public int DelayedAutoShift { get; set; } = 150;
    public int AutoRepeatRate { get; set; } = 50;
    public int DasCutDelay { get; set; } = 0;
    public int SoftDropRate { get; set; } = 100; // may be removed in the future

    public Keys LeftKey { get; set; } = Keys.A;
    public Keys RightKey { get; set; } = Keys.D;

    public Keys CwRotateKey { get; set; } = Keys.Right;
    public Keys CcwRotateKey { get; set; } = Keys.Left;
    public Keys FlipKey { get; set; } = Keys.Up;

    public Keys HardDropKey { get; set; } = Keys.Space;
    public Keys SoftDropKey { get; set; } = Keys.S; // may be removed in the future

    public Keys RetryKey { get; set; } = Keys.R;
    public Keys ForfeitKey { get; set; } = Keys.F;
    public Keys PauseKey { get; set; } = Keys.Escape;

    public string GetPreference(Keybinds keybind)
    {
        return keybind switch
        {
            Keybinds.LeftKey => LeftKey.ToString(),
            Keybinds.RightKey => RightKey.ToString(),
            Keybinds.CwRotateKey => CwRotateKey.ToString(),
            Keybinds.CcwRotateKey => CcwRotateKey.ToString(),
            Keybinds.FlipKey => FlipKey.ToString(),
            Keybinds.HardDropKey => HardDropKey.ToString(),
            Keybinds.SoftDropKey => SoftDropKey.ToString(),
            Keybinds.RetryKey => RetryKey.ToString(),
            Keybinds.ForfeitKey => ForfeitKey.ToString(),
            Keybinds.PauseKey => PauseKey.ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(keybind), keybind, null)
        };
    }

    public void SetPreference(Keybinds? keybind, Keys key)
    {
        if (keybind == null)
            return;

        // assign a variable to have a cleaner, less clunky switch
        var x = keybind switch
        {
            Keybinds.LeftKey => LeftKey = key,
            Keybinds.RightKey => RightKey = key,
            Keybinds.CwRotateKey => CwRotateKey = key,
            Keybinds.CcwRotateKey => CcwRotateKey = key,
            Keybinds.FlipKey => FlipKey = key,
            Keybinds.HardDropKey => HardDropKey = key,
            Keybinds.SoftDropKey => SoftDropKey = key,
            Keybinds.RetryKey => RetryKey = key,
            Keybinds.ForfeitKey => ForfeitKey = key,
            Keybinds.PauseKey => PauseKey = key,
            _ => throw new ArgumentOutOfRangeException(nameof(keybind), keybind, null)
        };
    }

    public Preferences()
    {
        Load();
    }

    private void Load()
    {
        if (File.Exists("preferences.txt"))
        {
            string[] preferences = File.ReadAllLines("preferences.txt");

            for (int line = 0; line < preferences.Length; line++)
                preferences[line] = preferences[line].Split('=')[1];

            DelayedAutoShift = int.Parse(preferences[0]);
            AutoRepeatRate = int.Parse(preferences[1]);
            DasCutDelay = int.Parse(preferences[2]);
            SoftDropRate = int.Parse(preferences[3]);

            LeftKey = (Keys)Enum.Parse(typeof(Keys), preferences[4]);
            RightKey = (Keys)Enum.Parse(typeof(Keys), preferences[5]);

            CwRotateKey = (Keys)Enum.Parse(typeof(Keys), preferences[6]);
            CcwRotateKey = (Keys)Enum.Parse(typeof(Keys), preferences[7]);
            FlipKey = (Keys)Enum.Parse(typeof(Keys), preferences[8]);

            HardDropKey = (Keys)Enum.Parse(typeof(Keys), preferences[9]);
            SoftDropKey = (Keys)Enum.Parse(typeof(Keys), preferences[10]);

            RetryKey = (Keys)Enum.Parse(typeof(Keys), preferences[11]);
            ForfeitKey = (Keys)Enum.Parse(typeof(Keys), preferences[12]);
            PauseKey = (Keys)Enum.Parse(typeof(Keys), preferences[13]);
        }
    }

    public void Save()
    {
        File.WriteAllLines("preferences.txt", [
            $"DelayedAutoShift={DelayedAutoShift}",
            $"AutoRepeatRate={AutoRepeatRate}",
            $"DasCutDelay={DasCutDelay}",
            $"SoftDropRate={SoftDropRate}",
            $"LeftKey={LeftKey}",
            $"RightKey={RightKey}",
            $"CwRotateKey={CwRotateKey}",
            $"CcwRotateKey={CcwRotateKey}",
            $"FlipKey={FlipKey}",
            $"HardDropKey={HardDropKey}",
            $"SoftDropKey={SoftDropKey}",
            $"RetryKey={RetryKey}",
            $"ForfeitKey={ForfeitKey}",
            $"PauseKey={PauseKey}"
        ]);
    }

    public static string GetKeybindName(Keybinds keybind)
    {
        return keybind switch
        {
            Keybinds.LeftKey => "Move Falling Piece Left",
            Keybinds.RightKey => "Move Falling Piece Right",
            Keybinds.CwRotateKey => "Rotate Falling Piece Clockwise",
            Keybinds.CcwRotateKey => "Rotate Falling Piece Counter Clockwise",
            Keybinds.FlipKey => "Flip Falling Piece (180 degree rotation)",
            Keybinds.HardDropKey => "Hard Drop Falling Piece",
            Keybinds.SoftDropKey => "Soft Drop Falling Piece",
            Keybinds.RetryKey => "Hold to Retry Game (if possible)",
            Keybinds.ForfeitKey => "Hold to Forfeit Game",
            Keybinds.PauseKey => "Pause Game",
            _ => throw new ArgumentOutOfRangeException(nameof(keybind), keybind, null)
        };
    }
}