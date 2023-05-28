using CommunityToolkit.Diagnostics;

namespace Architectus;

public static class HouseHelper
{
    /// <summary>
    /// Deflates the bounds to a random size between minSize and maxSize. The room will be positioned
    /// according to the position strategy. The room, by convention, will be positioned 
    /// prioritizing the right of the bounds because the entrance is assumed to be on the left side of the plot.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="bounds">The bounds of the plot to deflate.</param>
    /// <param name="minSize">The minimum size of the bounds.</param>
    /// <param name="maxSize">The maximum size of the bounds.</param>
    /// <returns>The deflated bounds.</returns>
    public static RoomBounds DeflatePlotToIndoorSize(Random random, RoomBounds bounds, Vector2Int minSize, Vector2Int maxSize)
    {
        Guard.IsBetweenOrEqualTo(minSize.X, 1, maxSize.X, $"{nameof(minSize)}.{nameof(minSize.X)}");
        Guard.IsBetweenOrEqualTo(minSize.Y, 1, maxSize.Y, $"{nameof(minSize)}.{nameof(minSize.Y)}");

        maxSize.X = Math.Min(maxSize.X, bounds.Width);
        maxSize.Y = Math.Min(maxSize.Y, bounds.Height);

        int w = random.Next(minSize.X, maxSize.X + 1);
        int h = random.Next(minSize.Y, maxSize.Y + 1);
        var size = new Vector2Int(w, h);

        var freeSpace = new Vector2Int(
            Math.Max(0, bounds.Size.X - size.X),
            Math.Max(0, bounds.Size.Y - size.Y));

        float? biasX = 0.8f; // TODO: Make this configurable.
        float? biasY = null;

        var percentX = biasX.HasValue
            ? random.NextBiasedGaussianRatio(biasX.Value)
            : random.NextDouble();
        
        var percentY = biasY.HasValue
            ? random.NextBiasedGaussianRatio(biasY.Value)
            : random.NextDouble();

        var x = (int)Math.Round(freeSpace.X * percentX);
        var y = (int)Math.Round(freeSpace.Y * percentY);
        var position = new Vector2Int(x, y) + bounds.Position;

        return new RoomBounds(position, size);
    }
}
