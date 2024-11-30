using System;
using Architectus.Layouts;
using LifeSim.Support.Numerics;

namespace Architectus;

public abstract class LayoutElement
{
    public Vector2Int DesiredSize { get; protected set; }

    public abstract void Measure(Vector2Int availableSize);

    public abstract void Arrange(RectInt finalRect);

    public abstract void Imprint(HouseLot house);
}

public class RoomElement : LayoutElement
{
    public Vector2Int Size { get; set; }

    public RoomType Type { get; set; } = RoomType.Garden;

    public RectInt Bounds { get; private set; }

    public override void Measure(Vector2Int availableSize)
    {
        if (availableSize.X < this.Size.X || availableSize.Y < this.Size.Y)
        {
            throw new InvalidOperationException($"The available size is too small for the room. Required: {this.Size}, available: {availableSize}.");
        }

        this.DesiredSize = this.Size;
    }

    public override void Arrange(RectInt finalRect)
    {
        this.Bounds = finalRect;
    }

    public override void Imprint(HouseLot house)
    {
        house.GroundFloor.AddRoom(this.Bounds, this.Type);
    }
}

public class HouseGenerator
{
    public Vector2Int PlotSize { get; set; } = new Vector2Int(10, 10);

    public bool TryGenerate(out HouseLot house)
    {
        var layout = new PaddingLayout
        {
            Padding = 1,
            Child = new StackLayout
            {
                Orientation = LayoutOrientation.Horizontal,
                LastChildFill = true,
                Children =
                {
                    new RoomElement { Size = new Vector2Int(3, 3), Type = RoomType.LivingRoom },
                    new RoomElement { Size = new Vector2Int(3, 3), Type = RoomType.Kitchen },
                    //new RoomElement { Size = new Vector2Int(3, 3), Type = RoomType.Bedroom },
                },
            },
        };


        house = new HouseLot(this.PlotSize);

        layout.Measure(this.PlotSize);
        layout.Arrange(new RectInt(0, 0, this.PlotSize.X, this.PlotSize.Y));
        layout.Imprint(house);

        return true;
    }
}
