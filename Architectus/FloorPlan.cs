namespace Architectus;

public enum RoomType : byte
{
    Garden,
    Bedroom,
    Kitchen,
    LivingRoom,
}

public class HousePlan
{
    private readonly List<FloorPlan> _floors = new();

    /// <summary>
    /// Gets a list of floors.
    /// </summary>
    public IReadOnlyList<FloorPlan> Floors => this._floors;

    /// <summary>
    /// Gets the size of the house.
    /// </summary>
    public Vector2Int Size { get; }

    /// <summary>
    /// Gets or sets the entrance position.
    /// </summary>
    public Vector2Int EntrancePosition { get; set; } = new Vector2Int(0, 0);

    /// <summary>
    /// Initializes a new instance of the <see cref="HousePlan"/> class.
    /// </summary>
    /// <param name="size">The size of the house.</param>
    public HousePlan(Vector2Int size)
    {
        this.Size = size;
    }

    /// <summary>
    /// Adds a floor to the house.
    /// </summary>
    /// <returns>The floor that was added.</returns>
    internal FloorPlan AddFloor()
    {
        var floor = new FloorPlan(this, this._floors.Count);
        this._floors.Add(floor);
        return floor;
    }
}

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

    public Grid? Grid { get; }

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

public class FloorPlan
{
    private readonly Room?[,] _roomsMap;

    private readonly HashSet<Room> _rooms = new();

    /// <summary>
    /// Gets the house that owns this floor.
    /// </summary>
    public HousePlan House { get; }

    /// <summary>
    /// Gets the floor's number. (zero-based)
    /// </summary>
    public int FloorNumber { get; }

    /// <summary>
    /// Gets the size of the floor.
    /// </summary>
    public Vector2Int Size => this.House.Size;

    /// <summary>
    /// Initializes a new instance of the <see cref="FloorPlan"/> class.
    /// </summary>
    /// <param name="house">The house that owns this floor.</param>
    /// <param name="floorNumber">The floor's number.</param>
    public FloorPlan(HousePlan house, int floorNumber)
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
    /// <param name="room">The room to add.</param>
    public void AddRoom(Room room)
    {
        this._rooms.Add(room);
        foreach (var cell in room.Cells)
        {
            this._roomsMap[cell.X, cell.Y] = room;
        }
    }

    /// <summary>
    /// Assigns the tiles in the given rectangle to the given room.
    /// </summary>
    /// <param name="room">The room to assign to.</param>
    /// <param name="position">The position of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <exception cref="ArgumentException">Thrown if the rectangle is out of bounds.</exception>
    /// <exception cref="InvalidOperationException">Thrown if some cell is already assigned to a room.</exception>
    internal void AssignTilesToRoom(Room room, Vector2Int position, Vector2Int size)
    {
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
