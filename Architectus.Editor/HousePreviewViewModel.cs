using Architectus.Support;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Architectus.Editor;

public class HousePreviewViewModel : ObservableObject
{
    private readonly HouseGenerator _generator;

    private int _plotWidth = 10;
    private int _plotHeight = 10;
    private int _floorIndex = 0;
    public HouseLot? House { get; private set; } = null;

    public HousePreviewViewModel(HouseGenerator generator)
    {
        this._generator = generator;
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
        this._generator.PlotSize = new Vector2Int(this._plotWidth, this._plotHeight);

        if (this._generator.TryGenerate(out var house))
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
