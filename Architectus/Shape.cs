using System;
using System.Diagnostics.CodeAnalysis;

namespace Architectus;

public class HouseShape
{
    private List<Rect2Int> _rectangles = new();

    public Rect2Int BoundingBox { get; private set; } = new();

    public IEnumerable<Rect2Int> Rectangles => this._rectangles;

    public void AddRectangle(Rect2Int rectangle)
    {
        this._rectangles.Add(rectangle);
        this.BoundingBox = this.BoundingBox.Union(rectangle);
    }

    public int Area => this.Rectangles.Sum(r => r.Area);
}

public enum ShapeType
{
    SingleRect,
    TwoRects,
}

public class ShapeGenerator
{
    public Random Random { get; set; }

    public ShapeType Type { get; set; } = ShapeType.SingleRect;

    public Vector2Int MaxBoundsSize { get; set; } = new(10, 10);

    public Vector2Int MinRectSize { get; set; } = new(2, 2);

    public int MaxAttempts { get; set; } = 20;

    public int MinArea { get; set; } = 1;

    public ShapeGenerator(Random? random = null)
    {
        this.Random = random ?? System.Random.Shared;
    }

    public bool TryGenerateShape([NotNullWhen(true)] out HouseShape? shape)
    {
        int attempts = 0;
        while (attempts < this.MaxAttempts)
        {
            attempts++;
            shape = this.GenerateShape();
            if (shape.Area >= this.MinArea)
            {
                return true;
            }
        }
        shape = null;
        return false;
    }

    private enum Anchor
    {
        Top = 0, 
        Right = 1, 
        Bottom = 2,
        Left = 3, 
    }

    private Rect2Int Positionate(Vector2Int rectSize, Rect2Int relativeTo, Anchor anchor)
    {
        var maxDeltaY = relativeTo.Size.Y - rectSize.Y;
        var maxDeltaX = relativeTo.Size.X - rectSize.X;
        Vector2Int delta = anchor switch
        {
            Anchor.Left => new Vector2Int(-rectSize.X, this.Random.Next(maxDeltaY)),
            Anchor.Right => new Vector2Int(relativeTo.Size.X, this.Random.Next(maxDeltaY)),
            Anchor.Top => new Vector2Int(this.Random.Next(maxDeltaX), -rectSize.Y),
            Anchor.Bottom => new Vector2Int(this.Random.Next(maxDeltaX), relativeTo.Size.Y),
            _ => throw new ArgumentException($"Invalid anchor {anchor}"),
        };
        return new Rect2Int(relativeTo.Position + delta, rectSize);
    }

    private HouseShape GenerateShape()
    {
        var shape = new HouseShape();

        // The first rectangle is the "main rect". Position is always (0, 0).
        var w = this.Random.Next(this.MinRectSize.X, this.MaxBoundsSize.X + 1);
        var h = this.Random.Next(this.MinRectSize.Y, this.MaxBoundsSize.Y + 1);
        var mainRect = new Rect2Int(0, 0, w, h);
        shape.AddRectangle(mainRect);

        if (this.Type == ShapeType.SingleRect)
        {
            return shape;
        }

        // The second rectangle is the "secondary rect".
        // The secondary rect must not overlap the main rect
        // and it's always smaller than the main rect.
        var maxSize = Vector2Int.Min(this.MaxBoundsSize - mainRect.Size, mainRect.Size);
        w = this.Random.Next(this.MinRectSize.X, maxSize.X + 1);
        h = this.Random.Next(this.MinRectSize.Y, maxSize.Y + 1);
        var anchor = (Anchor)this.Random.Next(4);
        var secondaryRect = this.Positionate(new Vector2Int(w, h), mainRect, anchor);
        shape.AddRectangle(secondaryRect);


        return shape;
    }
}   