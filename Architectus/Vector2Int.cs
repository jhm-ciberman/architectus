namespace Architectus;

public struct Vector2Int
{
    public int X { get; set; }
    public int Y { get; set; }
    public static Vector2Int Zero { get; } = new Vector2Int(0, 0);
    public static Vector2Int One { get; } = new Vector2Int(1, 1);

    public Vector2Int(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.X + b.X, a.Y + b.Y);
    }

    public static Vector2Int operator -(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.X - b.X, a.Y - b.Y);
    }

    public override string ToString()
    {
        return $"({this.X}, {this.Y})";
    }
}

public struct Rect2Int
{
    public Vector2Int Position { get; set; }
    public Vector2Int Size { get; set; }

    public Rect2Int(Vector2Int position, Vector2Int size)
    {
        this.Position = position;
        this.Size = size;
    }
}
