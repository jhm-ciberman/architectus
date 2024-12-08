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

    /// <summary>
    /// Updates the world matrix of the element
    /// </summary>
    /// <param name="parentMatrix">The world matrix of the parent element</param>
    public virtual void UpdateWorldMatrix(Matrix3x2 parentMatrix)
    {
        var localTransform = this.GetLocalTransform();
        this.WorldMatrix = localTransform * parentMatrix;
    }

    /// <summary>
    /// Measures the element and returns the desired size. The <see cref="DesiredSize"/> property is set to the
    /// returned value.
    /// </summary>
    /// <param name="availableSize">The size that the parent element has available to give to this element</param>
    /// <returns>The desired size of the element</returns>
    public Vector2Int Measure(Vector2Int availableSize)
    {
        this.DesiredSize = this.MeasureOverride(availableSize);
        return this.DesiredSize;
    }

    /// <summary>
    /// Arranges the element within the given bounds. The <see cref="Bounds"/> property is set to the
    /// returned value.
    /// </summary>
    /// <param name="finalRect">The bounds that the element should be arranged within</param>
    /// <returns>The actual bounds that the element occupies</returns>
    public RectInt Arrange(RectInt finalRect)
    {
        var bounds = this.ArrangeOverride(finalRect);
        this.Bounds = TransformRect(bounds, this.WorldMatrix);

        return this.Bounds;
    }

    /// <summary>
    /// Imprints the element onto the house lot.
    /// </summary>
    /// <param name="house">The house lot to imprint the element onto</param>
    public virtual void Imprint(HouseLot house)
    {
        // Do nothing by default
    }

    /// <summary>
    /// Measures the element and returns the desired size.
    /// </summary>
    protected virtual Vector2Int MeasureOverride(Vector2Int availableSize)
    {
        return Vector2Int.Zero;
    }

    /// <summary>
    /// Arranges the element within the given bounds.
    /// </summary>
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
