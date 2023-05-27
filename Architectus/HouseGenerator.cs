using System;
using System.Diagnostics.CodeAnalysis;

namespace Architectus;

public class HouseGenerator
{
    private readonly Random _random;

    public Vector2Int PlotSize { get; set; } = new Vector2Int(10, 10);

    public CardinalDirection PlotDirection { get; } // The direction the plot entrance is facing.

    public HouseGenerator(Random? random = null)
    {
        this._random = random ?? Random.Shared;
    }

    public House Generate()
    {
        int numberOfAttempts = 0;

        if (this.PlotSize.X < 3 || this.PlotSize.Y < 3)
        {
            throw new ArgumentException("Plot size must be at least 3x3.");
        }

        while (numberOfAttempts < 3)
        {
            if (this.TryGenerate(out var house))
            {
                return house;
            }
        }

        throw new InvalidOperationException("Failed to generate a house.");
    }

    private bool TryGenerate([NotNullWhen(true)] out House? house)
    {
        house = new House(this.PlotSize);
        var floor = house.AddFloor();
        return true;
    }
}