using System;
using System.Diagnostics.CodeAnalysis;
using Architectus.Support;
using LifeSim.Support.Numerics;

namespace Architectus;

public abstract class HouseTemplate
{
    public abstract bool TryBuild(Vector2Int plotSize, Random random, [NotNullWhen(true)] out HouseLot? house);
}

public class TwoRoomHouseTemplate : HouseTemplate
{
    public override bool TryBuild(Vector2Int plotSize, Random random, [NotNullWhen(true)] out HouseLot? house)
    {
        if (plotSize.X < 4 || plotSize.Y < 3)
        {
            house = null;
            return false;
        }

        house = new HouseLot(plotSize);
        var floor = house.GroundFloor;
        // Entrance always asumed to be on the left side of the plot.
        // It's responsability of the caller to rotate the house if needed afterwords.
        //var entrance = new Vector2Int(0, random.Next(1, plotSize.Y - 1));
        //var houseBounds = floor.Bounds.Deflate(1);

        var minSize = new Vector2Int(4, 3);
        var maxSize = new Vector2Int(8, 5);
        var houseBounds = HouseHelper.DeflatePlotToIndoorSize(random, floor.Bounds, minSize, maxSize);

        // Room1: Living room (left side of the plot)
        // Room2: Bedroom.

        // Room1: Living room (left side of the plot)
        var ratio = random.NextGaussianRatio();
        var livingBounds = houseBounds.SplitRatioLeft(2, 2, ratio, out var bedroomBounds);

        floor.AddRoom(livingBounds, RoomType.LivingRoom);
        floor.AddRoom(bedroomBounds, RoomType.Bedroom);

        return true;
    }
}

public class SmallFamiliarHouseTemplate : HouseTemplate
{
    public int NumberOfBedrooms { get; set; } = 2;

    public override bool TryBuild(Vector2Int plotSize, Random random, [NotNullWhen(true)] out HouseLot? house)
    {
        if (plotSize.X < 4 + this.NumberOfBedrooms * 3 || plotSize.Y < 4)
        {
            house = null;
            return false;
        }

        house = new HouseLot(plotSize);
        var floor = house.GroundFloor;

        // For this house we will create one living room in the left side
        // and one corridor with N bedrooms in the right side.
        // Something like this:
        // +----------+-----------------------------+
        // | Living   | Corridor                    |
        // | Room     +-----------------------------+
        // |          | Bedroom 1 | Bedroom 2 | ... |
        // |          |           |           |     |
        // |          |           |           |     |
        // +----------+-----------+-----------+-----+
        // The corridor could be at the top or at the bottom.

        // Pick a random size for the house
        var minSize = new Vector2Int(10, 4);
        var maxSize = new Vector2Int(20, 10);
        var houseBounds = HouseHelper.DeflatePlotToIndoorSize(random, floor.Bounds, minSize, maxSize);

        // Create the living room
        int livingWidth = 2 + (int)(random.NextGaussianRatio() * 5);
        var livingBounds = houseBounds.SplitLeft(livingWidth, out var privateBounds);

        floor.AddRoom(livingBounds, RoomType.LivingRoom);

        // Create the private area
        int corridorThickness = privateBounds.Y > 8 ? 2 : 1;
        var corridorBounds = privateBounds.SplitTop(corridorThickness, out var bedroomsBounds);

        floor.AddRoom(corridorBounds, RoomType.Corridor);

        // Now, create the bedrooms until we dont have enough space
        var bedroomWidth = bedroomsBounds.Width / this.NumberOfBedrooms;
        int extraSpace = bedroomsBounds.Width % bedroomWidth;
        int originalBedroomsWidth = bedroomsBounds.Width;
        int bigBedroomWidth = bedroomWidth + extraSpace; // Sorry for the kids, they will have the smallest rooms.

        int bigBedroomIndex = random.Next(this.NumberOfBedrooms);


        Console.WriteLine($"Bedrooms: {this.NumberOfBedrooms} - {bedroomsBounds}");
        for (int i = 0; i < this.NumberOfBedrooms; i++)
        {
            int width = i == bigBedroomIndex ? bigBedroomWidth : bedroomWidth;
            if (bedroomsBounds.Width == width)
            {
                floor.AddRoom(bedroomsBounds, RoomType.Bedroom);
            }
            else
            {
                bedroomsBounds = bedroomsBounds.SplitLeft(width, out var nextBedroomsBounds);
                floor.AddRoom(bedroomsBounds, RoomType.Bedroom);
                bedroomsBounds = nextBedroomsBounds;
            }
        }

        return true;
    }


}
