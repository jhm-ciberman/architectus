namespace Architectus;

public class FloorPlan
{
    private readonly Room?[,] _roomsMap;

    private readonly HashSet<Room> _rooms = new();

    /// <summary>
    /// Gets the size of the floor.
    /// </summary>
    public Vector2Int Size { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FloorPlan"/> class.
    /// </summary>
    /// <param name="size">The floor plan's size.</param>
    public FloorPlan(Vector2Int size)
    {
        this.Size = size;
        this._roomsMap = new Room[size.X, size.Y];
    }

    /// <summary>
    /// Gets a list of all rooms on the floor.
    /// </summary>
    public IEnumerable<Room> Rooms => this._rooms;

    /// <summary>
    /// Gets the count of rooms on the floor.
    /// </summary>
    public int RoomCount => this._rooms.Count;

    /// <summary>
    /// Assigns all the cells in the given rectangle to the given room.
    /// </summary>
    /// <param name="room">The room to assign to.</param>
    /// <param name="position">The position of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <exception cref="ArgumentException">Thrown if the rectangle is out of bounds.</exception>
    /// <exception cref="InvalidOperationException">Thrown if some cell is already assigned to a room.</exception>
    public void AssignRoom(Room room, Vector2Int position, Vector2Int size)
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

        this._rooms.Add(room);
    }

    /// <summary>
    /// Gets the room at the given cell or null if the cell is empty.
    /// </summary>
    /// <param name="position">The position of the cell.</param>
    /// <returns>The room at the given cell.</returns>
    /// <exception cref="ArgumentException">Thrown if the cell is out of bounds.</exception>
    public Room? GetRoom(Vector2Int position)
    {
        if (position.X < 0 || position.X >= this.Size.X || position.Y < 0 || position.Y >= this.Size.Y)
        {
            throw new ArgumentException("The cell is out of bounds.", nameof(position));
        }

        return this._roomsMap[position.X, position.Y];
    }
}
