using System;
using Eto.Drawing;
using Eto.Forms;

namespace Architectus.Editor
{
    /// <summary>
    /// Renders the specified house.
    /// </summary>
    public class HousePreviewControl : Drawable
    {
        private HousePlan? _house = null;
        public HousePlan? House
        {
            get => this._house;
            set
            {
                if (this._house == value)
                    return;
                this._house = value;
                this.Invalidate();
            }
        }

        public BindableBinding<HousePreviewControl, HousePlan?> FloorBinding { get; }

        private readonly int _cellSize = 20; // The size of each cell in the grid (in pixels).

        private readonly Pen _wallPen = new Pen(Brushes.Black, 3f);

        private readonly Pen _gridPen = new Pen(Brushes.LightGrey, 2f);

        private readonly Color _grassColor = Colors.LightGreen;

        private readonly Font _font = new Font(FontFamilies.Sans, 8f);

        private readonly Brush _fontBrush = Brushes.DarkSlateGray;

        private readonly Color[] _roomColors = new[]
        {
            Colors.Salmon,
            Colors.SandyBrown,
            Colors.LightBlue,
            Colors.Thistle,
            Colors.PaleGoldenrod,
            Colors.PaleTurquoise,
            Colors.PaleVioletRed,
            Colors.PapayaWhip,
            Colors.PeachPuff,
            Colors.Peru,
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="HousePreviewControl"/> class.
        /// </summary>
        public HousePreviewControl()
        {
            this.FloorBinding = new BindableBinding<HousePreviewControl, HousePlan?>(this, 
                self => self.House, 
                (self, value) => self.House = value);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var house = this.House;
            if (house == null) return;

            var floor = house.Floors[0];

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
                        var color = room.Type == RoomType.Garden
                            ? this._grassColor
                            : this._roomColors[room.GetHashCode() % this._roomColors.Length];
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

            foreach (var room in floor.Rooms)
            {
                var pos = room.BoundingBox.Center;
                var str = room.Type.ToString()[0];
                g.DrawText(this._font, this._fontBrush, coords.X + pos.X * cellSize, coords.Y + pos.Y * cellSize, str.ToString());
            }

            {
                var pos = coords + floor.Entrance * cellSize;
                g.DrawRectangle(this._wallPen, new RectangleF(pos.X + 2, pos.Y + 2, cellSize - 4, cellSize - 4));
            }

            Grid? grid = floor.Grid;
            if (grid != null)
            {
                float xxx, yyy;
                // Draw lines for grid.
                int x = 0;
                foreach (var w in grid.ColumnWidths)
                {
                    xxx = coords.X + (grid.Position.X + x) * cellSize;
                    yyy = coords.Y + grid.Position.Y * cellSize;
                    g.DrawLine(this._wallPen, new PointF(xxx, yyy), new PointF(xxx, yyy + grid.Size.Y * cellSize));
                    x += w;
                }
                // Draw bottom line
                xxx = coords.X + grid.Position.X * cellSize;
                yyy = coords.Y + (grid.Position.Y + grid.Size.Y) * cellSize;
                g.DrawLine(this._wallPen, new PointF(xxx, yyy), new PointF(xxx + grid.Size.X * cellSize, yyy));

                int y = 0;
                foreach (var h in grid.RowHeights)
                {
                    yyy = coords.Y + (grid.Position.Y + y) * cellSize;
                    xxx = coords.X + grid.Position.X * cellSize;
                    g.DrawLine(this._wallPen, new PointF(xxx, yyy), new PointF(xxx + grid.Size.X * cellSize, yyy));
                    y += h;
                }
                // Draw right line
                xxx = coords.X + (grid.Position.X + grid.Size.X) * cellSize;
                yyy = coords.Y + grid.Position.Y * cellSize;
                g.DrawLine(this._wallPen, new PointF(xxx, yyy), new PointF(xxx, yyy + grid.Size.Y * cellSize));
            }
        }
    }
}
