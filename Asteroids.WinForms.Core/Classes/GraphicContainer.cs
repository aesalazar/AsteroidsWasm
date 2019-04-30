using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Asteroids.Standard.Interfaces;

namespace Asteroids.WinForms.Core.Classes
{
    public class GraphicPictureBox : PictureBox, IGraphicContainer
    {
        private IEnumerable<IGraphicLine> _lastLines = new List<IGraphicLine>();
        private IEnumerable<IGraphicPolygon> _lastPolygons = new List<IGraphicPolygon>();

        public Task Initialize()
        {
            Paint += OnPaint;
            return Task.CompletedTask;
        }

        public Task Draw(IEnumerable<IGraphicLine> lines, IEnumerable<IGraphicPolygon> polygons)
        {
            Invalidate();
            _lastLines = lines;
            _lastPolygons = polygons;
            return Task.CompletedTask;

        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            foreach (var line in _lastLines)
                e.Graphics.DrawLine(ColorHexToPen(line.ColorHex), line.Point1, line.Point2);

            foreach (var poly in _lastPolygons)
                e.Graphics.DrawPolygon(ColorHexToPen(poly.ColorHex), poly.Points.ToArray());
        }

        #region Color Pen

        private string _lastColorHex;
        private Pen _lastPen;

        private Pen ColorHexToPen(string colorHex)
        {
            if (colorHex == _lastColorHex)
                return _lastPen;

            _lastColorHex = colorHex;
            _lastPen = new Pen(ColorTranslator.FromHtml(colorHex));

            return _lastPen;
        }

        #endregion
    }
}