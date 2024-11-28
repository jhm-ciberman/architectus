using System;
using System.Collections.Generic;
using LifeSim.Support.Numerics;

namespace Architectus;

// Two-pass layout. Aka "Measure and Arrange".
public abstract class LayoutElement
{
    public Vector2Int DesiredSize { get; protected set; }

    public abstract void Measure(Vector2Int availableSize);

    public abstract void Arrange(RectInt finalRect);

    public abstract void Render(HouseLot house);
}

public abstract class ContainerLayout : LayoutElement
{
    public List<LayoutElement> Children { get; } = new List<LayoutElement>();

    public override void Render(HouseLot house)
    {
        foreach (var child in this.Children)
        {
            child.Render(house);
        }
    }
}

public abstract class DecoratorLayout : LayoutElement
{
    public LayoutElement Child { get; set; } = null!;

    public override void Measure(Vector2Int availableSize)
    {
        this.Child.Measure(availableSize);
        this.DesiredSize = this.Child.DesiredSize;
    }

    public override void Arrange(RectInt finalRect)
    {
        this.Child.Arrange(finalRect);
    }

    public override void Render(HouseLot house)
    {
        this.Child.Render(house);
    }
}	

public class HorizontalLayout : ContainerLayout
{
    public override void Measure(Vector2Int availableSize)
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

    public override void Arrange(RectInt finalRect)
    {
        int x = finalRect.X;
        int y = finalRect.Y;

        foreach (var child in this.Children)
        {
            child.Arrange(new RectInt(x, y, child.DesiredSize.X, finalRect.Height));
            x += child.DesiredSize.X;
        }
    }
}

public class VerticalLayout : ContainerLayout
{
    public override void Measure(Vector2Int availableSize)
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

    public override void Arrange(RectInt finalRect)
    {
        int x = finalRect.X;
        int y = finalRect.Y;

        foreach (var child in this.Children)
        {
            child.Arrange(new RectInt(x, y, finalRect.Width, child.DesiredSize.Y));
            y += child.DesiredSize.Y;
        }
    }
}

public class RoomElement : LayoutElement
{
    public Vector2Int Size { get; set; }

    public RoomType Type { get; set; } = RoomType.Garden;

    public RectInt Bounds { get; private set; }

    public override void Measure(Vector2Int availableSize)
    {
        if (availableSize.X < this.Size.X || availableSize.Y < this.Size.Y)
        {
            throw new InvalidOperationException($"The available size is too small for the room. Required: {this.Size}, available: {availableSize}.");
        }

        this.DesiredSize = this.Size;
    }

    public override void Arrange(RectInt finalRect)
    {
        this.Bounds = finalRect;
    }

    public override void Render(HouseLot house)
    {
        house.GroundFloor.AddRoom(this.Bounds, this.Type);
    }
}

public class PaddingLayout : DecoratorLayout
{
    public int Padding { get; set; }

    public override void Measure(Vector2Int availableSize)
    {
        var padding = Vector2Int.One * this.Padding * 2;
        this.Child.Measure(availableSize - padding);

        this.DesiredSize = this.Child.DesiredSize + padding;
    }

    public override void Arrange(RectInt finalRect)
    {
        this.Child.Arrange(new RectInt(
            finalRect.X + this.Padding,
            finalRect.Y + this.Padding,
            finalRect.Width - 2 * this.Padding,
            finalRect.Height - 2 * this.Padding));
    }

    public override void Render(HouseLot house)
    {
        this.Child.Render(house);
    }
}

public class HouseGenerator
{
    public Vector2Int PlotSize { get; set; } = new Vector2Int(10, 10);

    public bool TryGenerate(out HouseLot house)
    {
        var layout = new PaddingLayout
        {
            Padding = 1,
            Child = new HorizontalLayout
            {
                Children =
                {
                    new RoomElement { Size = new Vector2Int(3, 3), Type = RoomType.LivingRoom },
                    new RoomElement { Size = new Vector2Int(3, 3), Type = RoomType.Kitchen },
                    //new RoomElement { Size = new Vector2Int(3, 3), Type = RoomType.Bedroom },
                },
            },
        };


        house = new HouseLot(this.PlotSize);

        layout.Measure(this.PlotSize);
        layout.Arrange(new RectInt(0, 0, this.PlotSize.X, this.PlotSize.Y));
        layout.Render(house);

        return true;
    }
}