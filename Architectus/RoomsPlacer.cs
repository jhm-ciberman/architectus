using System.Diagnostics.CodeAnalysis;

namespace Architectus;

public class RoomsPlacer
{
    /// <summary>
    /// Gets or sets a list of room requests. The rooms placer will try to place the rooms in the grid.
    /// </summary>
    public IList<RoomRequest> RoomRequests { get; set; } = new List<RoomRequest>();

    /// <summary>
    /// Gets or sets the entrance position.
    /// </summary>
    public Vector2Int EntrancePosition { get; set; } = new Vector2Int(0, 0);

    /// <summary>
    /// Gets or sets the maximum number of attempts.
    /// </summary>
    public int MaximumAttempts { get; set; } = 100;

    private readonly Random _random;

    private record struct RoomCandidate(RoomRequest Request, GridCell Cell);

    private readonly WeightedRandom<RoomCandidate> _wrRoomCandidates;

    private readonly WeightedRandom<CardinalDirection> _wrGrowDirections;

    private class CellInfo
    {
        public GridCell Cell { get; set; }
        public float DistanceToEntrance { get; set; } // Normalized
        public bool IsConnectedToExterior { get; set; }

        public CellInfo(GridCell cell, float distanceToEntrance, bool isConnectedToExterior)
        {
            this.Cell = cell;
            this.DistanceToEntrance = distanceToEntrance;
            this.IsConnectedToExterior = isConnectedToExterior;
        }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="RoomsPlacer"/> class.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    public RoomsPlacer(Random? random = null)
    {
        this._random = random ?? Random.Shared;
        this._wrRoomCandidates = new WeightedRandom<RoomCandidate>(this._random);
        this._wrGrowDirections = new WeightedRandom<CardinalDirection>(this._random);
    }


    /// <summary>
    /// Try to place the rooms in the grid.
    /// </summary>
    /// <param name="grid">The grid use as a base for the placement.</param>
    /// <returns>True if the rooms were placed, false otherwise.</returns>
    public bool TryPlaceRooms(Grid grid)
    {
        int attempts = 0;
        while (attempts < this.MaximumAttempts)
        {
            attempts++;
            if (this.PlaceRoomsCore(grid))
            {
                return true;
            }
        }

        return false;
    }

    private float CellDistToEntrance(GridCell cell)
    {
        Vector2Int p = this.EntrancePosition;
        p = Vector2Int.Clamp(p, cell.Position, cell.Position + cell.Size - Vector2Int.One);
        return Vector2Int.ManhattanDistance(p, cell.Position); // TODO: test with euclidean distance
    }

    private static bool IsCellConnectedToExterior(GridCell cell)
    {
        // We need to test in all 4 directions.
        return cell.NorthNeighbor != null
            || cell.EastNeighbor != null
            || cell.SouthNeighbor != null
            || cell.WestNeighbor != null;
    }

    private static float ScoreRoom(RoomRequest request, CellInfo cell)
    {
        // less missing area means a better score.
        float areaScore = request.MinArea - cell.Cell.Area;

        // If window score is a function of if it's connected to the exterior.
        float windowScore = request.Type switch
        {
            RoomType.Bedroom => cell.IsConnectedToExterior ? 1.0f : 0.0f,
            RoomType.Kitchen => cell.IsConnectedToExterior ? 0.5f : 0.0f,
            RoomType.LivingRoom => cell.IsConnectedToExterior ? 0.5f : 0.5f,
            _ => 0.0f,
        };

        // The social score is a function of the distance to the entrance.
        float socialScore = request.Type switch
        {
            RoomType.Bedroom => cell.DistanceToEntrance, // We want to place the bedroom as far as possible from the entrance. 
            RoomType.Kitchen => 0.5f, 
            RoomType.LivingRoom => 1.0f - cell.DistanceToEntrance, // We want to place the living room as close as possible to the entrance.
            _ => 0.0f,
        };

        // The score is the avg of the three factors.
        return (areaScore + windowScore + socialScore) / 3.0f;
    }

    private List<CellInfo> CollectInitialCandidates(WorkGrid workGrid)
    {
        // First we get the info for each cell in the grid.
        var candidates = new List<CellInfo>();
        float maxDistance = 0;
        foreach (var cell in workGrid.Grid.Cells)
        {
            if (workGrid.GetAssignedRoom(cell) != null) continue;
            var info = new CellInfo(cell, this.CellDistToEntrance(cell), IsCellConnectedToExterior(cell));
            candidates.Add(info);
            maxDistance = MathF.Max(maxDistance, info.DistanceToEntrance);
        }

        // Now, normalize the distances
        foreach (var info in candidates)
        {
            info.DistanceToEntrance /= maxDistance;
        }

        return candidates;
    }

    private class WorkGrid
    {
        public readonly Dictionary<GridCell, GrowingRoom> _cellsToRooms = new();

        public Grid Grid { get; }

        public WorkGrid(Grid grid)
        {
            this.Grid = grid;
        }

        public void SetRoom(GridCell cell, GrowingRoom room)
        {
            this._cellsToRooms[cell] = room;
        }

        public GrowingRoom? GetAssignedRoom(GridCell cell)
        {
            return this._cellsToRooms.TryGetValue(cell, out var room) ? room : null;
        }
    }

    private class GrowingRoom
    {
        private static readonly List<GridCell> _tempCellsGrid = new List<GridCell>();

        public RoomRequest Request { get; }
        private readonly List<GridCell> _cells = new();
        public IReadOnlyList<GridCell> Cells => this._cells;
        public int Area { get; private set; } = 0;
        public Rect2Int Bounds { get; private set; } = Rect2Int.Zero;
        private readonly WorkGrid _workGrid;
        public GrowingRoom(WorkGrid workGrid, RoomRequest request)
        {
            this._workGrid = workGrid;
            this.Request = request;
        }

        public void AddCell(GridCell cell)
        {
            if (this._workGrid.GetAssignedRoom(cell) != null)
            {
                throw new InvalidOperationException("Cell already belongs to a room.");
            }

            this._cells.Add(cell);
            this._workGrid.SetRoom(cell, this);
            this.Area += cell.Area;
            var cellBounds = new Rect2Int(cell.Coordinate, cell.Size);
            this.Bounds = this.Bounds.Union(cellBounds);
        }

        private float GetSquariness(int extraArea)
        {
            float boundsArea = this.Bounds.Size.X * this.Bounds.Size.Y;
            return (this.Area + extraArea) / boundsArea;
        }

        // Grow oportunities scores:
        // For a cardinal direction (N, E, S, W), we want to know if we can grow in that direction.
        // Example, for north direction:
        // Get all available cells at the north edge of the room. (They can be in an irregular shape.)
        // - If all cells are occupied, the score is 0 and there is no oportunity to grow in North direction.
        // - Then we sum the extra area to grow, and calculate the new squariness. If the squariness
        // is above the squariness threshold, we can't grow in North direction and the score is 0.
        // - If the newArea is below the minArea, the score is:
        //    = (maxArea - newArea) / minArea
        // - If the newArea is between the minArea and maxArea, the score is:
        //    = (maxArea - newArea) / (maxArea - minArea)
        // - If the newArea is above the maxArea, the score is 0 and there is no oportunity to grow in North direction.
        public float GetGrowScore(CardinalDirection direction)
        {
            _tempCellsGrid.Clear();
            this.GetAvailableCells(direction, _tempCellsGrid);
            if (_tempCellsGrid.Count == 0)
            {
                return 0.0f;
            }
            int extraArea = _tempCellsGrid.Sum(c => c.Area);
            float newSquariness = this.GetSquariness(extraArea);
            if (newSquariness > this.Request.SquarenessThredshold)
            {
                return 0.0f;
            }
            float newArea = this.Area + extraArea;
            if (newArea < this.Request.MinArea)
            {
                return (this.Request.MaxArea - newArea) / this.Request.MinArea;
            }
            if (newArea > this.Request.MaxArea)
            {
                return 0.0f;
            }
            return (this.Request.MaxArea - newArea) / (this.Request.MaxArea - this.Request.MinArea);
        }

        // Get all available cells at a given grow direction. We need to check the cells at the edge of the room.
        private void GetAvailableCells(CardinalDirection direction, List<GridCell> list)
        {
            Vector2Int pos = this.Bounds.Position;
            Vector2Int size = this.Bounds.Size;
            switch (direction)
            {
                case CardinalDirection.North:
                    this.ScanHorizontal(pos.X, pos.Y - 1, size.X, +1, list);
                    break;
                case CardinalDirection.South:
                    this.ScanHorizontal(pos.X, pos.Y + size.Y, size.X, -1, list);
                    break;
                case CardinalDirection.West:
                    this.ScanVertical(pos.X - 1, pos.Y, size.Y, -1, list);
                    break;
                case CardinalDirection.East:
                    this.ScanVertical(pos.X + size.X, pos.Y, size.Y, +1, list);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private void ScanHorizontal(int xStart, int yStart, int width, int inwardsDir, List<GridCell> list)
        {
            for (int x = xStart; x < xStart + width; x++)
            {
                int y = yStart;
                GridCell? prevCell = null;
                while (true)
                {
                    var cell = this._workGrid.Grid.GetCell(x, y);
                    if (cell == null) break; // There is no available cell to expand in that direction.

                    // We can expand in that direction if the cell is not assigned to a room.
                    var room = this._workGrid.GetAssignedRoom(cell);
                    if (room != null)
                    {
                        if (prevCell != null) list.Add(prevCell);
                        break;
                    }

                    y += inwardsDir;
                    prevCell = cell;
                }
            }
        }

        private void ScanVertical(int xStart, int yStart, int height, int inwardsDir, List<GridCell> list)
        {
            for (int y = yStart; y < yStart + height; y++)
            {
                int x = xStart;
                GridCell? prevCell = null;
                while (true)
                {
                    var cell = this._workGrid.Grid.GetCell(x, y);
                    if (cell == null) break; // There is no available cell to expand in that direction.

                    // We can expand in that direction if the cell is not assigned to a room.
                    var room = this._workGrid.GetAssignedRoom(cell);
                    if (room != null)
                    {
                        if (prevCell != null) list.Add(prevCell);
                        break;
                    }

                    x += inwardsDir;
                    prevCell = cell;
                }
            }
        }

        public void Grow(CardinalDirection direction)
        {
            _tempCellsGrid.Clear();
            this.GetAvailableCells(direction, _tempCellsGrid);
            foreach (var cell in _tempCellsGrid)
            {
                this.AddCell(cell);
            }
        }
    }

    private bool PlaceRoomsCore(Grid grid)
    {
        // Each kind of room will have a different chriteria for scoring a cell.
        WorkGrid workGrid = new WorkGrid(grid);
        List<GrowingRoom> rooms = new();
        rooms.Sort((a, b) => b.Request.Priority.CompareTo(a.Request.Priority));
        foreach (var request in this.RoomRequests)
        {
            var candidates = this.CollectInitialCandidates(workGrid);
            this._wrRoomCandidates.Clear();
            foreach (var info in candidates)
            {
                float score = ScoreRoom(request, info);
                score = MathF.Max(0.01f, score); // We don't want to have a score of 0.
                this._wrRoomCandidates.Add(new RoomCandidate(request, info.Cell), score);
            }

            if (this._wrRoomCandidates.Count == 0)
            {
                return false;
            }
            var candidate = this._wrRoomCandidates.Next();
            var room = new GrowingRoom(workGrid, candidate.Request);
            room.AddCell(candidate.Cell);
            rooms.Add(room);
        }

        // Now we need to grow each room fully until the MinArea requirement is met.

        foreach (var room in rooms)
        {
            while (room.Area < room.Request.MinArea)
            {
                var wr = this._wrGrowDirections;
                wr.Clear();
                wr.Add(CardinalDirection.North, room.GetGrowScore(CardinalDirection.North));
                wr.Add(CardinalDirection.East, room.GetGrowScore(CardinalDirection.East));
                wr.Add(CardinalDirection.South, room.GetGrowScore(CardinalDirection.South));
                wr.Add(CardinalDirection.West, room.GetGrowScore(CardinalDirection.West));

                if (wr.Count == 0)
                {
                    return false;
                }
                var direction = wr.Next();
                room.Grow(direction);
            }
        }

        bool roomsExpanded = true;
        while (roomsExpanded)
        {
            roomsExpanded = false;
            foreach (var room in rooms)
            {
                if (room.Area < room.Request.MaxArea)
                {
                    var wr = this._wrGrowDirections;
                    wr.Clear();
                    wr.Add(CardinalDirection.North, room.GetGrowScore(CardinalDirection.North));
                    wr.Add(CardinalDirection.East, room.GetGrowScore(CardinalDirection.East));
                    wr.Add(CardinalDirection.South, room.GetGrowScore(CardinalDirection.South));
                    wr.Add(CardinalDirection.West, room.GetGrowScore(CardinalDirection.West));

                    if (wr.Count != 0)
                    {
                        var direction = wr.Next();
                        room.Grow(direction);
                        roomsExpanded = true;
                    }
                }
            }
        }

        // All okay, copy the results to the RoomRequests.
        foreach (var room in rooms)
        {
            room.Request.AssignedCells = room.Cells;
        }
        
        return true;
    }
}