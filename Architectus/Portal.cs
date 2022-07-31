namespace Architectus;

/// <summary>
/// A portal is a connection between two rooms. Portals are bidirectional.
/// </summary>
public interface IPortal
{
    /// <summary>
    /// Gets or sets the floor index in which the portal is located.
    /// For portals that span multiple floors, (i.e. stairs), this is the floor index of the lowest floor.
    /// </summary>
    public int Floor { get; }

    /// <summary>
    /// Gets or sets the direction the portal is facing.
    /// </summary>
    public CardinalDirection Direction { get; }
}

/// <summary>
/// Represents a door in a house.
/// </summary>
public class DoorPortal : IPortal
{
    /// <summary>
    /// Gets the end position of the door.
    /// </summary>
    public Vector2Int Position { get; }

    /// <summary>
    /// Gets the floor index in which the door is located.
    /// </summary>
    public int Floor { get; }

    /// <summary>
    /// Gets the direction the door is facing.
    /// </summary>
    public CardinalDirection Direction { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoorPortal"/> class.
    /// </summary>
    /// <param name="position">The position of the door.</param>
    /// <param name="floor">The floor index in which the door is located.</param>
    /// <param name="direction">The direction the door is facing.</param>
    public DoorPortal(Vector2Int position, int floor, CardinalDirection direction)
    {
        this.Position = position;
        this.Floor = floor;
        this.Direction = direction;
    }
}

/// <summary>
/// Represents an staircase in a house.
/// </summary>
public class StaircasePortal : IPortal
{
    /// <summary>
    /// Gets the lenght in tiles of the staircase.
    /// This is the number of tiles between the start position and the end position
    /// in a straight line, determined by the <see cref="Direction"/>.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Gets the start position of the staircase.
    /// </summary>
    public Vector2Int PositionStart { get; }

    /// <summary>
    /// Gets the end position of the staircase.
    /// </summary>
    public Vector2Int PositionEnd { get; }

    /// <summary>
    /// Gets the floor index in which the staircase is located.
    /// This is the floor index of the lowest floor.
    /// </summary>
    public int Floor { get; }

    /// <summary>
    /// Gets the direction the staircase is facing.
    /// </summary>
    public CardinalDirection Direction { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StaircasePortal"/> class.
    /// </summary>
    /// <param name="position">The position of the staircase.</param>
    /// <param name="floor">The lowest floor the staircase is on.</param>
    /// <param name="direction">The direction the staircase is facing.</param>
    public StaircasePortal(Vector2Int position, int floor, CardinalDirection direction)
    {
        this.PositionStart = position;
        this.Floor = floor;
        this.Direction = direction;

        this.Length = 4;
        this.PositionEnd = position + direction.ToVector2Int() * this.Length;
    }
}

/// <summary>
/// An edge portal determines the connection between two rooms
/// when there is no wall between them. If multiple wall tiles should be "demolished"
/// then multiple <see cref="EdgePortal"/> objects need to be created, one for each
/// wall tile that should be demolished.
/// </summary>
public class EdgePortal : IPortal
{
    public Vector2Int PositionStart { get; }

    public Vector2Int PositionEnd { get; }

    public int Floor { get; }

    public CardinalDirection Direction { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EdgePortal"/> class.
    /// </summary>
    /// <param name="position">The position of the open wall.</param>
    /// <param name="floor">The lowest floor the open wall is on.</param>
    /// <param name="direction">The direction the open wall is facing.</param>
    public EdgePortal(Vector2Int position, int floor, CardinalDirection direction)
    {
        this.PositionStart = position;
        this.Floor = floor;
        this.Direction = direction;
        this.PositionEnd = position + direction.ToVector2Int();
    }
}