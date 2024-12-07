using System.Numerics;
using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

public abstract class ContentLayout : LayoutElement
{
    public LayoutElement Content { get; set; } = null!;

    public override void UpdateWorldTransform(LayoutElement parent)
    {
        base.UpdateWorldTransform(parent);

        this.Content.UpdateWorldTransform(this);
    }

    protected override RectInt ArrangeOverride(RectInt finalRect)
    {
        this.Content.Arrange(finalRect);

        return this.Content.Bounds;
    }

    protected override Vector2Int MeasureOverride(Vector2Int availableSize)
    {
        this.Content.Measure(availableSize);

        return this.Content.DesiredSize;
    }

    public override void Imprint(HouseLot house)
    {
        this.Content.Imprint(house);
    }
}
