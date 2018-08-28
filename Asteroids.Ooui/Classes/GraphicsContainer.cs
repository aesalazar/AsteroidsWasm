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
        private CanvasRenderingContext2D context;

        public void Initialize(GameController controller, Rectangle rectangle)
        {
            _controller = controller;

            context = GetContext2D();
            context.LineWidth = 2;
            context.FillStyle = Colors.Clear;

            SetDimensions(rectangle);
        }

        public void Activate()
        {
            Clear();
            _controller.Repaint(this);
        }

        public void DrawLine(string colorHex, Point point1, Point point2)
        {
            context.StrokeStyle = colorHex;

            context.BeginPath();
            context.LineTo(point1.X, point1.Y);
            context.LineTo(point2.X, point2.Y);
            context.Stroke();
        }

        public void DrawPolygon(string colorHex, IEnumerable<Point> points)
        {
            context.StrokeStyle = colorHex;

            context.BeginPath();

            foreach (var pt in points)
                context.LineTo(pt.X, pt.Y);

            var first = points.First();
            context.LineTo(first.X, first.Y);

            context.Stroke();
        }

        public void SetDimensions(Rectangle rectangle)
        {
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        private void Clear()
        {
            context.ClearRect(0, 0, Width, Height);
        }

    }
}
