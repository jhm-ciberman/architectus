using System;
using System.Numerics;
using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

public enum LayoutRotation
{
    Rotation0 = 0,
    Rotation90 = 90,
    Rotation180 = 180,
    Rotation270 = 270
}

public abstract class LayoutElement
{
    public LayoutRotation Rotation { get; set; } = LayoutRotation.Rotation0;
    public bool FlipX { get; set; }
    public bool FlipY { get; set; }

    public Vector2Int DesiredSize { get; protected set; }
    public RectInt Bounds { get; private set; }

    public Matrix3x2 WorldMatrix { get; private set; } = Matrix3x2.Identity;

    public virtual void UpdateWorldMatrix(Matrix3x2 parentMatrix)
    {
        var localTransform = this.GetLocalTransform();
        this.WorldMatrix = localTransform * parentMatrix;
    }

    public Vector2Int Measure(Vector2Int availableSize)
    {
        availableSize = this.TransformSizeInverse(availableSize);

        this.DesiredSize = this.MeasureOverride(availableSize);
        return this.TransformSize(this.DesiredSize);
    }

    public RectInt Arrange(RectInt finalRect)
    {
        var bounds = this.ArrangeOverride(finalRect);
        this.Bounds = this.TransformRect(bounds);

        return this.Bounds;
    }

    public virtual void Imprint(HouseLot house) { }

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
        // Determine the center point of the element based on its bounds
        Vector2 center = this.Bounds.Position + new Vector2(this.DesiredSize.X, this.DesiredSize.Y) / 2;

        // Create rotation and flip matrices
        var rotationAngle = (int)this.Rotation * MathF.PI / 180f;
        var rotation = Matrix3x2.CreateRotation(rotationAngle, center);
        var flip = Matrix3x2.CreateScale(this.FlipX ? -1 : 1, this.FlipY ? -1 : 1, center);

        // Combine transformations: toCenter -> (flip + rotate) -> fromCenter
        return flip * rotation;
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

    private RectInt TransformRect(RectInt rect)
    {
        return TransformRect(rect, this.WorldMatrix);
    }

    private RectInt TransformRectInverse(RectInt rect)
    {
        if (!Matrix3x2.Invert(this.WorldMatrix, out var inverse))
        {
            throw new InvalidOperationException("Matrix is not invertible.");
        }

        return TransformRect(rect, inverse);
    }

    private Vector2Int TransformSize(Vector2Int size)
    {
        return (((int)this.Rotation) % 180) == 0 ? size : new Vector2Int(size.Y, size.X);
    }

    private Vector2Int TransformSizeInverse(Vector2Int size)
    {
        return (((int)this.Rotation) % 180) == 0 ? size : new Vector2Int(size.Y, size.X);
    }
}

