namespace Architectus;
public enum RoomType
{
    Undefined = 0,
    Empty,
    Bedroom,
    Kitchen,
    LivingRoom,
}



public class HousePlan
{
    private readonly List<FloorPlan> _floors = new();
    public IReadOnlyList<FloorPlan> Floors => this._floors;

    public Vector2Int Size { get; }

    public HousePlan(Vector2Int size)
    {
        this.Size = size;
    }

    internal FloorPlan AddFloor()
    {
        var floor = new FloorPlan(this.Size);
        this._floors.Add(floor);
        return floor;
    }
}

public enum CardinalDirection
{
    North,
    East,
    South,
    West,
}

public enum Rotation
{
    None = 0,
    Rotate90 = 1,
    Rotate180 = 2,
    Rotate270 = 3,
}

[Flags]
public enum Mirror
{
    None = 0,
    Horizontal = 1,
    Vertical = 2,
}



/// <summary>
/// Represents a room in a house.
/// </summary>
public class Room
{
    private static int _nextId = 0;

    public int Id { get; }

    /// <summary>
    /// Gets or sets the room's type.
    /// </summary>
    public RoomType Type { get; set; } = RoomType.Undefined;

    /// <summary>
    /// Initializes a new instance of the <see cref="Room"/> class.
    /// </summary>
    /// <param name="type">The room's type.</param>
    public Room(RoomType type)
    {
        this.Id = _nextId++;
        this.Type = type;
    }
}


public class HouseGenerator
{
    private readonly Random _random;

    public Vector2Int PlotSize { get; set; } = new Vector2Int(10, 10);

    public CardinalDirection PlotDirection { get; } // The direction the plot entrance is facing.

    public HouseGenerator(Random random)
    {
        this._random = random;
    }

    public HouseGenerator() : this(new Random())
    {
    }

    public HousePlan Generate()
    {
        int numberOfAttempts = 0;

        if (this.PlotSize.X < 3 || this.PlotSize.Y < 3)
        {
            throw new ArgumentException("Plot size must be at least 3x3.");
        }

        while (numberOfAttempts < 3)
        {
            if (this.TryGenerate(out var house))
            {
                return house;
            }
        }

        throw new InvalidOperationException("Failed to generate a house.");
    }

    private bool TryGenerate(out HousePlan house)
    {
        var template = new GridTemplate
        {
            Columns =
            {
                new ColumnDefinition("*"),
                new ColumnDefinition("5"),
                new ColumnDefinition("*")
            },
            Rows =
            {
                new RowDefinition("*"),
                new RowDefinition("*"),
            },
            Mappings =
            {
                0, 1, 1,
                2, 2, 0,
            },
            Elements =
            {
                new GridElement(RoomType.Empty),
                new GridElement(RoomType.LivingRoom), // Non-empty terminal
                new GridElement(RoomType.Kitchen),
            },
        };

        house = new HousePlan(this.PlotSize);
        var floor = house.AddFloor();

        if (template.CanHandle(floor.Size))
        {
            template.Handle(floor.Size, floor);
        }
        else
        {
            throw new InvalidOperationException("Failed to generate a house.");
        }

        return true;
    }
}