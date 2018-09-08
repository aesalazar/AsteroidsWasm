using System.Collections.Generic;
using System.Drawing;
using Asteroids.Standard.Interfaces;

namespace Asteroids.Standard.Components
{
    public class GraphicPolygon : IGraphicPolygon
    {
        public string ColorHex { get; set; }

        public IList<Point> Points { get; set; }
    }
}
