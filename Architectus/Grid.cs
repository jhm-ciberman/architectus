namespace Architectus;

public class Grid
{
    private readonly Dictionary<Vector2Int, GridCell> _cells;

    /// <summary>
    /// Gets the position of the grid in the floor plan.
    /// </summary>
    public Vector2Int Position { get; }

    /// <summary>
    /// Gets the number of columns and rows in the grid.
    /// </summary>
    public Vector2Int SizeInCells { get; }

    /// <summary>
    /// Gets the size of the grid (in the floor plan).
    /// </summary>
    public Vector2Int Size { get; }

    /// <summary>
    /// Gets the average aspect ratio of the cells. The number is always >= 1
    /// </summary>
    public float AverageAspectRatio { get; }

    /// <summary>
    /// Get the column widths.
    /// </summary>
    public int[] ColumnWidths { get; }

    /// <summary>
    /// Gets the row heights.
    /// </summary>
    public int[] RowHeights { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Grid"/> class.
    /// </summary>
    /// <param name="floorPlan">The floor plan that this grid belongs to.</param>
    /// <param name="position">The position of the grid in the floor plan.</param>
    /// <param name="columnsWidths">The width of each column in the grid.</param>
    /// <param name="rowsHeights">The height of each row in the grid.</param>
    public Grid(Vector2Int position, int[] columnsWidths, int[] rowsHeights)
    {
        this.Position = position;
        this.ColumnWidths = columnsWidths;
        this.RowHeights = rowsHeights;
        this.SizeInCells = new Vector2Int(columnsWidths.Length, rowsHeights.Length);
        int cellsCount = columnsWidths.Length * rowsHeights.Length;
        this._cells = new Dictionary<Vector2Int, GridCell>(cellsCount);

        Vector2Int cellPos = position;
        for (int row = 0; row < rowsHeights.Length; row++)
        {
            for (int col = 0; col < columnsWidths.Length; col++)
            {
                var cell = new GridCell(this, new Vector2Int(col, row), cellPos, new Vector2Int(columnsWidths[col], rowsHeights[row]));
                this._cells.Add(cell.Coordinate, cell);
                cellPos.X += columnsWidths[col];
            }
            cellPos.X = position.X;
            cellPos.Y += rowsHeights[row];
        }

        this.Size = new Vector2Int(columnsWidths.Sum(), rowsHeights.Sum());

        float totalAspectRatio = 0;
        foreach (var cell in this._cells.Values)
        {
            totalAspectRatio += cell.Size.X / (float)cell.Size.Y;
        }
        totalAspectRatio /= this._cells.Count;
        if (totalAspectRatio < 1)
        {
            totalAspectRatio = 1 / totalAspectRatio;
        }
        this.AverageAspectRatio = totalAspectRatio;
    }

    /// <summary>
    /// Gets the cell at the specified grid coordinate or null if the coordinate is out of bounds
    /// or there is no cell at the specified coordinate.
    /// </summary>
    /// <param name="coordinate">The grid coordinate.</param>
    /// <returns>The cell at the specified grid coordinate.</returns>
    public GridCell? GetCell(Vector2Int coordinate)
    {
        if (coordinate.X < 0 || coordinate.X >= this.SizeInCells.X || coordinate.Y < 0 || coordinate.Y >= this.SizeInCells.Y)
        {
            return null;
        }

        return this._cells.TryGetValue(coordinate, out var cell) ? cell : null;
    }

    /// <summary>
    /// Gets the cell at the specified grid coordinate or null if the coordinate is out of bounds
    /// or there is no cell at the specified coordinate.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>The cell at the specified grid coordinate.</returns>
    public GridCell? GetCell(int x, int y)
    {
        return this.GetCell(new Vector2Int(x, y));
    }

    /// <summary>
    /// Gets all the cells in the grid.
    /// </summary>
    public IEnumerable<GridCell> Cells => this._cells.Values;

    /// <summary>
    /// Get the number of cells in the grid.
    /// </summary>
    public int CellsCount => this._cells.Count;

    /// <summary>
    /// Removes the cell at the specified grid coordinate.
    /// </summary>
    /// <param name="coordinate">The grid coordinate.</param>
    public void RemoveCell(Vector2Int coordinate)
    {
        this._cells.Remove(coordinate);
    }
}
