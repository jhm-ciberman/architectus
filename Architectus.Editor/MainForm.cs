using Eto.Forms;
using Eto.Drawing;

namespace Architectus.Editor;

public partial class MainForm : Form
{
    private readonly HousePreviewControl _housePreviewControl;

    public MainForm()
    {
        this.Title = "Architectus Editor";
        this.MinimumSize = new Size(800, 600);
        this.DataContext = new HousePreviewViewModel(new HouseGenerator());

        var quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
        quitCommand.Executed += (sender, e) => Application.Instance.Quit();

        var aboutCommand = new Command { MenuText = "About..." };
        aboutCommand.Executed += (sender, e) => new AboutDialog().ShowDialog(this);

        // create menu
        this.Menu = new MenuBar
        {
            Items =
            {
                // File submenu
                new SubMenuItem { Text = "&File" },
                // new SubMenuItem { Text = "&Edit", Items = { /* commands/items */ } },
                // new SubMenuItem { Text = "&View", Items = { /* commands/items */ } },
            },
            ApplicationItems =
            {
                // application (OS X) or file menu (others)
                new ButtonMenuItem { Text = "&Preferences..." },
            },
            QuitItem = quitCommand,
            AboutItem = aboutCommand
        };

        var widthStepper = new NumericStepper { MinValue = 1, MaxValue = 100, Value = 10 };
        widthStepper.ValueBinding.BindDataContext((HousePreviewViewModel vm) => vm.PlotWidth);

        var heightStepper = new NumericStepper { MinValue = 1, MaxValue = 100, Value = 10 };
        heightStepper.ValueBinding.BindDataContext((HousePreviewViewModel vm) => vm.PlotHeight);

        var floorIndexStepper = new NumericStepper { MinValue = 0, MaxValue = 10, Value = 0 };
        floorIndexStepper.ValueBinding.BindDataContext((HousePreviewViewModel vm) => vm.FloorIndex);

        this._housePreviewControl = new HousePreviewControl
        {
            Size = new Size(400, 400),
            BackgroundColor = Colors.White,
        };
        this._housePreviewControl.HouseLotBinding.BindDataContext((HousePreviewViewModel vm) => vm.House);
        this._housePreviewControl.FloorIndexBinding.BindDataContext((HousePreviewViewModel vm) => vm.FloorIndex);


        this.Content = new StackLayout
        {
            Padding = 10,
            Orientation = Orientation.Horizontal,
            Items =
            {
                new StackLayout
                {
                    Padding = 10,
                    Orientation = Orientation.Vertical,
                    Items =
                    {
                        "Width",
                        widthStepper,
                        "Height",
                        heightStepper,
                        "Floor Index",
                        floorIndexStepper,
                    },
                },
                new StackLayoutItem(this._housePreviewControl, true),
            }
        };
    }
}
