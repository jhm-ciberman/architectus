using System.Collections.Generic;

namespace Architectus;

public class House
{
    private readonly List<Floor> _floors = new();

    /// <summary>
    /// Gets a list of floors.
    /// </summary>
    public IReadOnlyList<Floor> Floors => this._floors;

    /// <summary>
    /// Gets the size of the house.
    /// </summary>
    public Vector2Int Size { get; }

    /// <summary>
    /// Gets or sets the entrance position.
    /// </summary>
    public Vector2Int EntrancePosition { get; set; } = new Vector2Int(0, 0);

    /// <summary>
    /// Initializes a new instance of the <see cref="House"/> class.
    /// </summary>
    /// <param name="size">The size of the house.</param>
    public House(Vector2Int size)
    {
        this.Size = size;
    }

    /// <summary>
    /// Adds a floor to the house.
    /// </summary>
    /// <returns>The floor that was added.</returns>
    internal Floor AddFloor()
    {
        var floor = new Floor(this, this._floors.Count);
        this._floors.Add(floor);
        return floor;
    }
}