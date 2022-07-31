namespace Architectus;

/// <summary>
/// Represents a cardinal direction.
/// </summary>
public enum CardinalDirection : byte
{
    North = 0,
    East = 1,
    South = 2,
    West = 3,
}

/// <summary>
/// Provides extension methods for <see cref="CardinalDirection"/>.
/// </summary>
public static class CardinalDirectionExtensions
{
    /// <summary>
    /// Gets the oposite direction of the given direction. (i.e. North -> South, East -> West)
    /// </summary>
    /// <param name="direction">The direction to get the oposite of.</param>
    /// <returns>The oposite direction.</returns>
    /// <exception cref="ArgumentException">Thrown if the direction is not a cardinal direction.</exception>
    public static CardinalDirection Opposite(this CardinalDirection direction)
    {
        return direction switch
        {
            CardinalDirection.North => CardinalDirection.South,
            CardinalDirection.East => CardinalDirection.West,
            CardinalDirection.South => CardinalDirection.North,
            CardinalDirection.West => CardinalDirection.East,
            _ => throw new ArgumentException("Invalid direction", nameof(direction)),
        };
    }

    /// <summary>
    /// Gets the vector representation of the given direction. (i.e. North -> (0, 1), East -> (1, 0))
    /// </summary>
    /// <param name="direction">The direction to get the vector representation of.</param>
    /// <returns>The vector representation of the direction.</returns>
    /// <exception cref="ArgumentException">Thrown if the direction is not a cardinal direction.</exception>
    public static Vector2Int ToVector2Int(this CardinalDirection direction)
    {
        return direction switch
        {
            CardinalDirection.North => new Vector2Int(0, 1),
            CardinalDirection.East => new Vector2Int(1, 0),
            CardinalDirection.South => new Vector2Int(0, -1),
            CardinalDirection.West => new Vector2Int(-1, 0),
            _ => throw new ArgumentException("Invalid direction", nameof(direction)),
        };
    }
}