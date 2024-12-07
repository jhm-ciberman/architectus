using System;
using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

public class PaddingLayout : ContentLayout
{
    public ThicknessInt Padding { get; set; }

    protected override Vector2Int MeasureOverride(Vector2Int availableSize)
    {
        this.Content.Measure(availableSize - this.Padding.Total);

        return availableSize;
    }

    protected override RectInt ArrangeOverride(RectInt finalRect)
    {
        finalRect = finalRect.Deflate(this.Padding);

        return this.Content.Arrange(finalRect);
    }
}
