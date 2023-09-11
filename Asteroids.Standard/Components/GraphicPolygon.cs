using System.Collections.Generic;
using System.Drawing;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;

namespace Asteroids.Standard.Components
{
    internal sealed class GraphicPolygon : IGraphicPolygon
    {
        public GraphicPolygon(DrawColor color, IList<Point> points)
        {
            Color = color;
            Points = points;
        }

        public DrawColor Color { get; }

        public IList<Point> Points { get; }
    }
}
