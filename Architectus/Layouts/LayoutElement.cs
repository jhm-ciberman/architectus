using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

public abstract class LayoutElement
{
    /// <summary>
    /// Gets or sets whether the element and its children should be flipped horizontally.
    /// </summary>
    public bool FlipX { get; set; }

    /// <summary>
    /// Gets or sets whether the element and its children should be flipped vertically.
    /// </summary>
    public bool FlipY { get; set; }

    /// <summary>
    /// Gets the desired size of the element. This is the size that the element would like to be, after
    /// the Measure pass.
    /// </summary>
    public Vector2Int DesiredSize { get; protected set; }

    /// <summary>
    /// Gets the actual bounds that the element occupies in the layout after the Arrange pass.
    /// </summary>
    public RectInt Bounds { get; private set; }

    protected bool WorldFlipX { get; private set; }

    protected bool WorldFlipY { get; private set; }

    public virtual void UpdateWorldTransform(LayoutElement parent)
    {
        this.WorldFlipX = parent.WorldFlipX ^ this.FlipX;
        this.WorldFlipY = parent.WorldFlipY ^ this.FlipY;
    }

    public Vector2Int Measure(Vector2Int availableSize)
    {
        this.DesiredSize = this.MeasureOverride(availableSize);
        return this.DesiredSize;
    }

    public RectInt Arrange(RectInt finalRect)
    {
        // Apply flips to determine final position
        var x = this.WorldFlipX ? finalRect.X + finalRect.Width - this.DesiredSize.X : finalRect.X;
        var y = this.WorldFlipY ? finalRect.Y + finalRect.Height - this.DesiredSize.Y : finalRect.Y;

        var localBounds = new RectInt(x, y, this.DesiredSize.X, this.DesiredSize.Y);
        this.Bounds = this.ArrangeOverride(localBounds);

        return this.Bounds;
    }

    public virtual void Imprint(HouseLot house)
    {
        // Do nothing by default
    }

    protected virtual Vector2Int MeasureOverride(Vector2Int availableSize)
    {
        return Vector2Int.Zero;
    }

    protected virtual RectInt ArrangeOverride(RectInt finalRect)
    {
        return finalRect;
    }
}

