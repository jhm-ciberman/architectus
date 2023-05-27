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

    // Sample some random number using the Random provided and the mean and standard deviation.
    // The random number will be clamped to the mean - standard deviation and mean + standard deviation
    public float Sample(Random random, float min, float max)
    {
        double mean = (max + min) / 2.0;
        double standardDeviation = (max - min) / 6.0;
        float value = (float)random.NextGaussianDouble(mean, standardDeviation);
        return MathF.Max(min, MathF.Min(max, value));
    }
}



