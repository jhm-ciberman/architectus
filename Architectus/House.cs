namespace Architectus;

public class HouseLot
{
    private readonly List<Floor> _floors = new();

    /// <summary>
    /// Gets a list of floors.
    /// </summary>
    public IReadOnlyList<Floor> Floors => this._floors;

    public Floor GroundFloor => this._floors[0];

    /// <summary>
    /// Gets the size of the house.
    /// </summary>
    public Vector2Int Size { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HouseLot"/> class.
    /// </summary>
    /// <param name="size">The size of the house.</param>
    public HouseLot(Vector2Int size)
    {
        this.Size = size;

        var ground = new Floor(this, 0);
        this._floors.Add(ground);
    }

    /// <summary>
    /// Gets a floor by its number.
    /// </summary>
    /// <param name="floorNumber">The floor's number.</param>
    /// <returns>The floor.</returns>
    public Floor GetFloor(int floorNumber)
    {
        return this._floors[floorNumber];
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