using System;

namespace PolygonBazooka.Util;

// ReSharper disable once InconsistentNaming
public static class RNG
{
    private static readonly Random Random = new();

    public static int Next() => Random.Next();
    public static int Next(int maxValue) => Random.Next(maxValue);
    public static int Next(int minValue, int maxValue) => Random.Next(minValue, maxValue);
}