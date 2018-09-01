using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Asteroids.Standard;
using Asteroids.Standard.Interfaces;
using Ooui;

namespace Asteroids.Ooui.Classes
{
    public class GraphicsContainer : Canvas, IGraphicContainer
    {
        private GameController _controller;
        private CanvasRenderingContext2D _context;

        public Task Initialize(GameController controller, Rectangle rectangle)
        {
            _controller = controller;

            _context = GetContext2D();
            _context.LineWidth = 2;
            _context.FillStyle = Colors.Clear;

            return SetDimensions(rectangle);
        }

        public Task Activate()
        {
            Clear();
            return _controller.Repaint(this);
        }

        public Task DrawLine(string colorHex, Point point1, Point point2)
        {
            _context.StrokeStyle = colorHex;

            _context.BeginPath();
            _context.LineTo(point1.X, point1.Y);
            _context.LineTo(point2.X, point2.Y);
            _context.Stroke();

            return Task.CompletedTask;
        }

        public Task DrawPolygon(string colorHex, IEnumerable<Point> points)
        {
            _context.StrokeStyle = colorHex;

            _context.BeginPath();

            var pts = points.ToList();

            foreach (var pt in pts)
                _context.LineTo(pt.X, pt.Y);

            var first = pts.First();
            _context.LineTo(first.X, first.Y);

            _context.Stroke();

            return Task.CompletedTask;
        }

        public Task SetDimensions(Rectangle rectangle)
        {
            Width = rectangle.Width;
            Height = rectangle.Height;

            return Task.CompletedTask;
        }

        private void Clear()
        {
            _context.ClearRect(0, 0, Width, Height);
        }

    }
}
