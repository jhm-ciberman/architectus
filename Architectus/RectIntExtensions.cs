using CommunityToolkit.Diagnostics;
using LifeSim.Support.Numerics;

namespace Architectus;

public static class RectIntExtensions
{
    /// <summary>
    /// Split the bounds into two parts, with the left part having a minimum width of <paramref name="minLeft"/> and
    /// the right part having a minimum width of <paramref name="minRight"/>. The ratio of the left part to the right
    /// part is <paramref name="ratio"/> (without the minimum widths).
    /// </summary>
    /// <param name="minLeft">The minimum width of the left part.</param>
    /// <param name="minRight">The minimum width of the right part.</param>
    /// <param name="ratio">The ratio of the left part to the right part.</param>
    /// <param name="rightBounds">The right part.</param>
    /// <returns>The left part.</returns>
    public static RectInt SplitRatioLeft(this RectInt self, int minLeft, int minRight, float ratio, out RectInt rightBounds)
    {
        Guard.IsBetween(minLeft, 0, self.Width);
        Guard.IsBetween(minRight, 0, self.Width);
        Guard.IsBetween(ratio, 0, 1);
        Guard.IsLessThanOrEqualTo(minLeft + minRight, self.Width);

        int leftWidth = (int)(self.Width * ratio);
        int rightWidth = self.Width - leftWidth;

        if (leftWidth < minLeft)
        {
            leftWidth = minLeft;
            rightWidth = self.Width - leftWidth;
        }
        else if (rightWidth < minRight)
        {
            rightWidth = minRight;
            leftWidth = self.Width - rightWidth;
        }

        rightBounds = new RectInt(self.X + leftWidth, self.Y, rightWidth, self.Height);
        return new RectInt(self.X, self.Y, leftWidth, self.Height);
    }

    public static RectInt SplitLeft(this RectInt self, int width, out RectInt right)
    {
        Guard.IsBetween(width, 0, self.Width);

        right = new RectInt(self.X + width, self.Y, self.Width - width, self.Height);
        return new RectInt(self.X, self.Y, width, self.Height);
    }

    public static RectInt SplitTop(this RectInt self, int height, out RectInt bottom)
    {
        Guard.IsBetween(height, 0, self.Height);

        bottom = new RectInt(self.X, self.Y + height, self.Width, self.Height - height);
        return new RectInt(self.X, self.Y, self.Width, height);
    }
}
