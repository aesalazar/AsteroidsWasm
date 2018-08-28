using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
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

        public void Activate()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                IsVisible = true;
                InvalidateSurface();
            });
        }

        public void DrawLine(string colorHex, Point point1, Point point2)
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
        }

        public void DrawPolygon(string colorHex, IEnumerable<Point> points)
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
        }

        public void Initialize(GameController controller, Rectangle rectangle)
        {
            _controller = controller;
            SetDimensions(rectangle);
            PaintSurface += OnPaitSurface;
        }

        private void OnPaitSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            _lastCanvas = e.Surface.Canvas;
            _lastCanvas.Clear();
            _controller.Repaint(this);
        }

        public void SetDimensions(Rectangle rectangle)
        {
        }

        private SKColor FromHex(string colorHex)
        {
            var argb = int.Parse(colorHex.Replace("#", ""), NumberStyles.HexNumber);

            return new SKColor((byte)((argb & -16777216) >> 0x18),
                                  (byte)((argb & 0xff0000) >> 0x10),
                                  (byte)((argb & 0xff00) >> 8),
                                  (byte)(argb & 0xff));
        }
    }
}
