using System.Numerics;
using Architectus.Components;
using Architectus.Layouts;
using LifeSim.Support.Numerics;

namespace Architectus;

public class HouseGenerator
{
    public Vector2Int PlotSize { get; set; } = new Vector2Int(20, 10);
    public bool FlipX { get; set; } = false;
    public bool FlipY { get; set; } = false;
    public int Seed { get; set; } = 0;

    public bool TryGenerate(out HouseLot house)
    {
        var layout = new PaddingLayout
        {
            FlipX = this.FlipX,
            FlipY = this.FlipY,
            Padding = new ThicknessInt(6, 1, 1, 1),
            Content = GetContent(),
        };

        house = new HouseLot(this.PlotSize);

        //layout.UpdateWorldMatrix(Matrix3x2.Identity);
        layout.Measure(this.PlotSize);
        layout.UpdateWorldMatrix(Matrix3x2.Identity);
        layout.Arrange(new RectInt(0, 0, this.PlotSize.X, this.PlotSize.Y));
        layout.Imprint(house);

        return true;
    }

    private LayoutElement GetContent()
    {
        var component = new TinyHouseComponent();
        var ctx = new HouseContext(this.Seed);
        var bounds = new RectInt(0, 0, this.PlotSize.X, this.PlotSize.Y);
        return component.Expand(bounds, ctx);
    }
}
