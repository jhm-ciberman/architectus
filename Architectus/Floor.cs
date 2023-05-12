using System;
using System.Collections.Generic;

namespace Architectus;

public class Floor
{
    private readonly Room?[,] _roomsMap;

    private readonly HashSet<Room> _rooms = new();

    /// <summary>
    /// Gets the house that owns this floor.
    /// </summary>
    public House House { get; }

    /// <summary>
    /// Gets the floor's number. (zero-based)
    /// </summary>
    public int FloorNumber { get; }

    /// <summary>
    /// Gets the size of the floor.
    /// </summary>
    public Vector2Int Size => this.House.Size;

    /// <summary>
    /// Initializes a new instance of the <see cref="Floor"/> class.
    /// </summary>
    /// <param name="house">The house that owns this floor.</param>
    /// <param name="floorNumber">The floor's number.</param>
    public Floor(House house, int floorNumber)
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
