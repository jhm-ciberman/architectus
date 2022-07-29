using System.Diagnostics.CodeAnalysis;

namespace Architectus;
public enum RoomType
{
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
    public RoomType Type { get; set; } = RoomType.Empty;

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

    public HouseGenerator(Random? random = null)
    {
        this._random = random ?? Random.Shared;
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

    private RoomType[] _rooms = new[]
    {
        RoomType.Empty,
        RoomType.Bedroom,
        RoomType.Kitchen,
        RoomType.LivingRoom,
    };

    private RoomType NextRoomType()
    {
        return this._rooms[this._random.Next(this._rooms.Length)];
    }

    private bool TryGenerate([NotNullWhen(true)] out HousePlan? house)
    {
        house = null;
        var maxHouseArea = this.PlotSize - Vector2Int.One;
        var minHouseArea = new Vector2Int(maxHouseArea.X * 0.8f, maxHouseArea.Y * 0.8f);
        var paddingGenerator = new PaddingGenerator(this._random)
        {
            RectangleSize = this.PlotSize,
            MinThicknessX = 1,
            MaxThicknessX = (int)(this.PlotSize.X * 0.6f), // 80% of the plot width.
            MinThicknessY = 1,
            MaxThicknessY = (int)(this.PlotSize.Y * 0.6f), // 80% of the plot height.
            MinContentArea = minHouseArea.X * minHouseArea.Y, // The minimum area of the house.
        };

        if (!paddingGenerator.TryGeneratePadding(out var padding)) return false;

        var gridGenerator = new GridGenerator(this._random)
        {
            MinCellArea = 5,
            GridSize = this.PlotSize - padding.Total,
        };

        if (! gridGenerator.TryGenerateGrid(out Grid? grid)) return false;

        house = new HousePlan(this.PlotSize);
        var floor = house.AddFloor();

        var topLeft = padding.TopLeft;
        foreach (var cell in grid.GetCells())
        {
            // for now, assign a random room type to each cell.
            var room = new Room(this.NextRoomType());
            floor.AssignRoom(room, cell.Position + topLeft, cell.Size);
        }

        Console.WriteLine($"Average aspect ratio: {grid.AverageAspectRatio}");
        return true;
    }
}