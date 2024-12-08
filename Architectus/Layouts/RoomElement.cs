using System;
using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

public class RoomElement : LayoutElement
{
    public Vector2Int MinSize { get; set; } = Vector2Int.One;

    public Vector2Int MaxSize { get; set; } = Vector2Int.MaxValue;

    public RoomType Type { get; set; } = RoomType.Garden;


    protected override Vector2Int MeasureOverride(Vector2Int availableSize)
    {
        return this.MinSize;
    }

    protected override RectInt ArrangeOverride(RectInt finalRect)
    {
        finalRect.Size = Vector2Int.Min(finalRect.Size, this.MaxSize);

        return finalRect;
    }

    public override void Imprint(HouseLot house)
    {
        house.GroundFloor.AddRoom(this.Bounds, this.Type);
    }
}
