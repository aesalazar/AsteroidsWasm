using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Asteroids.Standard.Interfaces;
using Ooui;

namespace Asteroids.Ooui.Classes
{
    public class GraphicsContainer : Canvas, IGraphicContainer
    {
        private CanvasRenderingContext2D _context;
        private string _lastColorHex;

        public Task Initialize()
        {
            _context = GetContext2D();
            _context.LineWidth = 2;
            _context.FillStyle = Colors.Clear;
            return Task.CompletedTask;
        }

        public Task Draw(IEnumerable<IGraphicLine> lines, IEnumerable<IGraphicPolygon> polygons)
        {
            _context.ClearRect(0, 0, Width, Height);
            _context.BeginPath();

            //Draw the lines
            foreach (var line in lines)
            {
                var colorHex = line.ColorHex;
                var point1 = line.Point1;
                var point2 = line.Point2;

                //If start of a new line color
                if (_lastColorHex != colorHex)
                {
                    _context.Stroke();
                    _context.BeginPath();
                    _context.StrokeStyle = colorHex;
                    _lastColorHex = colorHex;
                }

                //Connect the points
                _context.MoveTo(point1.X, point1.Y);
                _context.LineTo(point2.X, point2.Y);
            }

            //Draw polygons
            foreach (var poly in polygons)
            {
                var colorHex = poly.ColorHex;
                var points = poly.Points;

                //If start of a new line color
                if (_lastColorHex != colorHex)
                {
                    _context.Stroke();
                    _context.BeginPath();
                    _context.StrokeStyle = colorHex;
                    _lastColorHex = colorHex;
                }

                //Connect the points
                var first = points[0];
                _context.MoveTo(first.X, first.Y);

                foreach (var pt in points)
                    _context.LineTo(pt.X, pt.Y);

                _context.ClosePath();
            }

            //Commit and complete
            _context.Stroke();
            return Task.CompletedTask;
        }
    }
}
