using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Asteroids.Xamarin.Classes
{
    internal sealed class GraphicsContainer : SKCanvasView, IGraphicContainer, IRegisterable
    {
        private IDictionary<DrawColor, SKColor> _colorCache = new Dictionary<DrawColor, SKColor>();
        private IEnumerable<IGraphicLine> _lastLines = new List<IGraphicLine>();
        private IEnumerable<IGraphicPolygon> _lastPolygons = new List<IGraphicPolygon>();

        public Task Initialize(IDictionary<DrawColor, string> drawColorMap)
        {
            _colorCache = new ReadOnlyDictionary<DrawColor, SKColor>(
                drawColorMap.ToDictionary(
                    kvp => kvp.Key
                    , kvp => ColorHexToColor(kvp.Value)
                )
            );

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
                    Color = _colorCache[gline.Color],
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
                    Color = _colorCache[gpoly.Color],
                    IsStroke = true,
                }; 

                var path = new SKPath();
                path.AddPoly(gpoly.Points.Select(p => new SKPoint(p.X, p.Y)).ToArray());

                canvas.DrawPath(path, paint);
            }
        }

        #region Color

        private static SKColor ColorHexToColor(string colorHex)
        {
            var hex = colorHex.Replace("#", "");
            var length = hex.Length;

            var bytes = new byte[length / 2];

            for (var i = 0; i < length; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            return new SKColor(bytes[0], bytes[1], bytes[2]);
        }

        #endregion
    }
}
