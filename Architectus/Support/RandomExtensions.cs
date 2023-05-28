namespace Architectus;

public static class RandomExtensions
{
    public static T Choose<T>(this Random random, IReadOnlyList<T> values)
    {
        return values[random.Next(values.Count)];
    }

    public static T Choose<T>(this Random random, IEnumerable<T> values)
    {
        if (values is IReadOnlyList<T> list)
        {
            return Choose(random, list);
        }
        
        return Choose(random, values.ToList());
    }

    public static double NextGaussianDouble(this Random random, double mean, double standardDeviation)
    {
        double u1 = 1.0d - random.NextDouble();
        double u2 = 1.0d - random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        double randNormal = mean + standardDeviation * randStdNormal;
        return randNormal;
    }

    public static double NextNormalizedDouble(this Random random, double standardDeviation = 0.15d)
    {
        const double mean = 0.5d;
        double randNormal = random.NextGaussianDouble(mean, standardDeviation);
        double normalizedValue = Math.Max(0, Math.Min(1, randNormal));
        return normalizedValue;
    }
}