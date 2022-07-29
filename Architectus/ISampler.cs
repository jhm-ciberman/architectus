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


