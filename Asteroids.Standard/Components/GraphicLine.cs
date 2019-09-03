using System.Drawing;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;

namespace Asteroids.Standard.Components
{
    public class GraphicLine : IGraphicLine
    {
        public DrawColor Color { get; set; }

        public Point Point1 { get; set; }

        public Point Point2 { get; set; }
    }
}
