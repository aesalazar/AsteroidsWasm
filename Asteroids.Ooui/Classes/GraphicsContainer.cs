using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Asteroids.Standard;
using Asteroids.Standard.Interfaces;
using Ooui;

namespace Asteroids.Ooui.Classes
{
    public class GraphicsContainer : Canvas, IGraphicContainer
    {
        private GameController _controller;
        private CanvasRenderingContext2D _context;

        public void Initialize(GameController controller, Rectangle rectangle)
        {
            _controller = controller;

            _context = GetContext2D();
            _context.LineWidth = 2;
            _context.FillStyle = Colors.Clear;

            SetDimensions(rectangle);
        }

        public void Activate()
        {
            Clear();
            _controller.Repaint(this);
        }

        public void DrawLine(string colorHex, Point point1, Point point2)
        {
            _context.StrokeStyle = colorHex;

            _context.BeginPath();
            _context.LineTo(point1.X, point1.Y);
            _context.LineTo(point2.X, point2.Y);
            _context.Stroke();
        }

        public void DrawPolygon(string colorHex, IEnumerable<Point> points)
        {
            _context.StrokeStyle = colorHex;

            _context.BeginPath();

            var pts = points.ToList();

            foreach (var pt in pts)
                _context.LineTo(pt.X, pt.Y);

            var first = pts.First();
            _context.LineTo(first.X, first.Y);

            _context.Stroke();
        }

        public void SetDimensions(Rectangle rectangle)
        {
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        private void Clear()
        {
            _context.ClearRect(0, 0, Width, Height);
        }

    }
}
