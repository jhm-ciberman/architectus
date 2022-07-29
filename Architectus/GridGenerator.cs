using System.Diagnostics.CodeAnalysis;

namespace Architectus;

// The algorithm for generating the house will be the following:
// - First a random grid of WxH cells is generated using the GridGenerator where W >= H.
// This is to simplify the grid generation. (The following steps will rotate the grid if necessary. )
// The generator uses the following parameters:
//    - Width, Height: Determine the exact size of the grid (in tiles). 
//    - MinCellSize, MaxCellSize: (Vector2Int) Determine the minimum and maximum size of each cell.
//    - MinCellArea, MaxCellArea: Optional. Determine the minimum and maximum area of each cell. If null the values are not checked.
//    - MinCells, MaxCells: Optional. Determine the minimum and maximum number of cells in the grid. If null the values are not checked.
//    - MaxAttempts: Determines the maximum number of attempts to generate the grid. If the grid cannot 
//      be generated after this number of attempts, the algorithm will fail. (Default: 100)
//    - Sampler: (Uniform or Gaussian) Determine the distribution used when sampling random numbers.
//    - Random: The random number generator used to sample random numbers.
// The generator will attempt to generate a random grid of cells until it finds a valid grid. (TryGenerateGrid)

/// <summary>
/// Generates a random grid of cells.
/// </summary>
public class GridGenerator
{
    /// <summary>
    /// Gets or sets the minimum size of each cell. (in tiles)
    /// </summary>
    public Vector2Int MinCellSize { get; set; } = new Vector2Int(1, 1);

    /// <summary>
    /// Gets or sets the maximum size of each cell. (in tiles)
    /// </summary>
    public Vector2Int MaxCellSize { get; set; } = new Vector2Int(6, 6);

    /// <summary>
    /// Gets or sets the minimum area of each cell. (in tiles)
    /// </summary>
    public int? MinCellArea { get; set; } = null;

    /// <summary>
    /// Gets or sets the maximum area of each cell. (in tiles)
    /// </summary>
    public int? MaxCellArea { get; set; } = null;

    /// <summary>
    /// Gets or sets the minimum number of cells in the grid.
    /// </summary>
    public int? MinCellsCount { get; set; } = null;

    /// <summary>
    /// Gets or sets the maximum number of cells in the grid.
    /// </summary>
    public int? MaxCellsCount { get; set; } = null;

    /// <summary>
    /// Gets or sets the minimin required number of rows.
    /// </summary>
    public int? MinRowsCount { get; set; } = 1;

    /// <summary>
    /// Gets or sets the maximum required number of rows.
    /// </summary>
    public int? MaxRowsCount { get; set; } = null;

    /// <summary>
    /// Gets or sets the minimum required number of columns.
    /// </summary>
    public int? MinColumnsCount { get; set; } = 1;

    /// <summary>
    /// Gets or sets the maximum required number of columns.
    /// </summary>
    public int? MaxColumnsCount { get; set; } = null;

    /// <summary>
    /// Gets or sets the minimum number of attempts to generate a valid grid.
    /// </summary>
    public int MaxAttempts { get; set; } = 100;

    /// <summary>
    /// Gets or sets the size of the grid. (in tiles)
    /// This is NOT the number of row/columns. This is the size of the grid in tiles.
    /// </summary>
    public Vector2Int GridSize { get; set; } = new Vector2Int(100, 100);

    /// <summary>
    /// Gets or sets the sampler to use when sampling the number of grid columns and rows.
    /// </summary>
    public ISampler GridCountSampler { get; set; } = GaussianSampler.Instance;

    /// <summary>
    /// Gets or sets the sampler to use when sampling the size of each cell.
    /// </summary>
    public ISampler CellSizeSampler { get; set; } = GaussianSampler.Instance;

    /// <summary>
    /// Gets or sets the random number generator to use.
    /// </summary>
    public Random Random { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GridGenerator"/> class.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    public GridGenerator(Random? random = null)
    {
        this.Random = random ?? Random.Shared;
    }

    public bool TryGenerateGrid([NotNullWhen(true)] out Grid? grid)
    {
        grid = null;
        var attempts = 0;
        while (grid == null && attempts < this.MaxAttempts)
        {
            if (this.TryGenerateRowsAndCols(out int[]? rowsSizes, out int[]? columnsSizes))
            {
                grid = new Grid(columnsSizes, rowsSizes);
                return true;
            }
            attempts++;
        }

        grid = null;
        return false;
    }

    private int SampleGridCount(int min, int max)
    {
        float value = this.GridCountSampler.Sample(this.Random, min, max);
        return (int)MathF.Round(value);
    }

    private int SampleCellSize(int min, int max)
    {
        float value = this.CellSizeSampler.Sample(this.Random, min, max);
        return (int)MathF.Round(value);
    }

    private bool TryGenerateRowsAndCols([NotNullWhen(true)] out int[]? rowsSizes, [NotNullWhen(true)] out int[]? columnsSizes)
    {
        int maxColumns = (int)MathF.Floor((float)this.GridSize.X / (float)this.MinCellSize.X);
        int minColumns = (int)MathF.Ceiling((float)this.GridSize.X / (float)this.MaxCellSize.X);
        int maxRows = (int)MathF.Floor((float)this.GridSize.Y / (float)this.MinCellSize.Y);
        int minRows = (int)MathF.Ceiling((float)this.GridSize.Y / (float)this.MaxCellSize.Y);

        columnsSizes = null;
        rowsSizes = null;

        int colCount = this.SampleGridCount(minColumns, maxColumns);
        int rowCount = this.SampleGridCount(minRows, maxRows);

        if (this.MinColumnsCount != null && colCount < this.MinColumnsCount) return false;
        if (this.MaxColumnsCount != null && colCount > this.MaxColumnsCount) return false;


        int totalCellsCount = colCount * rowCount;
        if (this.MinCellsCount != null && totalCellsCount < this.MinCellsCount) return false;
        if (this.MaxCellsCount != null && totalCellsCount > this.MaxCellsCount) return false;

        columnsSizes = this.CreateSizesArray(colCount, total: this.GridSize.X, this.MinCellSize.X, this.MaxCellSize.X);
        rowsSizes = this.CreateSizesArray(rowCount, total: this.GridSize.Y, this.MinCellSize.Y, this.MaxCellSize.Y);

        if (this.MinCellArea != null || this.MaxCellArea != null)
        {
            for (int i = 0; i < rowsSizes.Length; i++)
            {
                for (int j = 0; j < columnsSizes.Length; j++)
                {
                    if (!this.ValidateCell(rowsSizes[i], columnsSizes[j]))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private bool ValidateCell(int rowSize, int columnSize)
    {
        if (this.MinCellArea != null && rowSize * columnSize < this.MinCellArea) return false;
        if (this.MaxCellArea != null && rowSize * columnSize > this.MaxCellArea) return false;
        return true;
    }

    private int[] CreateSizesArray(int count, int total, int min, int max)
    {
        int[] sizes = new int[count];
        int totalSize = 0;
        for (int i = 0; i < count; i++)
        {
            int size = this.SampleCellSize(min, max);
            sizes[i] = size;
            totalSize += size;
        }

        int attempts = 0;

        while (totalSize > total)
        {
            int index = this.Random.Next(count);
            if (sizes[index] > min)
            {
                sizes[index]--;
                totalSize--;
            }

            if (attempts++ > 1000)
            {
                throw new InvalidOperationException("Could not generate a valid grid.");
            }
        }

        attempts = 0;

        while (totalSize < total)
        {
            int index = this.Random.Next(count);
            if (sizes[index] < max)
            {
                sizes[index]++;
                totalSize++;
            }

            if (attempts++ > 1000)
            {
                throw new InvalidOperationException("Could not generate a valid grid.");
            }
        }

        return sizes;
    }
}
