using System.Drawing;

namespace Asteroids.Standard.Interfaces
{
    /// <summary>
    /// Line vector to render in the UI.
    /// </summary>
    public interface IGraphicLine
    {
        /// <summary>
        /// HTML color hex code (e.g. #000000)
        /// </summary>
        string ColorHex { get; }

        /// <summary>
        /// Staring point.
        /// </summary>
        Point Point1 { get; }

        /// <summary>
        /// Ending point.
        /// </summary>
        Point Point2 { get; }
    }
}
