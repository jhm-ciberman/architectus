namespace Architectus;

public class RoomRequest
{
    /// <summary>
    /// Gets or sets the type of the room.
    /// </summary>
    public RoomType Type { get; }

    /// <summary>
    /// Gets or sets the minimum area of the room.
    /// </summary>
    public int MinArea { get; }

    /// <summary>
    /// Gets or sets the maximum area of the room.
    /// </summary>
    public int MaxArea { get; }

    /// <summary>
    /// Gets or sets the priority of the room. A higher priority means the room will be first when taking 
    /// turns to take space in the grid.
    /// </summary>
    public float Priority { get; }

    /// <summary>
    /// Gets or sets a value that indicates how square the room should be.
    /// A value of 1.0 means the room should be as square as possible while a value of 0.0 means the room
    /// could take any shape.
    /// The squareness of a room is calculated as the ratio of the area of the filled room area
    /// to the area of the bounding box of the room.
    /// </summary>
    public float SquarenessThredshold { get; } = 0.75f;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoomRequest"/> class.
    /// </summary>
    /// <param name="type">The type of the room.</param>
    /// <param name="minArea">The minimum area of the room.</param>
    /// <param name="maxArea">The maximum area of the room.</param>
    public RoomRequest(RoomType type, int minArea, int maxArea)
    {
        this.Type = type;
        this.MinArea = minArea;
        this.MaxArea = maxArea;

        this.Priority = type switch
        {
            RoomType.Bedroom => 10,
            RoomType.Kitchen => 5,
            RoomType.LivingRoom => 1,
            _ => 0,
        };
    }

    public IEnumerable<GridCell> AssignedCells { get; set; } = Enumerable.Empty<GridCell>();
}
