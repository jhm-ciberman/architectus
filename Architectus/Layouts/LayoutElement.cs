using System.Numerics;
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

    public Matrix3x2 WorldMatrix { get; private set; } = Matrix3x2.Identity;

    public virtual void UpdateWorldMatrix(Matrix3x2 parentMatrix)
    {
        var localTransform = this.GetLocalTransform();
        this.WorldMatrix = localTransform * parentMatrix;
    }

    public Vector2Int Measure(Vector2Int availableSize)
    {
        this.DesiredSize = this.MeasureOverride(availableSize);
        return this.DesiredSize;
    }

    public RectInt Arrange(RectInt finalRect)
    {
        var bounds = this.ArrangeOverride(finalRect);
        this.Bounds = TransformRect(bounds, this.WorldMatrix);

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

    private Matrix3x2 GetLocalTransform()
    {
        Vector2 center = this.Bounds.Position + new Vector2(this.DesiredSize.X, this.DesiredSize.Y) / 2;
        float scaleX = this.FlipX ? -1 : 1;
        float scaleY = this.FlipY ? -1 : 1;

        return Matrix3x2.CreateScale(scaleX, scaleY, center);
    }

    private static RectInt TransformRect(RectInt rect, Matrix3x2 matrix)
    {
        var a = new Vector2(rect.X, rect.Y);
        var b = new Vector2(rect.X + rect.Width, rect.Y);
        var c = new Vector2(rect.X, rect.Y + rect.Height);
        var d = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);

        a = Vector2.Transform(a, matrix);
        b = Vector2.Transform(b, matrix);
        c = Vector2.Transform(c, matrix);
        d = Vector2.Transform(d, matrix);

        var min = Vector2.Min(Vector2.Min(a, b), Vector2.Min(c, d));
        var max = Vector2.Max(Vector2.Max(a, b), Vector2.Max(c, d));
        var delta = max - min;

        return new RectInt((int)min.X, (int)min.Y, (int)delta.X, (int)delta.Y);
    }
}
