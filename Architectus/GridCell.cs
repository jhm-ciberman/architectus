namespace Architectus;


public class GridCell
{
    /// <summary>
    /// Gets the coorinate of the cell. This is the row and column index of the cell.
    /// </summary>
    public Vector2Int Coordinate { get; }

    /// <summary>
    /// Gets the position of the cell relative to the top-left corner of the grid. (in tiles)
    /// </summary>
    public Vector2Int Position { get; }

    /// <summary>
    /// Gets or sets the size of the cell (in tiles).
    /// </summary>
    public Vector2Int Size { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GridCell"/> class.
    /// </summary>
    /// <param name="coordinate">The coordinate of the cell.</param>
    /// <param name="position">The position of the cell relative to the top-left corner of the grid.</param>
    /// <param name="size">The size of the cell (in tiles).</param>
    public GridCell(Vector2Int coordinate, Vector2Int position, Vector2Int size)
    {
        this.Coordinate = coordinate;
        this.Position = position;
        this.Size = size;
    }
}


