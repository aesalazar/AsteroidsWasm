using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Asteroids.Standard.Interfaces;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Base;
using Asteroids.Standard.Screen;
using Asteroids.Standard;

namespace Asteroids.WinForms.Classes
{
    public class GraphicPictureBox : PictureBox, IGraphicContainer
    {
        private GameController _controller;
        private Graphics _lastGraphics;

        public void Initialize(GameController controller, Rectangle frameRectangle)
        {
            _controller = controller;
            SetDimensions(frameRectangle);
            Paint += OnPaint;
        }

        public void SetDimensions(Rectangle rectangle)
        {
            Top = rectangle.Top;
            Left = rectangle.Left;
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        public void Activate()
        {
            Invoke(new Action(() =>
            {
                try
                {
                    BringToFront();
                    Visible = true;
                }
                catch
                {
                    //ignore
                }
            }));
        }

        public void DrawLine(string colorHex, Point point1, Point point2)
        {
            var color = ColorTranslator.FromHtml(colorHex);
            _lastGraphics.DrawLine(new Pen(color), point1, point2);
        }

        public void DrawPolygon(string colorHex, IEnumerable<Point> points)
        {
            var color = ColorTranslator.FromHtml(colorHex);
            _lastGraphics.DrawPolygon(new Pen(color), points.ToArray());
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            _lastGraphics = e.Graphics;
            _controller.Repaint(this);
        }
    }
}
