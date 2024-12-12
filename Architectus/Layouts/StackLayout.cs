using System;
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

        foreach (var child in this.Children)
        {
            child.Arrange(new RectInt(currentX, finalRect.Y, child.DesiredSize.X, finalRect.Height));

            currentX += child.DesiredSize.X;
        }

        return finalRect;
    }

    private RectInt ArrangeColumn(RectInt finalRect)
    {
        int currentY = finalRect.Y;

        foreach (var child in this.Children)
        {
            child.Arrange(new RectInt(finalRect.X, currentY, finalRect.Width, child.DesiredSize.Y));

            currentY += child.DesiredSize.Y;
        }

        return finalRect;
    }

    private RectInt ArrangeRowReverse(RectInt finalRect)
    {
        int currentX = finalRect.X + finalRect.Width;

        foreach (var child in this.Children)
        {
            currentX -= child.DesiredSize.X;
            child.Arrange(new RectInt(currentX, finalRect.Y, child.DesiredSize.X, finalRect.Height));
        }

        return finalRect;
    }

    private RectInt ArrangeColumnReverse(RectInt finalRect)
    {
        int currentY = finalRect.Y + finalRect.Height;

        foreach (var child in this.Children)
        {
            currentY -= child.DesiredSize.Y;
            child.Arrange(new RectInt(finalRect.X, currentY, finalRect.Width, child.DesiredSize.Y));
        }

        return finalRect;
    }
}
