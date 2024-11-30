using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

public class PaddingLayout : DecoratorLayout
{
    public int Padding { get; set; }

    public override void Measure(Vector2Int availableSize)
    {
        var padding = Vector2Int.One * this.Padding * 2;
        this.Child.Measure(availableSize - padding);

        this.DesiredSize = this.Child.DesiredSize + padding;
    }

    public override void Arrange(RectInt finalRect)
    {
        this.Child.Arrange(new RectInt(
            finalRect.X + this.Padding,
            finalRect.Y + this.Padding,
            finalRect.Width - 2 * this.Padding,
            finalRect.Height - 2 * this.Padding));
    }

    public override void Imprint(HouseLot house)
    {
        this.Child.Imprint(house);
    }
}
