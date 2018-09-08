using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Asteroids.Standard.Interfaces;

namespace Asteroids.Wpf.Classes
{
    public class GraphicContainer : Canvas, IGraphicContainer
    {
        private Dispatcher _mainDispatcher = Dispatcher.CurrentDispatcher;

        public async Task Draw(IEnumerable<IGraphicLine> lines, IEnumerable<IGraphicPolygon> polygons)
        {
            await _mainDispatcher.InvokeAsync(() =>
            {
                try
                {
                    Children.Clear();

                    foreach (var gline in lines)
                    {
                        var colorHex = gline.ColorHex;
                        var point1 = gline.Point1;
                        var point2 = gline.Point2;

                        var color = ColorTranslator.FromHtml(colorHex);
                        var line = new System.Windows.Shapes.Line();

                        line.X1 = point1.X;
                        line.Y1 = point1.Y;
                        line.X2 = point2.X;
                        line.Y2 = point2.Y;

                        var c = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
                        line.Stroke = new System.Windows.Media.SolidColorBrush(c);
                        line.StrokeThickness = 1;
                        Children.Add(line);
                    }

                    foreach (var gpoly in polygons)
                    {
                        var colorHex = gpoly.ColorHex;
                        var points = gpoly.Points;

                        var color = ColorTranslator.FromHtml(colorHex);
                        var poly = new System.Windows.Shapes.Polygon();

                        var c = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
                        poly.Stroke = new System.Windows.Media.SolidColorBrush(c);
                        poly.StrokeThickness = 1;

                        var pts = points.ToList();
                        pts.ForEach(p => poly.Points.Add(new System.Windows.Point(p.X, p.Y)));
                        Children.Add(poly);
                    }
                }
                catch (Exception)
                {
                    //Ignore
                }
            });
        }

        public async Task Initialize(Rectangle rectangle)
        {
            await SetDimensions(rectangle);
        }

        public Task SetDimensions(Rectangle rectangle)
        {
            return Task.CompletedTask;
        }
    }
}
