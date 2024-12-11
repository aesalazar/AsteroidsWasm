using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;

namespace Asteroids.AvaloniaUi.Classes
{
    public sealed class GraphicsContainer : Canvas, IGraphicContainer
    {
        private readonly Dispatcher _mainDispatcher;
        private IDictionary<DrawColor, IBrush> _colorCache = new Dictionary<DrawColor, IBrush>();

        public GraphicsContainer()
        {
            _mainDispatcher = Dispatcher.UIThread;
            _colorCache = new Dictionary<DrawColor, IBrush>();
        }

        public Task Initialize(IDictionary<DrawColor, string> drawColorMap)
        {
            _colorCache = new ReadOnlyDictionary<DrawColor, IBrush>(
                drawColorMap.ToDictionary(
                    kvp => kvp.Key
                    , kvp => Brush.Parse(kvp.Value)
                    )
                );

            return Task.CompletedTask;
        }

        public async Task Draw(IEnumerable<IGraphicLine> lines, IEnumerable<IGraphicPolygon> polygons)
        {
            await _mainDispatcher.InvokeAsync(() =>
            {
                var children = new List<Control>();

                foreach (var gline in lines)
                {
                    var p0 = new Point(gline.Point1.X, gline.Point1.Y);
                    var p1 = new Point(gline.Point2.X, gline.Point2.Y);
                    var color = _colorCache[gline.Color];

                    var line = new Line
                    {
                        StartPoint = p0,
                        EndPoint = p1,
                        Stroke = color,
                        StrokeThickness = 1,
                    };

                    children.Add(line);
                }

                foreach (var gpoly in polygons)
                {
                    var points = gpoly.Points.Select(p => new Point(p.X, p.Y)).ToList();
                    var color = _colorCache[gpoly.Color];
                    var polygon = new Polygon
                    {
                        Points = points,
                        Stroke = color,
                        StrokeThickness = 1,
                    };

                    children.Add(polygon);
                }

                Children.Clear();
                Children.AddRange(children);
            });
        }
    }
}
