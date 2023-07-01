using System.Diagnostics.CodeAnalysis;
using Architectus.Support;

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

    public HouseLot Generate()
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

            numberOfAttempts++;
        }

        throw new InvalidOperationException("Failed to generate a house.");
    }

    public bool TryGenerate([NotNullWhen(true)] out HouseLot? house)
    {
        //var template = new TwoRoomHouseTemplate();
        var templates = new HouseTemplate[]
        {
            new TwoRoomHouseTemplate(),
            new SmallFamiliarHouseTemplate() { NumberOfBedrooms = 2 },
            new SmallFamiliarHouseTemplate() { NumberOfBedrooms = 3 },
            new SmallFamiliarHouseTemplate() { NumberOfBedrooms = 4 },
        };

        List<HouseLot> generated = new();
        foreach (var template in templates)
        {
            if (template.TryBuild(this.PlotSize, this._random, out var houseLot))
            {
                generated.Add(houseLot);
            }
        }

        Console.WriteLine($"Generated {generated.Count} houses.");

        if (generated.Count == 0)
        {
            house = null;
            return false;
        }

        house = generated[this._random.Next(generated.Count)];
        return true;
    }
}
