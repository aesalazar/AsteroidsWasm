using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Asteroids.Standard;
using Asteroids.Standard.Interfaces;
using System.Threading.Tasks;

namespace Asteroids.WinForms.Classes
{
    public class GraphicPictureBox : PictureBox, IGraphicContainer
    {
        private GameController _controller;
        private Graphics _lastGraphics;

        public Task Initialize(GameController controller, Rectangle frameRectangle)
        {
            _controller = controller;
            SetDimensions(frameRectangle);
            Paint += OnPaint;
            return Task.CompletedTask;
        }

        public Task SetDimensions(Rectangle rectangle)
        {
            Top = rectangle.Top;
            Left = rectangle.Left;
            Width = rectangle.Width;
            Height = rectangle.Height;

            return Task.CompletedTask;
        }

        public Task Activate()
        {
            //trigger a repaint
            Invalidate();
            return Task.CompletedTask;
        }

        public Task DrawLine(string colorHex, Point point1, Point point2)
        {
            var color = ColorTranslator.FromHtml(colorHex);
            _lastGraphics.DrawLine(new Pen(color), point1, point2);
            return Task.CompletedTask;
        }

        public Task DrawPolygon(string colorHex, IEnumerable<Point> points)
        {
            var color = ColorTranslator.FromHtml(colorHex);
            _lastGraphics.DrawPolygon(new Pen(color), points.ToArray());
            return Task.CompletedTask;
        }

        private async void OnPaint(object sender, PaintEventArgs e)
        {
            _lastGraphics = e.Graphics;
            await _controller.Repaint(this);
        }
    }
}
