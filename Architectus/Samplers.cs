namespace Architectus;

public interface ISampler
{
    /// <summary>
    /// Samples a random number between min and max. The number is guaranteed to be between min and max.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="min">The minimum value to sample.</param>
    /// <param name="max">The maximum value to sample.</param>
    /// <returns>A random number between min and max.</returns>
    float Sample(Random random, float min, float max);
}


partial class UniformSampler : ISampler
{
    private UniformSampler() { }

    public static UniformSampler Instance { get; } = new UniformSampler();

    public float Sample(Random random, float min, float max)
    {
        return (float)(random.NextDouble() * (max - min) + min);
    }
}


public class GaussianSampler : ISampler
{

    private GaussianSampler() {}

    public static GaussianSampler Instance { get; } = new GaussianSampler();

    private static double NextGaussian(Random random, double mean, double standardDeviation)
    {
        double u1 = 1.0d - random.NextDouble();
        double u2 = 1.0d - random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        double randNormal = mean + standardDeviation * randStdNormal;
        return randNormal;
    }

    // Sample some random number using the Random provided and the mean and standard deviation.
    // The random number will be clamped to the mean - standard deviation and mean + standard deviation
    public float Sample(Random random, float min, float max)
    {
        double mean = (max + min) / 2.0;
        double standardDeviation = (max - min) / 6.0;
        float value = (float)NextGaussian(random, mean, standardDeviation);
        return MathF.Max(min, MathF.Min(max, value));
    }
}



