using System;
using System.Collections.Generic;
using System.Linq;
using Architectus.Layouts;
using CommunityToolkit.Diagnostics;
using LifeSim.Support.Numerics;

namespace Architectus.Support;

public static class RandomExtensions
{
    public static Orientation NextOrientation(this Random random)
    {
        return (Orientation)random.Next(4);
    }

    public static bool NextBool(this Random random)
    {
        return random.Next(2) == 0;
    }

    public static Orientation NextOrientationRow(this Random random)
    {
        return random.NextBool() ? Orientation.Row : Orientation.RowReverse;
    }

    public static Orientation NextOrientationColumn(this Random random)
    {
        return random.NextBool() ? Orientation.Column : Orientation.ColumnReverse;
    }

    public static Orientation NextOrientationLargeSide(this Random random, Vector2Int size)
    {
        return size.X > size.Y ? NextOrientationRow(random) : NextOrientationColumn(random);
    }

    public static Orientation NextOrientationSmallSide(this Random random, Vector2Int size)
    {
        return size.X > size.Y ? NextOrientationColumn(random) : NextOrientationRow(random);
    }

    public static float NextSingle(this Random random, float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }

    public static float NextSingle(this Random random, float max)
    {
        return random.NextSingle(0, max);
    }

    public static T Choose<T>(this Random random, IReadOnlyList<T> values)
    {
        return values[random.Next(values.Count)];
    }

    public static T Choose<T>(this Random random, IEnumerable<T> values)
    {
        if (values is IReadOnlyList<T> list)
            return random.Choose(list);

        return random.Choose(values.ToList());
    }

    public static double NextGaussianDouble(this Random random, double mean, double standardDeviation)
    {
        double u1 = 1.0d - random.NextDouble();
        double u2 = 1.0d - random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        double randNormal = mean + standardDeviation * randStdNormal;
        return randNormal;
    }

    public static float NextGaussianRatio(this Random random, double standardDeviation = 0.15d)
    {
        const double mean = 0.5d;
        float randNormal = (float)random.NextGaussianDouble(mean, standardDeviation);
        float normalizedValue = MathF.Max(0, MathF.Min(1, (float)randNormal));
        return normalizedValue;
    }

    private static double RemapRange(double value, double from1, double to1, double from2, double to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    /// <summary>
    /// Generates a random number between 0 and 1, with a bias towards the specified value.
    /// </summary>
    /// <param name="random"></param>
    /// <param name="bias">The value to bias towards. Must be between 0 and 1.</param>
    /// <param name="standardDeviation">The standard deviation of the gaussian distribution.</param>
    /// <returns></returns>
    public static double NextBiasedGaussianRatio(this Random random, double bias, double standardDeviation = 0.15d)
    {
        Guard.IsBetweenOrEqualTo(bias, 0, 1, nameof(bias));

        double value = random.NextGaussianRatio(standardDeviation);

        if (value < 0.5d)
            value = RemapRange(value, 0, 0.5d, 0, bias);
        else
        {
            value = RemapRange(value, 0.5d, 1, bias, 1);
        }

        return value;
    }

    /// <summary>
    /// Generates a random <see cref="Vector2Int"/> with the specified minimum and maximum values.
    /// </summary>
    /// <param name="random"></param>
    /// <param name="min">The minimum values for the <see cref="Vector2Int"/>.</param>
    /// <param name="max">The maximum values for the <see cref="Vector2Int"/>.</param>
    /// <returns></returns>
    public static Vector2Int NextVector2Int(this Random random, Vector2Int min, Vector2Int max)
    {
        int x = random.Next(min.X, max.X + 1);
        int y = random.Next(min.Y, max.Y + 1);
        return new Vector2Int(x, y);
    }

    public static Vector2Int NextVector2Int(this Random random, Vector2Int max)
    {
        return random.NextVector2Int(Vector2Int.Zero, max);
    }
}
