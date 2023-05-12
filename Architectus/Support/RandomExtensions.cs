using System;
using System.Collections.Generic;
using System.Linq;

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
}