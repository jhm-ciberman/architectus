using System;
using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

public enum LayoutOrientation { Horizontal, Vertical }

public class StackLayout : ContainerLayout
{
    public LayoutOrientation Orientation { get; set; } = LayoutOrientation.Horizontal;

    public bool LastChildFill { get; set; } = false;

    public override void Measure(Vector2Int availableSize)
    {
        if (this.Orientation == LayoutOrientation.Horizontal)
        {
            this.MeasureHorizontal(availableSize);
        }
        else
        {
            this.MeasureVertical(availableSize);
        }
    }

    public override void Arrange(RectInt finalRect)
    {
        if (this.Orientation == LayoutOrientation.Horizontal)
        {
            this.ArrangeHorizontal(finalRect);
        }
        else
        {
            this.ArrangeVertical(finalRect);
        }
    }

    private void MeasureHorizontal(Vector2Int availableSize)
    {
        int width = 0;
        int height = 0;

        foreach (var child in this.Children)
        {
            child.Measure(availableSize);
            var desiredSize = child.DesiredSize;
            width += desiredSize.X;
            height = Math.Max(height, desiredSize.Y);
            availableSize.X -= desiredSize.X;
        }

        this.DesiredSize = new Vector2Int(width, height);
    }

    private void MeasureVertical(Vector2Int availableSize)
    {
        int width = 0;
        int height = 0;

        foreach (var child in this.Children)
        {
            child.Measure(availableSize);
            var desiredSize = child.DesiredSize;
            width = Math.Max(width, desiredSize.X);
            height += desiredSize.Y;
            availableSize.Y -= desiredSize.Y;
        }

        this.DesiredSize = new Vector2Int(width, height);
    }

    private void ArrangeVertical(RectInt finalRect)
    {
        int x = finalRect.X;
        int y = finalRect.Y;

        for (int i = 0; i < this.Children.Count; i++)
        {
            var child = this.Children[i];

            var height = child.DesiredSize.Y;
            if (this.LastChildFill && i == this.Children.Count - 1)
            {
                height = finalRect.Height - y + 1;
            }

            child.Arrange(new RectInt(x, y, finalRect.Width, height));
            y += child.DesiredSize.Y;
        }
    }

    private void ArrangeHorizontal(RectInt finalRect)
    {
        int x = finalRect.X;
        int y = finalRect.Y;

        for (int i = 0; i < this.Children.Count; i++)
        {
            var child = this.Children[i];

            var width = child.DesiredSize.X;
            if (this.LastChildFill && i == this.Children.Count - 1)
            {
                width = finalRect.Width - x + 1;
            }

            child.Arrange(new RectInt(x, y, width, finalRect.Height));
            x += child.DesiredSize.X;
        }
    }
}
