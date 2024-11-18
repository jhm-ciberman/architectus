using System.Collections.Generic;
using Architectus.Support;
using LifeSim.Support.Numerics;

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
    /// Gets the <see cref="RectInt"/> that contains all cells of the room.
    /// </summary>
    public RectInt Bounds { get; private set; }

    private readonly HashSet<Vector2Int> _cells = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Room"/> class.
    /// </summary>
    /// <param name="floor">The floor that owns this room.</param>
    /// <param name="type">The room's type.</param>
    /// <param name="bounds">The <see cref="RectInt"/> that contains all cells of the room.</param>
    public Room(Floor floor, RoomType type, RectInt bounds)
    {
        this.Floor = floor;
        this.Type = type;
        this.Bounds = bounds;

        for (int x = bounds.X; x < bounds.X + bounds.Width; x++)
        {
            for (int y = bounds.Y; y < bounds.Y + bounds.Height; y++)
            {
                this._cells.Add(new Vector2Int(x, y));
            }
        }
    }

    /// <summary>
    /// Gets whether the room contains the given position.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>Whether the room contains the given position.</returns>
    public bool Contains(Vector2Int position)
    {
        return this._cells.Contains(position);
    }

    /// <summary>
    /// Removes the given rectangle from the room cells.
    /// </summary>
    /// <param name="bounds">The rectangle to remove.</param>
    public void Carve(RectInt bounds)
    {
        for (int x = bounds.X; x < bounds.X + bounds.Width; x++)
        {
            for (int y = bounds.Y; y < bounds.Y + bounds.Height; y++)
            {
                this._cells.Remove(new Vector2Int(x, y));
            }
        }
    }
}
