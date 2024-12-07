using System.Numerics;
using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

public abstract class ContentLayout : LayoutElement
{
    public LayoutElement Content { get; set; } = null!;

    public override void UpdateWorldMatrix(Matrix3x2 parentMatrix)
    {
        base.UpdateWorldMatrix(parentMatrix);

        this.Content.UpdateWorldMatrix(this.WorldMatrix);
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
