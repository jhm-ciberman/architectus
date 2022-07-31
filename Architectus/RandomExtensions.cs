namespace Architectus;

public static class RandomExtensions
{
    public static T Choose<T>(this Random random, IReadOnlyList<T> values)
    {
        return values[random.Next(values.Count)];
    }

    public static T Choose<T>(this Random random, IEnumerable<T> values)
    {
        return Choose(random, values.ToList());
    }
}