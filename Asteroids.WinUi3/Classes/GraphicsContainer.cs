using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;
using Asteroids.WinUi3.Helpers;
using SkiaSharp;
using SkiaSharp.Views.Windows;

namespace Asteroids.WinUi3.Classes
{
    /// <summary>
    /// Canvas Control to paint vectors.
    /// </summary>
    internal sealed class GraphicsContainer : SKXamlCanvas, IGraphicContainer, IDisposable
    {
        private bool _isDisposed;
        private IDictionary<DrawColor, SKPaint> _colorCache = new Dictionary<DrawColor, SKPaint>();
        private IEnumerable<IGraphicLine> _lastLines = [];
        private IEnumerable<IGraphicPolygon> _lastPolygons = [];

        public Task Initialize(IDictionary<DrawColor, string> drawColorMap)
        {
            _colorCache = new ReadOnlyDictionary<DrawColor, SKPaint>(
                drawColorMap.ToDictionary(
                    kvp => kvp.Key
                    , kvp => ColorHelper.ColorHexToPaint(kvp.Value)
                )
            );

            PaintSurface += OnPaintSurface;
            return Task.CompletedTask;
        }

        public Task Draw(IEnumerable<IGraphicLine> lines, IEnumerable<IGraphicPolygon> polygons)
        {
            if (_isDisposed)
                return Task.CompletedTask;

            _lastLines = lines;
            _lastPolygons = polygons;

            Invalidate();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            PaintSurface -= OnPaintSurface;
            _isDisposed = true;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();

            foreach (var gline in _lastLines)
            {
                var p0 = new SKPoint(gline.Point1.X, gline.Point1.Y);
                var p1 = new SKPoint(gline.Point2.X, gline.Point2.Y);
                canvas.DrawLine(p0, p1, _colorCache[gline.Color]);
            }

            foreach (var gpoly in _lastPolygons)
            {
                var path = new SKPath();
                path.AddPoly(gpoly.Points.Select(p => new SKPoint(p.X, p.Y)).ToArray());
                canvas.DrawPath(path, _colorCache[gpoly.Color]);
            }
        }
    }
}
