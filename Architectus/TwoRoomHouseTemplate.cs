using Architectus.Support;

namespace Architectus;

public abstract class HouseTemplate
{
    public abstract bool CanExecute(Vector2Int plotSize);

    protected abstract HouseLot ExecuteCore(Vector2Int plotSize, Random random);

    public HouseLot Execute(Vector2Int plotSize, Random random)
    {
        if (!this.CanExecute(plotSize))
        {
            throw new ArgumentException("Cannot execute template.");
        }

        return this.ExecuteCore(plotSize, random);
    }
}

public class TwoRoomHouseTemplate : HouseTemplate
{
    public override bool CanExecute(Vector2Int plotSize)
    {
        return plotSize.X >= 4 && plotSize.Y >= 3;
    }

    protected override HouseLot ExecuteCore(Vector2Int plotSize, Random random)
    {
        var lot = new HouseLot(plotSize);
        var floor = lot.GroundFloor;
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

        return lot;
    }
}
