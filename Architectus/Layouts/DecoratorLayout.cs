using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

public abstract class DecoratorLayout : LayoutElement
{
    public LayoutElement Child { get; set; } = null!;

    public override void Measure(Vector2Int availableSize)
    {
        this.Child.Measure(availableSize);
        this.DesiredSize = this.Child.DesiredSize;
    }

    public override void Arrange(RectInt finalRect)
    {
        this.Child.Arrange(finalRect);
    }

    public override void Imprint(HouseLot house)
    {
        this.Child.Imprint(house);
    }
}
