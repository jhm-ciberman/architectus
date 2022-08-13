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

    public void Translate(Vector2Int translation)
    {
        for (var i = 0; i < this._rectangles.Count; i++)
        {
            this._rectangles[i] = this._rectangles[i].Translate(translation);
        }
        this.BoundingBox = this.BoundingBox.Translate(translation);
    }
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

    public Vector2Int PlotSize { get; set; } = new(10, 10);

    public float GardenToHouseMinRatio { get; set; } = 0.2f;

    public float GardenToHouseMaxRatio { get; set; } = 1.0f;

    public float MainRectMinAspect { get; set; } = 2.22f;

    public int MainRectMinArea { get; set; } = 20;

    public int SecondaryRectMinArea { get; set; } = 10;

    public float SecondaryRectMinAspect { get; set; } = 1.8f;

    public int MaxAttempts { get; set; } = 50;

    public int MinArea { get; set; } = 45;

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
            if (shape != null && shape.Area >= this.MinArea)
            {
                this.TranslateShape(shape);
                return true;
            }
        }
        Console.WriteLine($"Failed to generate a shape after {attempts} attempts.");
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

    private bool CheckAspect(Vector2Int size, float minAspect)
    {
        if (size.X == 0 || size.Y == 0) return false;
        return (size.X > size.Y)
            ? (float)size.X / size.Y >= minAspect
            : (float)size.Y / size.X >= minAspect;
    }

    private HouseShape? GenerateShape()
    {
        var maxBoundsSize = this.PlotSize - new Vector2Int(2, 2); // 1 tile border
        var shape = new HouseShape();

        // The first rectangle is the "main rect". Position is always (0, 0).
        var w = this.Random.Next(this.MainRectMinSize.X, maxBoundsSize.X + 1);
        var h = this.Random.Next(this.MainRectMinSize.Y, maxBoundsSize.Y + 1);
        var mainRect = new Rect2Int(0, 0, w, h);

        if (mainRect.Area < this.MainRectMinArea)
        {
            Console.WriteLine($"Main rect area too small ({mainRect.Area} < {this.MainRectMinArea})");
            return null;
        }

        shape.AddRectangle(mainRect);
        if (this.Type == ShapeType.SingleRect)
        {
            return shape;
        }

        // The second rectangle is the "secondary rect".
        // The secondary rect must not overlap the main rect
        // and it's always smaller than the main rect.
        var anchor = (Anchor)this.Random.Next(4);
        var maxSize = anchor switch
        {
            Anchor.Left => new Vector2Int(maxBoundsSize.X - mainRect.Size.X, mainRect.Size.Y),
            Anchor.Right => new Vector2Int(maxBoundsSize.X - mainRect.Size.X, mainRect.Size.Y),
            Anchor.Top => new Vector2Int(mainRect.Size.X, maxBoundsSize.Y - mainRect.Size.Y),
            Anchor.Bottom => new Vector2Int(mainRect.Size.X, maxBoundsSize.Y - mainRect.Size.Y),
            _ => throw new ArgumentException($"Invalid anchor {anchor}"),
        };

        if (maxSize.X < this.SecondaryRectMinSize.X || maxSize.Y < this.SecondaryRectMinSize.Y)
        {
            Console.WriteLine($"Secondary rect size too small ({maxSize} < {this.SecondaryRectMinSize})");
            return shape;
        }

        w = this.Random.Next(this.SecondaryRectMinSize.X, maxSize.X + 1);
        h = this.Random.Next(this.SecondaryRectMinSize.Y, maxSize.Y + 1);
        if (w * h < this.SecondaryRectMinArea)
        {
            Console.WriteLine($"Secondary rect area too small ({w * h} < {this.SecondaryRectMinArea})");
            return shape;
        }
        
        var secondaryRect = this.Positionate(new Vector2Int(w, h), mainRect, anchor);
        shape.AddRectangle(secondaryRect);

        return shape;
    }

    private void TranslateShape(HouseShape shape)
    {
        var maxDelta = this.PlotSize - new Vector2Int(2, 2) - shape.BoundingBox.Size;
        if (maxDelta.X < 0 || maxDelta.Y < 0)
        {
            throw new ArgumentException("Shape is too big to fit in the plot");
        }

        Vector2Int delta = -shape.BoundingBox.Position + Vector2Int.One;

        if (maxDelta.X > 0)
        {
            delta.X += this.Random.Next(maxDelta.X + 1);
        }

        if (maxDelta.Y > 0)
        {
            delta.Y += this.Random.Next(maxDelta.Y + 1);
        }

        shape.Translate(delta);
    }


}   