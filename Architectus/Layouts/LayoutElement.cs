using System.Numerics;
using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

/// <summary>
/// Represents an element in a layout. This is the base class for all layout elements
/// and provides the basic functionality for measuring, arranging, and imprinting elements.
/// </summary>
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

    /// <summary>
    /// Gets the world matrix of the element. This matrix transforms the local space of the element to the
    /// world space of the layout.
    /// </summary>
    public Matrix3x2 WorldMatrix { get; private set; } = Matrix3x2.Identity;

    /// <summary>
    /// Gets or sets the dock position of the element within its parent. This property is only
    /// relevent for elements that are children of a <see cref="DockLayout"/>.
    /// </summary>
    public Dock Dock { get; set; } = Dock.Left;

    /// <summary>
    /// Gets or sets the allocation weight. When multiple rooms are placed in a stack layout,
    /// the remaining space after measuring all rooms is distributed based on the weight of each room.
    /// So a room with a weight of 2.0 will get four times as much space as a room with a weight of 0.5.
    /// </summary>
    public float Weight { get; set; } = 1.0f;

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

    /// <summary>
    /// Computes the local transform of the element.
    /// </summary>
    /// <returns>The local transform matrix</returns>
    private Matrix3x2 GetLocalTransform()
    {
        Vector2 center = this.Bounds.Position + new Vector2(this.DesiredSize.X, this.DesiredSize.Y) / 2;
        float scaleX = this.FlipX ? -1 : 1;
        float scaleY = this.FlipY ? -1 : 1;

        return Matrix3x2.CreateScale(scaleX, scaleY, center);
    }

    /// <summary>
    /// Transforms a rectangle by a transformation matrix.
    /// </summary>
    /// <param name="rect">The rectangle to transform</param>
    /// <param name="matrix">The transformation matrix</param>
    /// <returns>The transformed rectangle</returns>
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
