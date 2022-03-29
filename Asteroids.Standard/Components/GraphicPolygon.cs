﻿using System.Collections.Generic;
using System.Drawing;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;

namespace Asteroids.Standard.Components
{
    internal sealed class GraphicPolygon : IGraphicPolygon
    {
        public DrawColor Color { get; set; }

        public IList<Point> Points { get; set; }
    }
}
