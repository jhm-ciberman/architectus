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
        bool isHorizontal = this.IsHorizontal;
        return this.MeasureMainAxis(availableSize, isHorizontal);
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

    private Vector2Int MeasureMainAxis(Vector2Int availableSize, bool isHorizontal)
    {
        int totalMainSize = 0;

        foreach (var child in this.Children)
        {
            Vector2Int childAvailable = isHorizontal
                ? new Vector2Int(availableSize.X - totalMainSize, availableSize.Y)
                : new Vector2Int(availableSize.X, availableSize.Y - totalMainSize);

            Vector2Int childSize = child.Measure(childAvailable);
            totalMainSize += isHorizontal ? childSize.X : childSize.Y;
        }

        return isHorizontal
            ? new Vector2Int(totalMainSize, availableSize.Y)
            : new Vector2Int(availableSize.X, totalMainSize);
    }

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
        int currentMain = isHorizontal ? finalRect.X : finalRect.Y;

        foreach (var (child, extraSpace) in this.Children.Zip(extraSpaceAllocated))
        {
            if (isHorizontal)
            {
                child.Arrange(new RectInt(currentMain, finalRect.Y, child.DesiredSize.X + extraSpace, finalRect.Height));
                currentMain += child.DesiredSize.X + extraSpace;
            }
            else
            {
                child.Arrange(new RectInt(finalRect.X, currentMain, finalRect.Width, child.DesiredSize.Y + extraSpace));
                currentMain += child.DesiredSize.Y + extraSpace;
            }
        }
    }

    private void ArrangeChildrenReverse(RectInt finalRect, bool isHorizontal, int[] extraSpaceAllocated)
    {
        int currentMain = isHorizontal ? finalRect.X + finalRect.Width : finalRect.Y + finalRect.Height;
        int childCount = this.Children.Count;

        for (int i = childCount - 1; i >= 0; i--)
        {
            var child = this.Children[i];
            int extraSpace = extraSpaceAllocated[i];

            if (isHorizontal)
            {
                currentMain -= (child.DesiredSize.X + extraSpace);
                child.Arrange(new RectInt(currentMain, finalRect.Y, child.DesiredSize.X + extraSpace, finalRect.Height));
            }
            else
            {
                currentMain -= (child.DesiredSize.Y + extraSpace);
                child.Arrange(new RectInt(finalRect.X, currentMain, finalRect.Width, child.DesiredSize.Y + extraSpace));
            }
        }
    }
}
