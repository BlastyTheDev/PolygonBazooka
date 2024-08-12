using System;
using System.IO;
using Microsoft.Xna.Framework.Input;

namespace PolygonBazooka;

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

    public Preferences()
    {
        Load();
    }
    
    private void Load()
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
}