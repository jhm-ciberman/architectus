using System.Numerics;
using Architectus.Layouts;
using LifeSim.Support.Numerics;

namespace Architectus;

public class HouseGenerator
{
    public Vector2Int PlotSize { get; set; } = new Vector2Int(20, 10);
    public bool FlipX { get; set; } = false;
    public bool FlipY { get; set; } = false;

    public bool TryGenerate(out HouseLot house)
    {
        var layout = new PaddingLayout
        {
            FlipX = this.FlipX,
            FlipY = this.FlipY,
            Padding = new ThicknessInt(6, 1, 1, 1),
            Content = new StackLayout
            {
                Orientation = Orientation.Row,
                Children =
                {
                    new RoomElement { MinSize = new Vector2Int(3, 3), Type = RoomType.LivingRoom },
                    new RoomElement { MinSize = new Vector2Int(3, 3), Type = RoomType.Kitchen },
                    new StackLayout
                    {
                        Orientation = Orientation.ColumnReverse,
                        Children =
                        {
                            new RoomElement { MinSize = new Vector2Int(3, 3), Type = RoomType.Bedroom },
                            new RoomElement { MinSize = new Vector2Int(3, 3), Type = RoomType.Bathroom },
                        }
                    }
                },
            },
        };


        house = new HouseLot(this.PlotSize);

        //layout.UpdateWorldMatrix(Matrix3x2.Identity);
        layout.Measure(this.PlotSize);
        layout.UpdateWorldMatrix(Matrix3x2.Identity);
        layout.Arrange(new RectInt(0, 0, this.PlotSize.X, this.PlotSize.Y));
        layout.Imprint(house);

        return true;
    }
}
