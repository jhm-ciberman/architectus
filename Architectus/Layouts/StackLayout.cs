using System;
using System.Collections.Generic;
using System.Linq;
using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

public enum Orientation
{
    Horizontal,
    Vertical,
}

public class StackLayout : ContainerLayout
{
    public bool LastChildFill { get; set; } = true;

    public Orientation Orientation { get; set; } = Orientation.Vertical;

    public bool Reverse { get; set; } = false;

    private IReadOnlyList<LayoutElement>? _actualChildren;

    protected override Vector2Int MeasureOverride(Vector2Int availableSize)
    {
        var size = Vector2Int.Zero;

        for (var i = 0; i < this.Children.Count; i++)
        {
            var child = this.Children[i];
            var childSize = child.Measure(availableSize);

            if (this.Orientation == Orientation.Horizontal)
            {
                size.X += childSize.X;
                size.Y = Math.Max(size.Y, childSize.Y);
            }
            else
            {
                size.X = Math.Max(size.X, childSize.X);
                size.Y += childSize.Y;
            }
        }

        return size;
    }


    protected override RectInt ArrangeOverride(RectInt finalRect)
    {
        var offset = finalRect.Position;

        this._actualChildren ??= this.Reverse
            ? this.Children.ToArray().Reverse().ToList()
            : this.Children;

        for (var i = 0; i < this._actualChildren.Count; i++)
        {
            var child = this._actualChildren[i];
            var fill = this.LastChildFill && i == this._actualChildren.Count - 1;

            if (this.Orientation == Orientation.Horizontal)
            {
                var childWidth = fill ? finalRect.Right - offset.X : child.DesiredSize.X;

                child.Arrange(new RectInt(offset, new Vector2Int(childWidth, finalRect.Height)));
                offset.X += child.Bounds.Width;
            }
            else
            {
                var childHeight = fill ? finalRect.Bottom - offset.Y : child.DesiredSize.Y;

                child.Arrange(new RectInt(offset, new Vector2Int(finalRect.Width, childHeight)));
                offset.Y += child.Bounds.Height;
            }
        }

        return finalRect;
    }
}
