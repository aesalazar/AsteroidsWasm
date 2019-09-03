using System.Drawing;
using Asteroids.Standard.Enums;

namespace Asteroids.Standard.Interfaces
{
    /// <summary>
    /// Line vector to render in the UI.
    /// </summary>
    public interface IGraphicLine
    {
        /// <summary>
        /// <see cref="DrawColor"/> for the graphic.
        /// </summary>
        DrawColor Color { get; }

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
