using System;
using Architectus.Support;
using Eto.Drawing;
using Eto.Forms;
using LifeSim.Support.Numerics;

namespace Architectus.Editor;

/// <summary>
/// Renders the specified house.
/// </summary>
public class HousePreviewControl : Drawable
{
    private HouseLot? _houseLot = null;
    public HouseLot? HouseLot
    {
        get => this._houseLot;
        set
        {
            if (this._houseLot == value)
                return;
            this._houseLot = value;
            this.Invalidate();
        }
    }

    private int _floorIndex = 0;
    public int FloorIndex
    {
        get => this._floorIndex;
        set
        {
            if (this._floorIndex == value)
                return;
            this._floorIndex = value;
            this.Invalidate();
        }
    }

    private string? _errorMessage = null;

    public string? ErrorMessage
    {
        get => this._errorMessage;
        set
        {
            if (this._errorMessage == value)
                return;
            this._errorMessage = value;
            this.Invalidate();
        }
    }

    public BindableBinding<HousePreviewControl, HouseLot?> HouseLotBinding { get; }

    public BindableBinding<HousePreviewControl, int> FloorIndexBinding { get; }
    public BindableBinding<HousePreviewControl, string?> ErrorMessageBinding { get; }

    private readonly int _cellSize = 22; // The size of each cell in the grid (in pixels).

    private readonly Pen _wallPen = new Pen(TailwindColors.Slate600, 3f);

    private readonly Pen _gridPen = new Pen(new Color(TailwindColors.Slate600, 0.2f), 2f);

    //private readonly Color _grassColor = Colors.LightGreen;

    private readonly Font _font = new Font(FontFamilies.Sans, 10f, FontStyle.Bold);

    private readonly Brush _fontBrush = new SolidBrush(new Color(TailwindColors.Black, 0.8f));

    private readonly Brush _fontErrorBrush = new SolidBrush(TailwindColors.Red600);

    private static Color GetRoomColor(Room room)
    {
        return room.Type switch
        {
            RoomType.Garden => TailwindColors.Green200,
            RoomType.Bedroom => TailwindColors.Blue200,
            RoomType.Bathroom => TailwindColors.Indigo200,
            RoomType.Kitchen => TailwindColors.Yellow200,
            RoomType.LivingRoom => TailwindColors.Red200,
            RoomType.Corridor => TailwindColors.Slate200,
            _ => TailwindColors.Gray600,
        };
    }

    private static void DrawTextCentered(Graphics g, Font font, Brush brush, RectangleF bounds, string text, bool wrap = false)
    {
        var size = g.MeasureString(font, text);

        size.Width = MathF.Min(size.Width, bounds.Width) + 1;
        size.Height = MathF.Min(size.Height, bounds.Height) + 1;

        var textBounds = new RectangleF(
            bounds.X + (bounds.Width - size.Width) / 2,
            bounds.Y + (bounds.Height - size.Height) / 2,
            size.Width,
            bounds.Height
        );

        g.DrawText(
            font,
            brush,
            textBounds,
            text,
            alignment: FormattedTextAlignment.Center,
            trimming: FormattedTextTrimming.None,
            wrap: wrap ? FormattedTextWrapMode.Word : FormattedTextWrapMode.None
        );
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HousePreviewControl"/> class.
    /// </summary>
    public HousePreviewControl()
    {
        this.HouseLotBinding = new BindableBinding<HousePreviewControl, HouseLot?>(this,
            self => self.HouseLot,
            (self, value) => self.HouseLot = value);

        this.FloorIndexBinding = new BindableBinding<HousePreviewControl, int>(this,
            self => self.FloorIndex,
            (self, value) => self.FloorIndex = value);

        this.ErrorMessageBinding = new BindableBinding<HousePreviewControl, string?>(this,
            self => self.ErrorMessage,
            (self, value) => self.ErrorMessage = value);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (!string.IsNullOrEmpty(this.ErrorMessage))
        {
            var rect = e.ClipRectangle;
            var padding = new Size(40, 40);
            rect.TopLeft += padding;
            rect.Size -= padding * 2;
            var message = $"Error Generating House: \n{this.ErrorMessage}";
            DrawTextCentered(e.Graphics, this._font, this._fontErrorBrush, rect, message, wrap: true);
            return;
        }

        var house = this.HouseLot;
        if (house == null)
        {
            DrawTextCentered(e.Graphics, this._font, this._fontBrush, e.ClipRectangle, "No house to preview.");
            return;
        }

        if (this.FloorIndex < 0 || this.FloorIndex >= house.Floors.Count)
        {
            string str = $"Invalid floor index: {this.FloorIndex}. Must be between 0 and {house.Floors.Count - 1}.";
            DrawTextCentered(e.Graphics, this._font, this._fontBrush, e.ClipRectangle, str);
            return;
        }

        var floor = house.Floors[this.FloorIndex];

        var g = e.Graphics;
        var cellSize = this._cellSize;
        Vector2Int coords = Vector2Int.Zero;

        var size = floor.Size;
        for (int x = 0; x < size.X; x++)
        {
            for (int y = 0; y < size.Y; y++)
            {
                var room = floor.GetRoom(new Vector2Int(x, y));
                if (room != null)
                {
                    var rect = new RectangleF(coords.X + x * cellSize, coords.Y + y * cellSize, cellSize, cellSize);
                    var color = GetRoomColor(room);
                    g.FillRectangle(color, rect);
                }
            }
        }

        // Draw grid lines.
        for (int x = 0; x <= size.X; x++)
        {
            g.DrawLine(this._gridPen, new Point(coords.X + x * cellSize, coords.Y), new Point(coords.X + x * cellSize, coords.Y + size.Y * cellSize));
        }

        for (int y = 0; y <= size.Y; y++)
        {
            g.DrawLine(this._gridPen, new Point(coords.X, coords.Y + y * cellSize), new Point(coords.X + size.X * cellSize, coords.Y + y * cellSize));
        }

        this.DrawWalls(g, floor, coords, cellSize);

        foreach (var room in floor.Rooms)
        {
            var str = room.Type.ToString();
            var bounds = new RectangleF(
                coords.X + room.Bounds.X * cellSize,
                coords.Y + room.Bounds.Y * cellSize,
                room.Bounds.Width * cellSize,
                room.Bounds.Height * cellSize);

            DrawTextCentered(g, this._font, this._fontBrush, bounds, str);
        }
    }

    private void DrawWalls(Graphics g, Floor floor, Vector2Int coords, int cellSize)
    {
        var size = floor.Size;
        for (int x = 0; x < size.X + 1; x++)
        {
            for (int y = 0; y < size.Y + 1; y++)
            {
                var room = floor.GetRoom(new Vector2Int(x, y));
                var roomLeft = floor.GetRoom(new Vector2Int(x - 1, y));
                var roomTop = floor.GetRoom(new Vector2Int(x, y - 1));

                bool drawLeft = room != roomLeft;
                bool drawTop = room != roomTop;
                if (drawLeft)
                {
                    var pos = coords + new Vector2Int(x, y) * cellSize;
                    g.DrawLine(this._wallPen, new Point(pos.X, pos.Y), new Point(pos.X, pos.Y + cellSize));
                }

                if (drawTop)
                {
                    var pos = coords + new Vector2Int(x, y) * cellSize;
                    g.DrawLine(this._wallPen, new Point(pos.X, pos.Y), new Point(pos.X + cellSize, pos.Y));
                }
            }
        }
    }
}
