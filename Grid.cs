namespace Architectus;

public enum GridUnitType
{ 
    Meters,
    Star
}

/// <summary>
/// Expreses a grid column's width or grid column's height. It could be in absolute world units
/// or relative weights (Star)
/// </summary>
public struct GridSize
{
    public GridUnitType Unit { get; }
    public int Value { get; }

    public GridSize(GridUnitType unit, int value)
    {
        this.Unit = unit;
        this.Value = value;
    }

    public static GridSize Star(int value = 1) => new(GridUnitType.Star, value);
    public static GridSize Meters(int value) => new(GridUnitType.Meters, value);

    public bool IsStar => this.Unit == GridUnitType.Star;

    public static GridSize FromString(string value)
    {
        if (value.EndsWith("*"))
        {
            return value.Length == 1 
                ? GridSize.Star(1) 
                : GridSize.Star(int.Parse(value[..^1]));
        }
        else
        {
            return GridSize.Meters(int.Parse(value));
        }
    }
}

public interface IGridDimension
{
    GridSize Size { get; }

    int MinSize { get; }

    int? MaxSize { get; }
}

/// <summary>
/// Defines a grid column.
/// </summary>
public class ColumnDefinition : IGridDimension
{
    /// <summary>
    /// Gets or sets the column width.
    /// </summary>
    public GridSize Width { get; }

    /// <summary>
    /// Gets or sets the column's minimum width.
    /// </summary>
    public int MinWidth { get; } = 1;

    /// <summary>
    /// Gets or sets the column's maximum width.
    /// </summary>
    public int? MaxWidth { get; } = null;

    GridSize IGridDimension.Size => this.Width;

    int IGridDimension.MinSize => this.MinWidth;

    int? IGridDimension.MaxSize => this.MaxWidth;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnDefinition"/> class.
    /// </summary>
    /// <param name="width">The column width.</param>
    public ColumnDefinition(GridSize width)
    {
        this.Width = width;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnDefinition"/> class.
    /// </summary>
    /// <param name="width">The column width.</param>
    public ColumnDefinition(string width) : this(GridSize.FromString(width))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnDefinition"/> class.
    /// </summary>
    /// <param name="width">The column width.</param>
    /// <param name="minWidth">The column's minimum width.</param>
    /// <param name="maxWidth">The column's maximum width.</param>
    public ColumnDefinition(GridSize width, int minWidth, int maxWidth)
    {
        this.Width = width;
        this.MinWidth = minWidth;
        this.MaxWidth = maxWidth;
    }
}

/// <summary>
/// Defines a grid row.
/// </summary>
public class RowDefinition : IGridDimension
{
    /// <summary>
    /// Gets or sets the row height.
    /// </summary>
    public GridSize Height { get; }

    /// <summary>
    /// Gets or sets the row's minimum height.
    /// </summary>
    public int MinHeight { get; } = 1;

    /// <summary>
    /// Gets or sets the row's maximum height.
    /// </summary>
    public int? MaxHeight { get; } = null;

    GridSize IGridDimension.Size => this.Height;

    int IGridDimension.MinSize => this.MinHeight;

    int? IGridDimension.MaxSize => this.MaxHeight;

    /// <summary>
    /// Initializes a new instance of the <see cref="RowDefinition"/> class.
    /// </summary>
    /// <param name="height">The row height.</param>
    public RowDefinition(GridSize height)
    {
        this.Height = height;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RowDefinition"/> class.
    /// </summary>
    /// <param name="height">The row height.</param>
    public RowDefinition(string height) : this(GridSize.FromString(height))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RowDefinition"/> class.
    /// </summary>
    /// <param name="height">The row height.</param>
    /// <param name="minHeight">The row's minimum height.</param>
    /// <param name="maxHeight">The row's maximum height.</param>
    public RowDefinition(GridSize height, int minHeight, int maxHeight)
    {
        this.Height = height;
        this.MinHeight = minHeight;
        this.MaxHeight = maxHeight;
    }
}

/// <summary>
/// Defines a room element.
/// </summary>
public class GridElement
{
    /// <summary>
    /// Gets or sets the room type.
    /// </summary>
    public RoomType Type { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GridElement"/> class.
    /// </summary>
    /// <param name="type">The room type.</param>
    public GridElement(RoomType type)
    {
        this.Type = type;
    }
}

public interface ITemplate
{
    /// <summary>
    /// Returns whether this template can handle the given requirements.
    /// </summary>
    /// <param name="size">The size of the working area.</param>
    /// <returns>True if the template can handle the given requirements, false otherwise.</returns>
    bool CanHandle(Vector2Int size);

    void Handle(Vector2Int size, FloorPlan plan);
}

/// <summary>
/// Declares a grid template for building rooms based on a grid layout.
/// </summary>
public class GridTemplate
{
    /// <summary>
    /// Gets or sets the grid columns.
    /// </summary>
    public List<ColumnDefinition> Columns { get; } = new();

    /// <summary>
    /// Gets or sets the grid rows.
    /// </summary>
    public List<RowDefinition> Rows { get; } = new();

    /// <summary>
    /// Gets or sets the grid mappings. This maps the grid cells to the elements by their index.
    /// For example, if the grid has 3 columns and 2 rows, and the grid mappings should be an array
    /// of 6 elements. The first 3 elements will indicate the mappings for the first row, and the
    /// last 3 elements will indicate the mappings for the second row. 
    /// </summary>
    public List<int> Mappings { get; } = new();

    /// <summary>
    /// Gets or sets the grid elements. This is an array of elements that will be placed in the grid
    /// according to the mappings array.
    /// </summary>
    public List<GridElement> Elements { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="GridTemplate"/> class.
    /// </summary>
    public GridTemplate()
    {
    }


    public bool CanHandle(Vector2Int size)
    {
        var minWidth = GetMin(this.Columns);
        var minHeight = GetMin(this.Rows);
        var maxWidth = GetMax(this.Columns);
        var maxHeight = GetMax(this.Rows);

        return size.X >= minWidth && size.Y >= minHeight && size.X <= maxWidth && size.Y <= maxHeight;
    }

    private static int GetMin(IReadOnlyList<IGridDimension> dimensions)
    {
        int min = 0;
        foreach (var dimension in dimensions)
        {
            min += dimension.MinSize;
        }
        return min;
    }

    private static int GetMax(IReadOnlyList<IGridDimension> dimensions)
    {
        int max = 0;
        foreach (var dimension in dimensions)
        {
            if (dimension.MaxSize.HasValue)
            {
                max += dimension.MaxSize.Value;
            }
            else
            {
                return int.MaxValue;
            }
        }
        return max;
    }

    private static int[] ComputeDimensionSizes(IReadOnlyList<IGridDimension> dimensions, int totalSize)
    {
        // Two passes need to be made, first all the fixed sizes are assigned, and then the remaining
        // size is distributed among the variable width columns.
        int remainingSize = totalSize;
        int starCount = 0;
        int[] sizes = new int[dimensions.Count];
        for (int i = 0; i < dimensions.Count; i++)
        {
            var column = dimensions[i];
            if (column.Size.IsStar)
            {
                starCount++;
            }
            else
            {
                remainingSize -= column.Size.Value;
                sizes[i] = column.Size.Value;
            }
        }

        if (starCount == 0) return sizes;

        int starSize = remainingSize / starCount;
        int leftOver = remainingSize % starCount;
        for (int i = 0; i < dimensions.Count; i++)
        {
            var column = dimensions[i];
            if (column.Size.IsStar)
            {
                sizes[i] = starSize * column.Size.Value;
                if (leftOver > 0)
                {
                    sizes[i] += 1;
                    leftOver--;
                }
            }
        }

        return sizes;
    }


    public void Handle(Vector2Int size, FloorPlan plan)
    {
        // Get the sizes of the columns and rows.
        var columnSizes = ComputeDimensionSizes(this.Columns, size.X);
        var rowSizes = ComputeDimensionSizes(this.Rows, size.Y);

        // Create a dictionary of rooms by their index.
        // I use a simple list because it's faster than a dictionary.
        var rooms = new Room[this.Elements.Count];
        for (int i = 0; i < this.Elements.Count; i++)
        {
            rooms[i] = new Room(this.Elements[i].Type);
        }

        // Create a grid of rooms using the mapping array.
        var grid = new Room[rowSizes.Length * columnSizes.Length];
        for (int i = 0; i < this.Mappings.Count; i++)
        {
            var index = this.Mappings[i];
            grid[i] = rooms[index];
        }

        // Imprint the floor plan.
        var gridSize = new Vector2Int(columnSizes.Length, rowSizes.Length);
        var gridOrigin = new Vector2Int(0, 0);

        Vector2Int cellPosition = gridOrigin;
        for (int y = 0; y < gridSize.Y; y++)
        {
            cellPosition.X = 0;
            for (int x = 0; x < gridSize.X; x++)
            {
                var room = grid[y * gridSize.X + x];
                if (room != null)
                {
                    Console.WriteLine($"=> Grid cell ({x}, {y})");
                    var roomPosition = cellPosition + gridOrigin;
                    var roomSize = new Vector2Int(columnSizes[x], rowSizes[y]);
                    Console.WriteLine($"Room position: {roomPosition}");
                    Console.WriteLine($"Room size: {roomSize}");
                    Console.WriteLine($"Room type: {room.Type}");
                    plan.AssignRoom(room, roomPosition, roomSize);
                }
                cellPosition.X += columnSizes[x];
            }
            cellPosition.Y += rowSizes[y];
        }
    }
}


