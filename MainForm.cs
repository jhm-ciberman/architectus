namespace Architectus;

public partial class MainForm : Form
{
    public HousePlan _house;

    private readonly int _cellSize = 20; // The size of each cell in the grid (in pixels).

    private readonly Pen _wallPen = new Pen(Color.Black, 3f);

    private readonly Pen _gridPen = new Pen(Color.LightGray, 2f);

    private readonly Brush _grassBrush = new SolidBrush(Color.LightGreen); 

    private readonly Brush[] _roomBrushes = new[]
    {
        Brushes.Salmon,
        Brushes.SandyBrown,
        Brushes.LightBlue,
        Brushes.Thistle,
        Brushes.PaleGoldenrod,
        Brushes.PaleTurquoise,
        Brushes.PaleVioletRed,
        Brushes.PapayaWhip,
        Brushes.PeachPuff,
        Brushes.Peru,
    };

    private readonly HouseGenerator _generator;

    private readonly NumericUpDown _plotWidth;

    private readonly NumericUpDown _plotHeight;

    public MainForm()
    {
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 600);
        this.Text = "Architectus";

        this._generator = new HouseGenerator();
        this._generator.PlotSize = new Vector2Int(20, 8);
        this._house = this._generator.Generate();

        // Add a numeric updown control for controlling the size of the plot.
        this._plotWidth = new NumericUpDown
        {
            Minimum = 3, Maximum = 100,
            Value = this._generator.PlotSize.X,
            Width = 100, Left = 10, Top = 10,
            Anchor = AnchorStyles.Left | AnchorStyles.Top,
        };

        this._plotHeight = new NumericUpDown
        {
            Minimum = 3, Maximum = 100,
            Value = this._generator.PlotSize.Y,
            Width = 100, Left = 10, Top = 30,
            Anchor = AnchorStyles.Left | AnchorStyles.Top,
        };

        this._plotWidth.ValueChanged += (sender, e) => this.RegenerateHouse();
        this._plotHeight.ValueChanged += (sender, e) => this.RegenerateHouse();

        this.Controls.Add(this._plotWidth);
        this.Controls.Add(this._plotHeight);
    }

    private void RegenerateHouse()
    {
        this._generator.PlotSize = new Vector2Int((int)this._plotWidth.Value, (int)this._plotHeight.Value);
        this._house = this._generator.Generate();
        this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;

        Vector2Int coords = new(150, 10);
        foreach (var floor in this._house.Floors)
        {
            this.DrawFloor(g, floor, coords);
            coords.X += floor.Size.X;
        }
    }

    private void DrawFloor(Graphics g, FloorPlan floor, Vector2Int coords)
    {
        var cellSize = this._cellSize;

        var size = floor.Size;
        for (int x = 0; x < size.X; x++)
        {
            for (int y = 0; y < size.Y; y++)
            {
                var room = floor.GetRoom(new Vector2Int(x, y));
                if (room != null)
                {
                    var rect = new Rectangle(coords.X + x * cellSize, coords.Y + y * cellSize, cellSize, cellSize);
                    var brush = room.Type == RoomType.Empty
                        ? this._grassBrush
                        : this._roomBrushes[room.Id % this._roomBrushes.Length];
                    g.FillRectangle(brush, rect);
                }
            }
        }

        // Draw grid lines.
        for (int x = 0; x <= size.X; x++)
        {
            g.DrawLine(this._gridPen, coords.X + x * cellSize, coords.Y, coords.X + x * cellSize, coords.Y + size.Y * cellSize);
        }

        for (int y = 0; y <= size.Y; y++)
        {
            g.DrawLine(this._gridPen, coords.X, coords.Y + y * cellSize, coords.X + size.X * cellSize, coords.Y + y * cellSize);
        }
    }
}
