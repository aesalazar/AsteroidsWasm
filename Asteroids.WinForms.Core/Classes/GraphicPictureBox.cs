using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;

namespace Asteroids.WinForms.Core.Classes
{
    public class GraphicPictureBox : PictureBox, IGraphicContainer
    {
        private IDictionary<DrawColor, Pen> _colorCache;
        private IEnumerable<IGraphicLine> _lastLines = new List<IGraphicLine>();
        private IEnumerable<IGraphicPolygon> _lastPolygons = new List<IGraphicPolygon>();

        public Task Initialize(IDictionary<DrawColor, string> drawColorMap)
        {
            _colorCache = new ReadOnlyDictionary<DrawColor, Pen>(
                drawColorMap.ToDictionary(
                    kvp => kvp.Key
                    , kvp => new Pen(ColorTranslator.FromHtml(kvp.Value))
                )
            );

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
                e.Graphics.DrawLine(_colorCache[line.Color], line.Point1, line.Point2);

            foreach (var poly in _lastPolygons)
                e.Graphics.DrawPolygon(_colorCache[poly.Color], poly.Points.ToArray());
        }
    }
}