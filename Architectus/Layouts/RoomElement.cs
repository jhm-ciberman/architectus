using System;
using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

public class RoomElement : LayoutElement
{
    /// <summary>
    /// Gets or sets the minimum size of the room.
    /// </summary>
    public Vector2Int MinSize { get; set; } = Vector2Int.One;

    /// <summary>
    /// Gets or sets the maximum size of the room.
    /// </summary>
    public Vector2Int MaxSize { get; set; } = Vector2Int.MaxValue;

    /// <summary>
    /// Gets or sets the type of the room.
    /// </summary>
    public RoomType Type { get; set; } = RoomType.Garden;

    protected override Vector2Int MeasureOverride(Vector2Int availableSize)
    {
        return this.MinSize;
    }

    protected override RectInt ArrangeOverride(RectInt finalRect)
    {
        finalRect.Size = Vector2Int.Max(finalRect.Size, this.MinSize);
        finalRect.Size = Vector2Int.Min(finalRect.Size, this.MaxSize);

        return finalRect;
    }

    public override void Imprint(HouseLot house)
    {
        house.GroundFloor.AddRoom(this.Bounds, this.Type);
    }
}
