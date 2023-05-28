using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Architectus;

public struct RoomBounds
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public RoomBounds(int x, int y, int width, int height)
    {
        this.X = x;
        this.Y = y;
        this.Width = width;
        this.Height = height;
    }

    public RoomBounds(Vector2Int position, Vector2Int size)
        : this(position.X, position.Y, size.X, size.Y) { }

    public RoomBounds(Vector2Int position, int width, int height)
        : this(position.X, position.Y, width, height) { }

    public RoomBounds(int x, int y, Vector2Int size)
        : this(x, y, size.X, size.Y) { }

    public Vector2Int Position => new Vector2Int(this.X, this.Y);
    public Vector2Int Size => new Vector2Int(this.Width, this.Height);
    public Vector2Int Center => new Vector2Int(this.X + this.Width / 2, this.Y + this.Height / 2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertBetween(int value, int min, int max, [CallerArgumentExpression("value")] string name = "")
    {
        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(name);
        }
    }

    private static void AssertBetween(float value, float min, float max, [CallerArgumentExpression("value")] string name = "")
    {
        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(name);
        }
    }

    public RoomBounds SplitLeft(int width, out RoomBounds right)
    {
        AssertBetween(width, 0, this.Width);

        right = new RoomBounds(this.X + width, this.Y, this.Width - width, this.Height);
        return new RoomBounds(this.X, this.Y, width, this.Height);
    }

    public RoomBounds SplitRight(int width, out RoomBounds left)
    {
        AssertBetween(width, 0, this.Width);

        left = new RoomBounds(this.X, this.Y, width, this.Height);
        return new RoomBounds(this.X + width, this.Y, this.Width - width, this.Height);
    }

    public RoomBounds SplitTop(int height, out RoomBounds bottom)
    {
        AssertBetween(height, 0, this.Height);

        bottom = new RoomBounds(this.X, this.Y + height, this.Width, this.Height - height);
        return new RoomBounds(this.X, this.Y, this.Width, height);
    }

    public RoomBounds SplitBottom(int height, out RoomBounds top)
    {
        AssertBetween(height, 0, this.Height);

        top = new RoomBounds(this.X, this.Y, this.Width, height);
        return new RoomBounds(this.X, this.Y + height, this.Width, this.Height - height);
    }

    public RoomBounds Deflate(int left, int top, int right, int bottom)
    {
        AssertBetween(left, 0, this.Width);
        AssertBetween(top, 0, this.Height);
        AssertBetween(right, 0, this.Width);
        AssertBetween(bottom, 0, this.Height);

        return new RoomBounds(this.X + left, this.Y + top, this.Width - left - right, this.Height - top - bottom);
    }

    public RoomBounds Deflate(int leftRight, int topBottom)
    {
        return this.Deflate(leftRight, topBottom, leftRight, topBottom);
    }

    public RoomBounds Deflate(int all)
    {
        return this.Deflate(all, all, all, all);
    }

    public RoomBounds Deflate(Vector2Int leftTop, Vector2Int rightBottom)
    {
        return this.Deflate(leftTop.X, leftTop.Y, rightBottom.X, rightBottom.Y);
    }

    public bool Contains(Vector2Int point)
    {
        return point.X >= this.X && point.X < this.X + this.Width
            && point.Y >= this.Y && point.Y < this.Y + this.Height;
    }

    public bool Contains(RoomBounds bounds)
    {
        return this.Contains(bounds.Position) && this.Contains(bounds.Position + bounds.Size);
    }

    public bool Intersects(RoomBounds bounds)
    {
        return this.X < bounds.X + bounds.Width && this.X + this.Width > bounds.X
            && this.Y < bounds.Y + bounds.Height && this.Y + this.Height > bounds.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is RoomBounds bounds && this == bounds;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.X, this.Y, this.Width, this.Height);
    }

    public static bool operator ==(RoomBounds left, RoomBounds right)
    {
        return left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;
    }

    public static bool operator !=(RoomBounds left, RoomBounds right)
    {
        return !(left == right);
    }

    public RoomBounds SplitRatioLeft(int left, int right, float ratio, out RoomBounds rightBounds)
    {
        AssertBetween(left, 0, this.Width);
        AssertBetween(right, 0, this.Width);
        AssertBetween(ratio, 0f, 1f);

        int width = this.Width - left - right;
        int split = (int)MathF.Round(width * ratio);
        rightBounds = new RoomBounds(this.X + left + split, this.Y, width - split, this.Height);
        return new RoomBounds(this.X + left, this.Y, split, this.Height);
    }

    public RoomBounds SplitRatioRight(int left, int right, float ratio, out RoomBounds leftBounds)
    {
        AssertBetween(left, 0, this.Width);
        AssertBetween(right, 0, this.Width);
        AssertBetween(ratio, 0f, 1f);

        int width = this.Width - left - right;
        int split = (int)MathF.Round(width * ratio);
        leftBounds = new RoomBounds(this.X + left, this.Y, split, this.Height);
        return new RoomBounds(this.X + left + split, this.Y, width - split, this.Height);
    }

    public RoomBounds SplitRatioTop(int top, int bottom, float ratio, out RoomBounds bottomBounds)
    {
        AssertBetween(top, 0, this.Height);
        AssertBetween(bottom, 0, this.Height);
        AssertBetween(ratio, 0f, 1f);

        int height = this.Height - top - bottom;
        int split = (int)MathF.Round(height * ratio);
        bottomBounds = new RoomBounds(this.X, this.Y + top + split, this.Width, height - split);
        return new RoomBounds(this.X, this.Y + top, this.Width, split);
    }

    public RoomBounds SplitRatioBottom(int top, int bottom, float ratio, out RoomBounds topBounds)
    {
        AssertBetween(top, 0, this.Height);
        AssertBetween(bottom, 0, this.Height);
        AssertBetween(ratio, 0f, 1f);

        int height = this.Height - top - bottom;
        int split = (int)MathF.Round(height * ratio);
        topBounds = new RoomBounds(this.X, this.Y + top, this.Width, split);
        return new RoomBounds(this.X, this.Y + top + split, this.Width, height - split);
    }

    public RoomBounds SplitRatioLeft(int leftRight, float ratio, out RoomBounds rightBounds)
    {
        return this.SplitRatioLeft(leftRight, leftRight, ratio, out rightBounds);
    }

    public RoomBounds SplitRatioRight(int leftRight, float ratio, out RoomBounds leftBounds)
    {
        return this.SplitRatioRight(leftRight, leftRight, ratio, out leftBounds);
    }

    public RoomBounds SplitRatioTop(int topBottom, float ratio, out RoomBounds bottomBounds)
    {
        return this.SplitRatioTop(topBottom, topBottom, ratio, out bottomBounds);
    }

    public RoomBounds SplitRatioBottom(int topBottom, float ratio, out RoomBounds topBounds)
    {
        return this.SplitRatioBottom(topBottom, topBottom, ratio, out topBounds);
    }
}
