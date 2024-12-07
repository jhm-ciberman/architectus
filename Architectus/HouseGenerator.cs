using System.Numerics;
using Architectus.Layouts;
using LifeSim.Support.Numerics;

namespace Architectus;

public class HouseGenerator
{
    public Vector2Int PlotSize { get; set; } = new Vector2Int(20, 10);

    public bool TryGenerate(out HouseLot house)
    {
        var layout = new PaddingLayout
        {
            FlipX = true,
            Padding = new ThicknessInt(6, 1, 1, 1),
            Content = new StackLayout
            {
                Orientation = Orientation.Horizontal,
                LastChildFill = true,
                Children =
                {
                    new RoomElement { Size = new Vector2Int(3, 3), Type = RoomType.LivingRoom },
                    new RoomElement { Size = new Vector2Int(3, 3), Type = RoomType.Kitchen },
                    new StackLayout
                    {
                        Orientation = Orientation.Vertical,
                        LastChildFill = true,
                        Children =
                        {
                            new RoomElement { Size = new Vector2Int(3, 3), Type = RoomType.Bedroom },
                            new RoomElement { Size = new Vector2Int(3, 3), Type = RoomType.Bathroom },
                        }
                    }
                },
            },
        };


        house = new HouseLot(this.PlotSize);

        layout.UpdateWorldTransform(layout);
        layout.Measure(this.PlotSize);
        layout.Arrange(new RectInt(0, 0, this.PlotSize.X, this.PlotSize.Y));
        layout.Imprint(house);

        return true;
    }
}
