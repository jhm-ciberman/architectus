namespace Architectus;

/// <summary>
/// Represents a room in a house.
/// </summary>
public class Room
{
    /// <summary>
    /// Gets the floor that owns this room.
    /// </summary>
    public Floor Floor { get; }

    /// <summary>
    /// Gets or sets the room's type.
    /// </summary>
    public RoomType Type { get; } = RoomType.Garden;

    /// <summary>
    /// Gets the <see cref="RoomBounds"/> that contains all cells of the room.
    /// </summary>
    public RoomBounds Bounds { get; set; }

    private readonly List<RoomBounds> _carvings = new();

    /// <summary>
    /// Gets a list of all carvings in the room.
    /// </summary>
    public IReadOnlyList<RoomBounds> Carvings => this._carvings;

    /// <summary>
    /// Initializes a new instance of the <see cref="Room"/> class.
    /// </summary>
    /// <param name="floor">The floor that owns this room.</param>
    /// <param name="type">The room's type.</param>
    /// <param name="bounds">The <see cref="RoomBounds"/> that contains all cells of the room.</param>
    public Room(Floor floor, RoomType type, RoomBounds bounds)
    {
        this.Floor = floor;
        this.Type = type;
        this.Bounds = bounds;
    }

    /// <summary>
    /// Gets whether the room contains the given position.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>Whether the room contains the given position.</returns>
    public bool Contains(Vector2Int position)
    {
        return this.Bounds.Contains(position);
    }
    
}
