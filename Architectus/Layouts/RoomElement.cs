using System;
using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

public class RoomElement : LayoutElement
{
    public Vector2Int Size { get; set; }

    public RoomType Type { get; set; } = RoomType.Garden;


    protected override Vector2Int MeasureOverride(Vector2Int availableSize)
    {
        return this.Size;
    }

    protected override RectInt ArrangeOverride(RectInt finalRect)
    {
        return finalRect;
    }

    public override void Imprint(HouseLot house)
    {
        house.GroundFloor.AddRoom(this.Bounds, this.Type);
    }
}
