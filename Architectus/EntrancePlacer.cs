namespace Architectus;

/// <summary>
/// The <see cref="EntrancePlacer"/> class is used to generate the position of the main entrance of a house.
/// </summary>
public class EntrancePlacer
{
    private readonly Random _random;

    /// <summary>
    /// Gets or sets the direction that the plot is facing.
    /// </summary>
    public CardinalDirection PlotDirection { get; set; }

    /// <summary>
    /// Gets or sets the size of the plot.
    /// </summary>
    public Vector2Int PlotSize { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntrancePlacer"/> class.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    public EntrancePlacer(Random? random = null)
    {
        this._random = random ?? Random.Shared;
    }

    /// <summary>
    /// Generates the entrance position.
    /// </summary>
    public Vector2Int GenerateEntrancePosition()
    {
        // we need to choose a random position located in the lateral wall of the plot
        // according to the direction the plot is facing.

        var size = this.PlotSize;
        return this.PlotDirection switch
        {
            CardinalDirection.North => new Vector2Int(this._random.Next(size.X), 0),
            CardinalDirection.East => new Vector2Int(size.X - 1, this._random.Next(size.Y)),
            CardinalDirection.South => new Vector2Int(this._random.Next(size.X), size.Y - 1),
            CardinalDirection.West => new Vector2Int(0, this._random.Next(size.Y)),
            _ => throw new InvalidOperationException("Invalid plot direction."),
        };
    }
}