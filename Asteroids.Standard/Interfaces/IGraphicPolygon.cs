using System.Collections.Generic;
using System.Drawing;

namespace Asteroids.Standard.Interfaces
{
    /// <summary>
    /// Polygon vector to render in the UI.
    /// </summary>
    public interface IGraphicPolygon
    {
        /// <summary>
        /// HTML color hex code (e.g. #000000)
        /// </summary>
        string ColorHex { get; }

        /// <summary>
        /// Collection of points to connect (non-closing)
        /// </summary>
        IList<Point> Points { get; }
    }
}
