namespace Architectus;

public struct Thickness
{
    public int Left { get; set; }
    public int Right { get; set; }
    public int Top { get; set; }
    public int Bottom { get; set; }
    public Vector2Int Total => new Vector2Int(Left + Right, Top + Bottom);
    public Vector2Int TopLeft => new Vector2Int(Left, Top);
    public Vector2Int TopRight => new Vector2Int(Right, Top);
    public Vector2Int BottomLeft => new Vector2Int(Left, Bottom);
    public Vector2Int BottomRight => new Vector2Int(Right, Bottom);

    public Thickness(int left, int right, int top, int bottom)
    {
        this.Left = left;
        this.Right = right;
        this.Top = top;
        this.Bottom = bottom;
    }

    public Thickness(int all)
    {
        this.Left = all;
        this.Right = all;
        this.Top = all;
        this.Bottom = all;
    }
}
