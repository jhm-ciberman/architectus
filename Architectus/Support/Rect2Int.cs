namespace Architectus;

public struct Rect2Int
{
    public Vector2Int Position { get; set; }
    public Vector2Int Size { get; set; }
    public Vector2 Center => new Vector2(this.Position.X + this.Size.X / 2, this.Position.Y + this.Size.Y / 2);

    public static Rect2Int Zero { get; } = new Rect2Int(Vector2Int.Zero, Vector2Int.Zero);

    public Rect2Int(Vector2Int position, Vector2Int size)
    {
        this.Position = position;
        this.Size = size;
    }

    public Rect2Int Union(Rect2Int other)
    {
        var min = Vector2Int.Min(this.Position, other.Position);
        var max = Vector2Int.Max(this.Position + this.Size, other.Position + other.Size);
        return new Rect2Int(min, max - min);
    }
}
