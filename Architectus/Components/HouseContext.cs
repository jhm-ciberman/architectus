using System;
using Architectus.Layouts;

namespace Architectus.Components;

public class HouseContext
{
    public Random Random { get; private set; }

    public int Seed { get; }

    public HouseContext(int seed = 0)
    {
        this.Seed = seed == 0 ? Environment.TickCount : seed;
        this.Random = new Random(this.Seed);
    }

    public void Reset()
    {
        this.Random = new Random(this.Seed);
    }
}
