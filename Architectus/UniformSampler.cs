namespace Architectus;

partial class UniformSampler : ISampler
{
    private UniformSampler() { }

    public static UniformSampler Instance { get; } = new UniformSampler();

    public float Sample(Random random, float min, float max)
    {
        return (float)(random.NextDouble() * (max - min) + min);
    }
}


