namespace Architectus;

/// <summary>
/// Represents a room in a house.
/// </summary>
public class Room
{
    /// <summary>
    /// Gets or sets the room's type.
    /// </summary>
    public RoomType Type { get; } = RoomType.Garden;

    /// <summary>
    /// Gets the bounding box of the room.
    /// </summary>
    public Rect2Int BoundingBox { get; private set; }

    private readonly List<Vector2Int> _cells = new();

    /// <summary>
    /// Gets a list of cells that are part of the room.
    /// </summary>
    public IEnumerable<Vector2Int> Cells => this._cells;

    /// <summary>
    /// Initializes a new instance of the <see cref="Room"/> class.
    /// </summary>
    /// <param name="type">The room's type.</param>
    public Room(RoomType type)
    {
        this.Type = type;
    }

    /// <summary>
    /// Adds the given rectangle to the room.
    /// </summary>
    /// <param name="position">The position of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    public void AddRectangle(Vector2Int position, Vector2Int size)
    {
        if (this._cells.Count == 0)
        {
            this.BoundingBox = new Rect2Int(position, size);
        }
        else
        {
            this.BoundingBox = this.BoundingBox.Union(new Rect2Int(position, size));
        }

        for (var x = position.X; x < position.X + size.X; x++)
        {
            for (var y = position.Y; y < position.Y + size.Y; y++)
            {
                this._cells.Add(new Vector2Int(x, y));
            }
        }
    }
}
