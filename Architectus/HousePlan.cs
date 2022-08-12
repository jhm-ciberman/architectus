using System.Diagnostics.CodeAnalysis;

namespace Architectus;



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

    private bool TryGenerate([NotNullWhen(true)] out HousePlan? house)
    {
        house = null;

        var entrancePlacer = new EntrancePlacer(this._random)
        {
            PlotSize = this.PlotSize,
            PlotDirection = this.PlotDirection,
        };

        Vector2Int entrancePosition = entrancePlacer.GenerateEntrancePosition();

        var maxHouseArea = this.PlotSize - Vector2Int.One;
        var minHouseArea = new Vector2Int((int)(maxHouseArea.X * 0.8f), (int)(maxHouseArea.Y * 0.8f));
        var paddingGenerator = new PaddingGenerator(this._random)
        {
            RectangleSize = this.PlotSize,
            MinThicknessX = 1,
            MaxThicknessX = (int)(this.PlotSize.X * 0.6f), // 60% of the plot width.
            MinThicknessY = 1,
            MaxThicknessY = (int)(this.PlotSize.Y * 0.6f), // 60% of the plot height.
            MinContentArea = minHouseArea.X * minHouseArea.Y, // The minimum area of the house.
        };

        if (!paddingGenerator.TryGeneratePadding(out var padding)) return false;

        var gridGenerator = new GridGenerator(this._random)
        {
            MinCellArea = 4,
            GridSize = this.PlotSize - padding.Total,
            GridPosition = padding.TopLeft,
            MinCellsCount = 8,
        };

        if (! gridGenerator.TryGenerateGrid(out Grid? grid)) return false;


        var roomsPlacer = new RoomsPlacer(this._random)
        {
            EntrancePosition = entrancePosition,
            RoomRequests =
            {
                new(RoomType.Bedroom, 10, 80),
                new(RoomType.Kitchen, 10, 80),
                new(RoomType.LivingRoom, 10, 80),
            },
        };

        if (!roomsPlacer.TryPlaceRooms(grid)) return false;
        
        house = new HousePlan(this.PlotSize);
        var floor = house.AddFloor();
        floor.Entrance = entrancePosition;
        foreach (var roomRequest in roomsPlacer.RoomRequests)
        {
            var room = new Room(roomRequest.Type);
            foreach (var cells in roomRequest.AssignedCells)
            {
                room.AddRectangle(cells.Position, cells.Size);
            }
            floor.AddRoom(room);
        }

        Console.WriteLine($"Average aspect ratio: {grid.AverageAspectRatio}");
        return true;
    }
}