using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Asteroids.Standard.Interfaces;

namespace Asteroids.Wpf.Classes
{
    public class GraphicContainer : Canvas, IGraphicContainer
    {
        private readonly Dispatcher _mainDispatcher = Dispatcher.CurrentDispatcher;

        public async Task Draw(IEnumerable<IGraphicLine> lines, IEnumerable<IGraphicPolygon> polygons)
        {
            await _mainDispatcher.InvokeAsync(() =>
            {
                try
                {
                    Children.Clear();

                    foreach (var gline in lines)
                    {
                        var point1 = gline.Point1;
                        var point2 = gline.Point2;

                        var line = new  Line
                        {
                            X1 = point1.X,
                            Y1 = point1.Y,
                            X2 = point2.X,
                            Y2 = point2.Y,
                            Stroke = ColorHexToBrush(gline.ColorHex),
                            StrokeThickness = 1
                        };

                        Children.Add(line);
                    }

                    foreach (var gpoly in polygons)
                    {
                        var points = gpoly.Points;
                        var poly = new Polygon
                        {
                            Stroke = ColorHexToBrush(gpoly.ColorHex),
                            StrokeThickness = 1
                        };

                        var pts = points.ToList();
                        pts.ForEach(p => poly.Points.Add(new Point(p.X, p.Y)));
                        Children.Add(poly);
                    }
                }
                catch (Exception)
                {
                    //Ignore
                }
            });
        }

        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        #region Color Brush

        private string _lastColorHex;
        private SolidColorBrush _lastBrush;

        private SolidColorBrush ColorHexToBrush(string colorHex)
        {
            if (colorHex == _lastColorHex)
                return _lastBrush;

            _lastColorHex = colorHex;
            _lastBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(_lastColorHex);

            return _lastBrush;
        }

        #endregion
    }
}
