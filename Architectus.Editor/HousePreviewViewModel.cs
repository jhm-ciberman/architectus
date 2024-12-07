using Architectus.Support;
using CommunityToolkit.Mvvm.ComponentModel;
using LifeSim.Support.Numerics;

namespace Architectus.Editor;

public class HousePreviewViewModel : ObservableObject
{
    private int _plotWidth = 20;
    private int _plotHeight = 15;
    private int _floorIndex = 0;
    public HouseLot? House { get; private set; } = null;

    public HousePreviewViewModel()
    {
        this.RegenerateHouse();
    }

    public int PlotWidth
    {
        get => this._plotWidth;
        set { if (this.SetProperty(ref this._plotWidth, value)) this.RegenerateHouse(); }
    }

    public int PlotHeight
    {
        get => this._plotHeight;
        set { if (this.SetProperty(ref this._plotHeight, value)) this.RegenerateHouse(); }
    }

    public int FloorIndex
    {
        get => this._floorIndex;
        set => this.SetProperty(ref this._floorIndex, value);
    }

    private void RegenerateHouse()
    {
        var generator = new HouseGenerator();
        generator.PlotSize = new Vector2Int(this._plotWidth, this._plotHeight);

        if (generator.TryGenerate(out var house))
        {
            this.House = house;
        }
        else
        {
            this.House = null!;
        }

        this.OnPropertyChanged(nameof(this.House));
    }
}
