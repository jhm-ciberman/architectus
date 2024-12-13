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
        Vector2Int size = Vector2Int.Zero;
        if (this.IsHorizontal)
        {
            foreach (var child in this.Children)
            {
                child.Measure(availableSize);
                size.X += child.DesiredSize.X;
                size.Y = Math.Max(size.Y, child.DesiredSize.Y);
            }
        }
        else
        {
            foreach (var child in this.Children)
            {
                child.Measure(availableSize);
                size.X = Math.Max(size.X, child.DesiredSize.X);
                size.Y += child.DesiredSize.Y;
            }
        }

        return size;
    }

    /// <inheritdoc/>
    protected override RectInt ArrangeOverride(RectInt finalRect)
    {
        bool isHorizontal = this.IsHorizontal;
        bool isReverse = this.IsReverse;

        this.ArrangeMainAxis(finalRect, isHorizontal, isReverse);
        return finalRect;
    }

    private bool IsHorizontal => this.Orientation == Orientation.Row || this.Orientation == Orientation.RowReverse;

    private bool IsReverse => this.Orientation == Orientation.RowReverse || this.Orientation == Orientation.ColumnReverse;

    private void ArrangeMainAxis(RectInt finalRect, bool isHorizontal, bool isReverse)
    {
        int mainSize = isHorizontal ? finalRect.Width : finalRect.Height;
        int totalDesiredMainSize = isHorizontal
            ? this.Children.Sum(c => c.DesiredSize.X)
            : this.Children.Sum(c => c.DesiredSize.Y);

        int remainingSpace = mainSize - totalDesiredMainSize;
        float totalGrowWeight = this.Children.Sum(c => c.GrowWeight);

        // Allocate extra space based on grow weights
        int[] extraSpaceAllocated = AllocateExtraSpace(this.Children, remainingSpace, totalGrowWeight);

        if (isReverse)
        {
            this.ArrangeChildrenReverse(finalRect, isHorizontal, extraSpaceAllocated);
        }
        else
        {
            this.ArrangeChildrenForward(finalRect, isHorizontal, extraSpaceAllocated);
        }
    }

    private static int[] AllocateExtraSpace(IReadOnlyList<LayoutElement> children, int remainingSpace, float totalGrowWeight)
    {
        int childCount = children.Count;
        int[] extraSpace = new int[childCount];
        int extraRemaining = remainingSpace;

        // First pass: allocate space based on grow weight
        for (int i = 0; i < childCount; i++)
        {
            var child = children[i];
            if (totalGrowWeight > 0 && child.GrowWeight > 0)
            {
                extraSpace[i] = (int)(remainingSpace * (child.GrowWeight / totalGrowWeight));
                extraRemaining -= extraSpace[i];
            }
        }

        // Second pass: distribute any remaining space due to rounding
        for (int i = 0; extraRemaining > 0 && i < childCount; i++)
        {
            if (children[i].GrowWeight > 0)
            {
                extraSpace[i]++;
                extraRemaining--;
            }
        }

        return extraSpace;
    }

    private void ArrangeChildrenForward(RectInt finalRect, bool isHorizontal, int[] extraSpaceAllocated)
    {
        Vector2Int coord = finalRect.Position;

        foreach (var (child, extraSpace) in this.Children.Zip(extraSpaceAllocated))
        {
            if (isHorizontal)
            {
                var size = new Vector2Int(child.DesiredSize.X + extraSpace, finalRect.Height);
                child.Arrange(new RectInt(coord, size));
                coord.X += child.DesiredSize.X + extraSpace;
            }
            else
            {
                var size = new Vector2Int(finalRect.Width, child.DesiredSize.Y + extraSpace);
                child.Arrange(new RectInt(coord, size));
                coord.Y += child.DesiredSize.Y + extraSpace;
            }
        }
    }

    private void ArrangeChildrenReverse(RectInt finalRect, bool isHorizontal, int[] extraSpaceAllocated)
    {
        var coord = isHorizontal
            ? new Vector2Int(finalRect.X + finalRect.Width, finalRect.Y)
            : new Vector2Int(finalRect.X, finalRect.Y + finalRect.Height);

        int childCount = this.Children.Count;

        foreach (var (child, extraSpace) in this.Children.Zip(extraSpaceAllocated))
        {
            if (isHorizontal)
            {
                var size = new Vector2Int(child.DesiredSize.X + extraSpace, finalRect.Height);
                coord.X -= size.X;
                child.Arrange(new RectInt(coord, size));
            }
            else
            {
                var size = new Vector2Int(finalRect.Width, child.DesiredSize.Y + extraSpace);
                coord.Y -= size.Y;
                child.Arrange(new RectInt(coord, size));
            }
        }
    }
}
