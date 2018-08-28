using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using Asteroids.Standard;
using Asteroids.Standard.Interfaces;

namespace Asteroids.Wpf.Classes
{
    public class GraphicContainer : Canvas, IGraphicContainer
    {
        private Dispatcher _mainDispatcher = Dispatcher.CurrentDispatcher;
        private GameController _controller;

        public void Activate()
        {
            _mainDispatcher.Invoke(() => {
                Children.Clear();
                Visibility = System.Windows.Visibility.Visible;
                _controller.Repaint(this);
            });
        }

        public void DrawLine(string colorHex, Point point1, Point point2)
        {
            var color = ColorTranslator.FromHtml(colorHex);
            var line = new System.Windows.Shapes.Line();
            Children.Add(line);

            line.X1 = point1.X;
            line.Y1 = point1.Y;
            line.X2 = point2.X;
            line.Y2 = point2.Y;

            var c = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
            line.Stroke = new System.Windows.Media.SolidColorBrush(c);
            line.StrokeThickness = 1;
        }

        public void DrawPolygon(string colorHex, IEnumerable<Point> points)
        {
            var color = ColorTranslator.FromHtml(colorHex);
            var poly = new System.Windows.Shapes.Polygon();
            Children.Add(poly);

            var c = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
            poly.Stroke = new System.Windows.Media.SolidColorBrush(c);
            poly.StrokeThickness = 1;

            var pts = points.ToList();
            pts.ForEach(p => poly.Points.Add(new System.Windows.Point(p.X, p.Y)));

        }

        public void Initialize(GameController controller, Rectangle rectangle)
        {
            _controller = controller;
            SetDimensions(rectangle);
        }

        public void SetDimensions(Rectangle rectangle)
        {
        }
    }
}
