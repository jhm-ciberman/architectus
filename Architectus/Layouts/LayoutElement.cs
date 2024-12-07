using System;
using System.Linq;
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

        Console.WriteLine($"{this.GetType().Name} WorldMatrix: {this.WorldMatrix}");
    }

    public Vector2Int Measure(Vector2Int availableSize)
    {
        Console.WriteLine($"Measuring1 {this.GetType().Name} with available size {availableSize}");

        availableSize = this.TransformSizeInverse(availableSize);

        Console.WriteLine($"Measuring2 {this.GetType().Name} with available size {availableSize}");

        this.DesiredSize = this.MeasureOverride(availableSize);
        return this.TransformSize(this.DesiredSize);
    }

    public RectInt Arrange(RectInt finalRect)
    {
        Console.WriteLine($"Arranging1 {this.GetType().Name} with final rect {finalRect}");

        var bounds = this.ArrangeOverride(finalRect);
        this.Bounds = this.TransformRect(bounds);

        Console.WriteLine($"Arranging3 {this.GetType().Name} with bounds {bounds} => {this.Bounds}");

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

    private Vector2Int TransformSize(Vector2Int size)
    {
        var rect = this.TransformRect(new RectInt(0, 0, size.X, size.Y));
        return new Vector2Int(rect.Width, rect.Height);
    }

    private Vector2Int TransformSizeInverse(Vector2Int size)
    {
        var rect = this.TransformRectInverse(new RectInt(0, 0, size.X, size.Y));
        return new Vector2Int(rect.Width, rect.Height);
    }

    private static RectInt TransformRect(RectInt rect, Matrix3x2 matrix)
    {
        var points = new[]
        {
            Vector2.Transform(new Vector2(rect.X, rect.Y), matrix),
            Vector2.Transform(new Vector2(rect.X + rect.Width, rect.Y), matrix),
            Vector2.Transform(new Vector2(rect.X, rect.Y + rect.Height), matrix),
            Vector2.Transform(new Vector2(rect.X + rect.Width, rect.Y + rect.Height), matrix)
        };

        var minX = points.Min(p => p.X);
        var minY = points.Min(p => p.Y);
        var maxX = points.Max(p => p.X);
        var maxY = points.Max(p => p.Y);

        return new RectInt((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));
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
}

