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
        }

        throw new InvalidOperationException("Failed to generate a house.");
    }

    private bool TryGenerate([NotNullWhen(true)] out HouseLot? house)
    {
        var template = new TwoRoomHouseTemplate();
        var plotSize = this.PlotSize;

        if (template.CanExecute(plotSize))
        {
            house = template.Execute(plotSize, this._random);
            return true;
        }

        house = null;
        return false;
    }
}

public class TwoRoomHouseTemplate
{
    public bool CanExecute(Vector2Int plotSize)
    {
        return plotSize.X >= 5 && plotSize.Y >= 3;
    }

    /*public HouseFeatures GetFeatures()
    {
        return new HouseFeatures
        {
            
        }
    }*/

    public HouseLot Execute(Vector2Int plotSize, Random random)
    {
        if (!this.CanExecute(plotSize))
        {
            throw new ArgumentException("Cannot execute template.");
        }

        var lot = new HouseLot(plotSize);
        var floor = lot.GroundFloor;
        // Entrance always asumed to be on the left side of the plot.
        // It's responsability of the caller to rotate the house if needed afterwords.
        var entrance = new Vector2Int(0, random.Next(1, plotSize.Y - 1));
        var houseBounds = floor.Bounds.Deflate(1);

        // Room1: Living room (left side of the plot)
        // Room2: Bedroom.

        // Room1: Living room (left side of the plot)
        var ratio = (float)random.NextNormalizedDouble();
        var livingBounds = houseBounds.SplitRatioLeft(2, 2, ratio, out var bedroomBounds);

        floor.AddRoom(livingBounds, RoomType.LivingRoom);
        floor.AddRoom(bedroomBounds, RoomType.Bedroom);

        return lot;
    }
}