using Architectus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Editor
{
    /// <summary>
    /// Renders the specified house.
    /// </summary>
    public class HousePreviewControl : FrameworkElement
    {
        public FloorPlan? Floor { get; set; }

        private readonly int _cellSize = 20; // The size of each cell in the grid (in pixels).

        private readonly Pen _wallPen = new Pen(Brushes.Black, 3f);

        private readonly Pen _gridPen = new Pen(Brushes.LightGray, 2f);

        private readonly Brush _grassBrush = new SolidColorBrush(Colors.LightGreen);

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

        static HousePreviewControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HousePreviewControl), new FrameworkPropertyMetadata(typeof(HousePreviewControl)));
        }

        public HousePreviewControl()
        {
            this.DataContextChanged += HousePreviewControl_DataContextChanged;
        }

        private void HousePreviewControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var house = (HousePlan)e.NewValue;
            this.Floor = house.Floors[0];
            this.InvalidateVisual();
        }

        public void Refresh()
        {
            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var g = drawingContext;

            var cellSize = this._cellSize;

            Vector2Int coords = Vector2Int.Zero;

            var floor = this.Floor;
            if (floor == null) return;

            var size = floor.Size;
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    var room = floor.GetRoom(new Vector2Int(x, y));
                    if (room != null)
                    {
                        var rect = new Rect(coords.X + x * cellSize, coords.Y + y * cellSize, cellSize, cellSize);
                        var brush = room.Type == RoomType.Empty
                            ? this._grassBrush
                            : this._roomBrushes[room.Id % this._roomBrushes.Length];
                        g.DrawRectangle(brush, null, rect);
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
        }
    }
}
