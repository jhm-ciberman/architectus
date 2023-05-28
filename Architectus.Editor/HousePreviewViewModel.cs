using Architectus;

namespace Architectus.Editor;

public class HousePreviewViewModel : ObservableObject
{
    private readonly HouseGenerator _generator;

    private int _plotWidth = 10;
    private int _plotHeight = 10;
    public HouseLot House { get; private set; } = null!;

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

    private void RegenerateHouse()
    {
        this._generator.PlotSize = new Vector2Int(this._plotWidth, this._plotHeight);
        this.House = this._generator.Generate();
        this.OnPropertyChanged(nameof(House));
    }
}