using Architectus.Support;
using CommunityToolkit.Mvvm.ComponentModel;
using LifeSim.Support.Numerics;

namespace Architectus.Editor;

public class HousePreviewViewModel : ObservableObject
{
    private int _plotWidth = 20;
    private int _plotHeight = 15;
    private int _floorIndex = 0;
    private bool _flipX = false;
    private bool _flipY = false;
    private int _seed = 0;
    public HouseLot? House { get; private set; } = null;

    public string ErrorMessage { get; private set; } = string.Empty;

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

    public bool FlipX
    {
        get => this._flipX;
        set { if (this.SetProperty(ref this._flipX, value)) this.RegenerateHouse(); }
    }

    public bool FlipY
    {
        get => this._flipY;
        set { if (this.SetProperty(ref this._flipY, value)) this.RegenerateHouse(); }
    }

    public int Seed
    {
        get => this._seed;
        set { if (this.SetProperty(ref this._seed, value)) this.RegenerateHouse(); }
    }

    private void RegenerateHouse()
    {
        var generator = new HouseGenerator();
        generator.PlotSize = new Vector2Int(this._plotWidth, this._plotHeight);
        generator.FlipX = this._flipX;
        generator.FlipY = this._flipY;
        generator.Seed = this._seed;

        if (generator.TryGenerate(out var house))
        {
            this.ErrorMessage = string.Empty;
            this.House = house;
        }
        else
        {
            this.ErrorMessage = generator.LastException?.Message ?? "Could not generate house";
            this.House = null!;
        }

        this.OnPropertyChanged(nameof(this.House));
        this.OnPropertyChanged(nameof(this.ErrorMessage));
    }
}
