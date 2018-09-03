using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Asteroids.Standard;
using Asteroids.Standard.Interfaces;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Device = Xamarin.Forms.Device;
using IRegisterable = Xamarin.Forms.IRegisterable;

namespace Asteroids.Xamarin.Classes
{
    public class GraphicsContainer : SKCanvasView, IGraphicContainer, IRegisterable
    {
        private GameController _controller;
        private SKCanvas _lastCanvas;

        public Task Activate()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                IsVisible = true;
                InvalidateSurface();
            });

            return Task.CompletedTask;
        }

        public Task DrawLine(string colorHex, Point point1, Point point2)
        {
            var color = FromHex(colorHex);
            var p0 = new SKPoint(point1.X, point1.Y);
            var p1 = new SKPoint(point2.X, point2.Y);

            var paint = new SKPaint
            {
                Color = color,
                IsStroke = true,
            };

            _lastCanvas.DrawLine(p0, p1, paint);

            return Task.CompletedTask;
        }

        public Task DrawPolygon(string colorHex, IEnumerable<Point> points)
        {
            var color = FromHex(colorHex);
            var path = new SKPath();
            path.AddPoly(points.Select(p => new SKPoint(p.X, p.Y)).ToArray());

            var paint = new SKPaint
            {
                Color = color,
                IsStroke = true,
            };

            _lastCanvas.DrawPath(path, paint);

            return Task.CompletedTask;
        }

        public Task Initialize(GameController controller, Rectangle rectangle)
        {
            _controller = controller;
            SetDimensions(rectangle);
            PaintSurface += OnPaintSurface;

            return Task.CompletedTask;
        }

        private async void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            _lastCanvas = e.Surface.Canvas;
            _lastCanvas.Clear();
            await _controller.Repaint(this);
        }

        public Task SetDimensions(Rectangle rectangle)
        {
            return Task.CompletedTask;
        }

        private SKColor FromHex(string colorHex)
        {
            var hex = colorHex.Replace("#", "");
            var length = hex.Length;

            var bytes = new byte[length / 2];

            for (int i = 0; i < length; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            return new SKColor(bytes[0], bytes[1], bytes[2]);
        }
    }
}
