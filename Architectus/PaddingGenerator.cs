namespace Architectus;

/// <summary>
/// Generates a random padding inside a rectangle. The padding is calculated inwards.
/// </summary>
public class PaddingGenerator
{
    /// <summary>
    /// Gets or sets the size of the rectangle.
    /// </summary>
    public Vector2Int RectangleSize { get; set; } = new Vector2Int(100, 100);

    /// <summary>
    /// Gets or sets the sampler to use when sampling the size of the padding.
    /// </summary>
    public ISampler PaddingSampler { get; set; } = UniformSampler.Instance;

    /// <summary>
    /// Gets or sets the random number generator to use.
    /// </summary>
    public Random Random { get; set; }

    /// <summary>
    /// Gets or sets the minimum size of the padding in the X axis.
    /// </summary>
    public int MinThicknessX { get; set; } = 10;

    /// <summary>
    /// Gets or sets the maximum size of the padding in the X axis.
    /// </summary>
    public int MaxThicknessX { get; set; } = 20;

    /// <summary>
    /// Gets or sets the minimum size of the padding in the Y axis.
    /// </summary>
    public int MinThicknessY { get; set; } = 10;

    /// <summary>
    /// Gets or sets the maximum size of the padding in the Y axis.
    /// </summary>
    public int MaxThicknessY { get; set; } = 20;

    /// <summary>
    /// Gets the minimum total area of the inner rectangle.
    /// </summary>
    public int? MinContentArea { get; set; }

    /// <summary>
    /// Gets the maximum total area of the inner rectangle.
    /// </summary>
    public int? MaxContentArea { get; set; }

    /// <summary>
    /// Gets the number of attempts to generate a valid padding.
    /// </summary>
    public int MaxAttempts { get; set; } = 1000;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddingGenerator"/> class.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    public PaddingGenerator(Random? random = null)
    {
        this.Random = random ?? Random.Shared;
    }
    
    /// <summary>
    /// Tries to generate a random padding inside a rectangle.
    /// </summary>
    /// <param name="padding">The generated padding.</param>
    /// <returns>True if a valid padding was generated, false otherwise.</returns>
    public bool TryGeneratePadding(out Thickness padding)
    {
        int attempts = 0;
        int totalRectArea = this.RectangleSize.X * this.RectangleSize.Y;
        while (attempts++ < this.MaxAttempts)
        {
            int left = this.Sample(this.MinThicknessX, this.MaxThicknessX);
            int right = this.Sample(this.MinThicknessX, this.MaxThicknessX);
            if (left + right > this.RectangleSize.X) continue;

            int top = this.Sample(this.MinThicknessY, this.MaxThicknessY);
            int bottom = this.Sample(this.MinThicknessY, this.MaxThicknessY);
            if (top + bottom > this.RectangleSize.Y) continue;

            int innerRectArea = (this.RectangleSize.X - left - right) * (this.RectangleSize.Y - top - bottom);
            if (this.MinContentArea != null && innerRectArea < this.MinContentArea) continue;
            if (this.MaxContentArea != null && innerRectArea > this.MaxContentArea) continue;

            padding = new Thickness(left, right, top, bottom);
            return true;
        }
        padding = default;
        return false;
    }

    private int Sample(int min, int max)
    {
        return (int)MathF.Round(this.PaddingSampler.Sample(this.Random, min, max));
    }
}
