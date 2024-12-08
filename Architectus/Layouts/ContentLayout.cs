using System.Numerics;
using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

/// <summary>
/// Represents a layout element that contains a single child element.
/// </summary>
public abstract class ContentLayout : LayoutElement
{
    /// <summary>
    /// Gets or sets the content of the layout.
    /// </summary>
    public LayoutElement Content { get; set; } = null!;

    /// <inheritdoc/>
    public override void UpdateWorldMatrix(Matrix3x2 parentMatrix)
    {
        base.UpdateWorldMatrix(parentMatrix);

        this.Content.UpdateWorldMatrix(this.WorldMatrix);
    }

    /// <inheritdoc/>
    protected override RectInt ArrangeOverride(RectInt finalRect)
    {
        this.Content.Arrange(finalRect);

        return this.Content.Bounds;
    }

    /// <inheritdoc/>
    protected override Vector2Int MeasureOverride(Vector2Int availableSize)
    {
        this.Content.Measure(availableSize);

        return this.Content.DesiredSize;
    }

    /// <inheritdoc/>
    public override void Imprint(HouseLot house)
    {
        this.Content.Imprint(house);
    }
}
