namespace Architectus;

public class Floor
{
    private readonly Room?[,] _roomsMap;

    private readonly HashSet<Room> _rooms = new();

    /// <summary>
    /// Gets the house that owns this floor.
    /// </summary>
    public HouseLot House { get; }

    /// <summary>
    /// Gets the floor's number. (zero-based)
    /// </summary>
    public int FloorNumber { get; }

    /// <summary>
    /// Gets the size of the floor.
    /// </summary>
    public Vector2Int Size => this.House.Size;


    public RoomBounds Bounds => new RoomBounds(Vector2Int.Zero, this.Size);

    /// <summary>
    /// Initializes a new instance of the <see cref="Floor"/> class.
    /// </summary>
    /// <param name="house">The house that owns this floor.</param>
    /// <param name="floorNumber">The floor's number.</param>
    public Floor(HouseLot house, int floorNumber)
    {
        this.House = house;
        this.FloorNumber = floorNumber;
        this._roomsMap = new Room?[this.Size.X, this.Size.Y];
    }

    /// <summary>
    /// Gets a list of all rooms on the floor.
    /// </summary>
    public IEnumerable<Room> Rooms => this._rooms;

    /// <summary>
    /// Gets the count of rooms on the floor.
    /// </summary>
    public int RoomCount => this._rooms.Count;

    public Vector2Int Entrance { get; set; } = new Vector2Int(0, 0);

    /// <summary>
    /// Gets a room at the given position or null if there is no room at the given position.
    /// </summary>
    /// <param name="position">The position in the floor.</param>
    /// <returns>The room at the given position.</returns>
    public Room? GetRoom(Vector2Int position)
    {
        if (position.X < 0 || position.X >= this.Size.X || position.Y < 0 || position.Y >= this.Size.Y)
        {
            return null;
        }
        return this._roomsMap[position.X, position.Y];
    }

    /// <summary>
    /// Adds a room to the floor.
    /// </summary>
    public Room AddRoom(RoomBounds bounds, RoomType type)
    {
        var room = new Room(this, type, bounds);
        this.AssignRectangle(room, bounds);
        this._rooms.Add(room);
        return room;
    }

    public bool RectIsEmpty(RoomBounds bounds)
    {
        for (var x = bounds.Position.X; x < bounds.Position.X + bounds.Size.X; x++)
        {
            for (var y = bounds.Position.Y; y < bounds.Position.Y + bounds.Size.Y; y++)
            {
                if (this._roomsMap[x, y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void AssignRectangle(Room room, RoomBounds area)
    {
        var position = area.Position;
        var size = area.Size;
        if (position.X < 0 || position.X + size.X > this.Size.X || position.Y < 0 || position.Y + size.Y > this.Size.Y)
        {
            throw new ArgumentException("The rectangle is out of bounds.", nameof(position));
        }

        for (var x = position.X; x < position.X + size.X; x++)
        {
            for (var y = position.Y; y < position.Y + size.Y; y++)
            {
                if (this._roomsMap[x, y] != null)
                {
                    throw new InvalidOperationException($"The cell at {x}, {y} is already assigned to a room.");
                }
            }
        }

        for (var x = position.X; x < position.X + size.X; x++)
        {
            for (var y = position.Y; y < position.Y + size.Y; y++)
            {
                this._roomsMap[x, y] = room;
            }
        }
    }
}
