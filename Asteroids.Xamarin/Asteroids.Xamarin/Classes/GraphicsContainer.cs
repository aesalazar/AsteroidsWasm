using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Asteroids.Standard.Interfaces;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using IRegisterable = Xamarin.Forms.IRegisterable;

namespace Asteroids.Xamarin.Classes
{
    public class GraphicsContainer : SKCanvasView, IGraphicContainer, IRegisterable
    {
        private IEnumerable<IGraphicLine> _lastLines = new List<IGraphicLine>();
        private IEnumerable<IGraphicPolygon> _lastPolygons = new List<IGraphicPolygon>();

        public Task Initialize()
        {
            PaintSurface += OnPaintSurface;
            return Task.CompletedTask;
        }

        public Task Draw(IEnumerable<IGraphicLine> lines, IEnumerable<IGraphicPolygon> polygons)
        {
            _lastLines = lines;
            _lastPolygons = polygons;

            InvalidateSurface();

            return Task.CompletedTask;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();

            foreach (var gline in _lastLines)
            {
                var paint = new SKPaint
                {
                    Color = ColorHexToColor(gline.ColorHex),
                    IsStroke = true,
                };

                var p0 = new SKPoint(gline.Point1.X, gline.Point1.Y);
                var p1 = new SKPoint(gline.Point2.X, gline.Point2.Y);

                canvas.DrawLine(p0, p1, paint);
            }

            foreach (var gpoly in _lastPolygons)
            {
                   var paint = new SKPaint
                {
                    Color = ColorHexToColor(gpoly.ColorHex),
                    IsStroke = true,
                };

                var path = new SKPath();
                path.AddPoly(gpoly.Points.Select(p => new SKPoint(p.X, p.Y)).ToArray());

                canvas.DrawPath(path, paint);
            }
        }

        #region Color

        private string _lastColorHex;
        private SKColor _lastColor;

        private SKColor ColorHexToColor(string colorHex)
        {
            if (colorHex == _lastColorHex)
                return _lastColor;

            _lastColorHex = colorHex;

            var hex = colorHex.Replace("#", "");
            var length = hex.Length;

            var bytes = new byte[length / 2];

            for (int i = 0; i < length; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            _lastColor = new SKColor(bytes[0], bytes[1], bytes[2]);
            return _lastColor;
        }

        #endregion
    }
}
