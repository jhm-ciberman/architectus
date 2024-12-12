using System;
using System.Collections.Generic;
using System.Linq;
using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

public class StackLayout : ContainerLayout
{
    /// <summary>
    /// Gets or sets the orientation of the layout.
    /// </summary>
    public Orientation Orientation { get; set; } = Orientation.Row;

    /// <inheritdoc/>
    protected override Vector2Int MeasureOverride(Vector2Int availableSize)
    {
        // Measure depends on orientation
        return this.Orientation switch
        {
            Orientation.Row or Orientation.RowReverse => this.MeasureHorizontal(availableSize),
            Orientation.Column or Orientation.ColumnReverse => this.MeasureVertical(availableSize),
            _ => throw new InvalidOperationException(nameof(this.Orientation)),
        };
    }

    /// <inheritdoc/>
    protected override RectInt ArrangeOverride(RectInt finalRect)
    {
        // Arrange depends on orientation
        return this.Orientation switch
        {
            Orientation.Row => this.ArrangeRow(finalRect),
            Orientation.Column => this.ArrangeColumn(finalRect),
            Orientation.RowReverse => this.ArrangeRowReverse(finalRect),
            Orientation.ColumnReverse => this.ArrangeColumnReverse(finalRect),
            _ => throw new InvalidOperationException(nameof(this.Orientation)),
        };
    }

    private Vector2Int MeasureHorizontal(Vector2Int availableSize)
    {
        int totalWidth = 0;

        foreach (var child in this.Children)
        {
            Vector2Int childSize = child.Measure(new Vector2Int(availableSize.X - totalWidth, availableSize.Y));
            totalWidth += childSize.X;
        }

        return new Vector2Int(totalWidth, availableSize.Y);
    }

    private Vector2Int MeasureVertical(Vector2Int availableSize)
    {
        int totalHeight = 0;

        foreach (var child in this.Children)
        {
            Vector2Int childSize = child.Measure(new Vector2Int(availableSize.X, availableSize.Y - totalHeight));
            totalHeight += childSize.Y;
        }

        return new Vector2Int(availableSize.X, totalHeight);
    }

    private RectInt ArrangeRow(RectInt finalRect)
    {
        int currentX = finalRect.X;
        int remainingSpace = finalRect.Width - this.Children.Sum(c => c.DesiredSize.X);
        float totalGrowWeight = this.Children.Sum(c => c.GrowWeight);

        int[] extraWidths = new int[this.Children.Count];
        int extraSpace = remainingSpace;

        for (int i = 0; i < this.Children.Count; i++)
        {
            var child = this.Children[i];
            if (totalGrowWeight > 0 && child.GrowWeight > 0)
            {
                extraWidths[i] = (int)(remainingSpace * (child.GrowWeight / totalGrowWeight));
                extraSpace -= extraWidths[i];
            }
        }

        // Distribute remaining extra space due to rounding
        for (int i = 0; extraSpace > 0 && i < this.Children.Count; i++)
        {
            if (this.Children[i].GrowWeight > 0)
            {
                extraWidths[i]++;
                extraSpace--;
            }
        }

        foreach (var (child, extraWidth) in this.Children.Zip(extraWidths, (c, w) => (c, w)))
        {
            child.Arrange(new RectInt(currentX, finalRect.Y, child.DesiredSize.X + extraWidth, finalRect.Height));
            currentX += child.DesiredSize.X + extraWidth;
        }

        return finalRect;
    }

    private RectInt ArrangeColumn(RectInt finalRect)
    {
        int currentY = finalRect.Y;
        int remainingSpace = finalRect.Height - this.Children.Sum(c => c.DesiredSize.Y);
        float totalGrowWeight = this.Children.Sum(c => c.GrowWeight);

        int[] extraHeights = new int[this.Children.Count];
        int extraSpace = remainingSpace;

        for (int i = 0; i < this.Children.Count; i++)
        {
            var child = this.Children[i];
            if (totalGrowWeight > 0 && child.GrowWeight > 0)
            {
                extraHeights[i] = (int)(remainingSpace * (child.GrowWeight / totalGrowWeight));
                extraSpace -= extraHeights[i];
            }
        }

        // Distribute remaining extra space due to rounding
        for (int i = 0; extraSpace > 0 && i < this.Children.Count; i++)
        {
            if (this.Children[i].GrowWeight > 0)
            {
                extraHeights[i]++;
                extraSpace--;
            }
        }

        foreach (var (child, extraHeight) in this.Children.Zip(extraHeights, (c, h) => (c, h)))
        {
            child.Arrange(new RectInt(finalRect.X, currentY, finalRect.Width, child.DesiredSize.Y + extraHeight));
            currentY += child.DesiredSize.Y + extraHeight;
        }

        return finalRect;
    }

    private RectInt ArrangeRowReverse(RectInt finalRect)
    {
        int currentX = finalRect.X + finalRect.Width;
        int remainingSpace = finalRect.Width - this.Children.Sum(c => c.DesiredSize.X);
        float totalGrowWeight = this.Children.Sum(c => c.GrowWeight);

        int[] extraWidths = new int[this.Children.Count];
        int extraSpace = remainingSpace;

        for (int i = 0; i < this.Children.Count; i++)
        {
            var child = this.Children[i];
            if (totalGrowWeight > 0 && child.GrowWeight > 0)
            {
                extraWidths[i] = (int)(remainingSpace * (child.GrowWeight / totalGrowWeight));
                extraSpace -= extraWidths[i];
            }
        }

        // Distribute remaining extra space due to rounding
        for (int i = 0; extraSpace > 0 && i < this.Children.Count; i++)
        {
            if (this.Children[i].GrowWeight > 0)
            {
                extraWidths[i]++;
                extraSpace--;
            }
        }

        for (int i = this.Children.Count - 1; i >= 0; i--)
        {
            var child = this.Children[i];
            int extraWidth = extraWidths[i];
            currentX -= (child.DesiredSize.X + extraWidth);
            child.Arrange(new RectInt(currentX, finalRect.Y, child.DesiredSize.X + extraWidth, finalRect.Height));
        }

        return finalRect;
    }

    private RectInt ArrangeColumnReverse(RectInt finalRect)
    {
        int currentY = finalRect.Y + finalRect.Height;
        int remainingSpace = finalRect.Height - this.Children.Sum(c => c.DesiredSize.Y);
        float totalGrowWeight = this.Children.Sum(c => c.GrowWeight);

        int[] extraHeights = new int[this.Children.Count];
        int extraSpace = remainingSpace;

        for (int i = 0; i < this.Children.Count; i++)
        {
            var child = this.Children[i];
            if (totalGrowWeight > 0 && child.GrowWeight > 0)
            {
                extraHeights[i] = (int)(remainingSpace * (child.GrowWeight / totalGrowWeight));
                extraSpace -= extraHeights[i];
            }
        }

        // Distribute remaining extra space due to rounding
        for (int i = 0; extraSpace > 0 && i < this.Children.Count; i++)
        {
            if (this.Children[i].GrowWeight > 0)
            {
                extraHeights[i]++;
                extraSpace--;
            }
        }

        for (int i = this.Children.Count - 1; i >= 0; i--)
        {
            var child = this.Children[i];
            int extraHeight = extraHeights[i];
            currentY -= (child.DesiredSize.Y + extraHeight);
            child.Arrange(new RectInt(finalRect.X, currentY, finalRect.Width, child.DesiredSize.Y + extraHeight));
        }

        return finalRect;
    }
}
