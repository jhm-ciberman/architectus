using System;
using Architectus.Layouts;

namespace Architectus.Components;

public class HouseContext
{
    public Random Random { get; }

    public HouseContext(int seed = 0)
    {
        if (seed == 0)
        {
            seed = Environment.TickCount;
        }

        this.Random = new Random(seed);
    }

    internal Orientation RandomOrientation()
    {
        return (Orientation)this.Random.Next(4);
    }
}
