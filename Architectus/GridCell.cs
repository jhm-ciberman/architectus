namespace Architectus;


public class GridCell
{
    /// <summary>
    /// Gets the grid that this cell belongs to.
    /// </summary>
    public Grid Grid { get; }

    /// <summary>
    /// Gets the coorinate of the cell. This is the row and column index of the cell.
    /// </summary>
    public Vector2Int Coordinate { get; }

    /// <summary>
    /// Gets the position of the cell relative to the top-left corner of the grid. (in tiles)
    /// </summary>
    public Vector2Int Position { get; }

    /// <summary>
    /// Gets the size of the cell (in tiles).
    /// </summary>
    public Vector2Int Size { get; }

    /// <summary>
    /// Gets the area of the cell.
    /// </summary>
    public int Area { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GridCell"/> class.
    /// </summary>
    /// <param name="grid">The grid that this cell belongs to.</param>
    /// <param name="coordinate">The coordinate of the cell.</param>
    /// <param name="position">The position of the cell relative to the top-left corner of the grid.</param>
    /// <param name="size">The size of the cell (in tiles).</param>
    public GridCell(Grid grid, Vector2Int coordinate, Vector2Int position, Vector2Int size)
    {
        this.Grid = grid;
        this.Coordinate = coordinate;
        this.Position = position;
        this.Size = size;
        this.Area = size.X * size.Y;
    }

    /// <summary>
    /// Gets the north neighbor of the cell.
    /// </summary>
    public GridCell? NorthNeighbor => this.Grid.GetCell(new Vector2Int(this.Coordinate.X, this.Coordinate.Y - 1));

    /// <summary>
    /// Gets the south neighbor of the cell.
    /// </summary>
    public GridCell? SouthNeighbor => this.Grid.GetCell(new Vector2Int(this.Coordinate.X, this.Coordinate.Y + 1));

    /// <summary>
    /// Gets the east neighbor of the cell.
    /// </summary>
    public GridCell? EastNeighbor => this.Grid.GetCell(new Vector2Int(this.Coordinate.X + 1, this.Coordinate.Y));

    /// <summary>
    /// Gets the west neighbor of the cell.
    /// </summary>
    public GridCell? WestNeighbor => this.Grid.GetCell(new Vector2Int(this.Coordinate.X - 1, this.Coordinate.Y));

    /// <summary>
    /// Gets all neighbors of the cell.
    /// </summary>
    public IEnumerable<GridCell> Neighbors
    {
        get
        {
            var north = this.NorthNeighbor;
            if (north != null) yield return north;
            
            var south = this.SouthNeighbor;
            if (south != null) yield return south;

            var east = this.EastNeighbor;
            if (east != null) yield return east;

            var west = this.WestNeighbor;
            if (west != null) yield return west;
        }
    }
}


