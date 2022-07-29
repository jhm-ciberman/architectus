using System.Diagnostics;

namespace Architectus;

public class Grid
{
    private readonly GridCell[] _cells;

    /// <summary>
    /// Gets the number of columns and rows in the grid.
    /// </summary>
    public Vector2Int Size { get; }

    /// <summary>
    /// Gets the average aspect ratio of the cells. The number is always >= 1
    /// </summary>
    public float AverageAspectRatio { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Grid"/> class.
    /// </summary>
    /// <param name="columnsWidths">The width of each column in the grid.</param>
    /// <param name="rowsHeights">The height of each row in the grid.</param>
    public Grid(int[] columnsWidths, int[] rowsHeights)
    {
        this.Size = new Vector2Int(columnsWidths.Length, rowsHeights.Length);
        this._cells = new GridCell[columnsWidths.Length * rowsHeights.Length];

        Vector2Int position = Vector2Int.Zero;
        for (int row = 0; row < rowsHeights.Length; row++)
        {
            for (int col = 0; col < columnsWidths.Length; col++)
            {
                var cell = new GridCell(new Vector2Int(col, row), position, new Vector2Int(columnsWidths[col], rowsHeights[row]));
                this._cells[row * columnsWidths.Length + col] = cell;
                position.X += columnsWidths[col];
            }
            position.X = 0;
            position.Y += rowsHeights[row];
        }

        float totalAspectRatio = 0;
        for (int i = 0; i < this._cells.Length; i++)
        {
            totalAspectRatio += this._cells[i].Size.X / (float)this._cells[i].Size.Y;
        }
        totalAspectRatio /= this._cells.Length;
        if (totalAspectRatio < 1)
        {
            totalAspectRatio = 1 / totalAspectRatio;
        }
        this.AverageAspectRatio = totalAspectRatio;
    }

    /// <summary>
    /// Gets the cell at the specified grid coordinate.
    /// </summary>
    /// <param name="coordinate">The grid coordinate.</param>
    /// <returns>The cell at the specified grid coordinate.</returns>
    public GridCell GetCell(Vector2Int coordinate)
    {
        return this._cells[coordinate.X + coordinate.Y * this.Size.X];
    }

    /// <summary>
    /// Gets the cell at the specified grid coordinate.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>The cell at the specified grid coordinate.</returns>
    public GridCell GetCell(int x, int y)
    {
        return this._cells[x + y * this.Size.X];
    }

    public GridCell this[Vector2Int position] => this.GetCell(position);

    /// <summary>
    /// Gets all the cells in the grid.
    /// </summary>
    /// <returns>All the cells in the grid.</returns>
    public IEnumerable<GridCell> GetCells()
    {
        return this._cells;
    }
}


