using System;
using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

public enum Dock
{
    /// <summary>
    /// The child is docked to the left side of the parent.
    /// </summary>
    Left,

    /// <summary>
    /// The child is docked to the top side of the parent.
    /// </summary>
    Top,

    /// <summary>
    /// The child is docked to the right side of the parent.
    /// </summary>
    Right,

    /// <summary>
    /// The child is docked to the bottom side of the parent.
    /// </summary>
    Bottom,
}


public class DockLayout : ContainerLayout
{
    /// <summary>
    /// Gets or sets whether the last child takes up the remaining space.
    /// </summary>
    public bool LastChildFill { get; set; } = true;

    /// <inheritdoc/>
    protected override Vector2Int MeasureOverride(Vector2Int availableSize)
    {
        int remainingWidth = availableSize.X;
        int remainingHeight = availableSize.Y;

        foreach (var child in this.Children)
        {
            // If LastChildFits is true, the last child gets the full remaining size
            if (this.LastChildFill && child == this.Children[^1])
            {
                child.Measure(new Vector2Int(remainingWidth, remainingHeight));
                break;
            }

            // Measure the child with the current available size
            var childSize = child.Measure(new Vector2Int(remainingWidth, remainingHeight));

            switch (child.Dock)
            {
                case Dock.Top:
                case Dock.Bottom:
                    remainingHeight -= childSize.Y;
                    break;
                case Dock.Left:
                case Dock.Right:
                    remainingWidth -= childSize.X;
                    break;
            }
        }

        // Return the full size of the DockPanel
        return availableSize;
    }

    /// <inheritdoc/>
    protected override RectInt ArrangeOverride(RectInt finalRect)
    {
        int currentX = finalRect.X;
        int currentY = finalRect.Y;
        int remainingWidth = finalRect.Width;
        int remainingHeight = finalRect.Height;

        foreach (var child in this.Children)
        {
            // If LastChildFits is true, the last child gets the full remaining space
            if (this.LastChildFill && child == this.Children[^1])
            {
                child.Arrange(new RectInt(currentX, currentY, remainingWidth, remainingHeight));
                break;
            }

            switch (child.Dock)
            {
                case Dock.Top:
                    child.Arrange(new RectInt(currentX, currentY, remainingWidth, child.DesiredSize.Y));
                    currentY += child.DesiredSize.Y;
                    remainingHeight -= child.DesiredSize.Y;
                    break;

                case Dock.Bottom:
                    child.Arrange(new RectInt(currentX, currentY + remainingHeight - child.DesiredSize.Y, remainingWidth, child.DesiredSize.Y));
                    remainingHeight -= child.DesiredSize.Y;
                    break;

                case Dock.Left:
                    child.Arrange(new RectInt(currentX, currentY, child.DesiredSize.X, remainingHeight));
                    currentX += child.DesiredSize.X;
                    remainingWidth -= child.DesiredSize.X;
                    break;

                case Dock.Right:
                    child.Arrange(new RectInt(currentX + remainingWidth - child.DesiredSize.X, currentY, child.DesiredSize.X, remainingHeight));
                    remainingWidth -= child.DesiredSize.X;
                    break;
            }
        }

        return finalRect;
    }
}
