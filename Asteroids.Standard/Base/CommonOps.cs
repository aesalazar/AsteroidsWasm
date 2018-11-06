using System;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Base
{
    /// <summary>
    /// Base class for a <see cref="ScreenCanvas"/>-based concept.  Meaning that the concept
    /// can have a phsical manifestation on the canvas or interact with the canvas to
    /// draw objects.
    /// </summary>
    public abstract class CommonOps
    {
        /// <summary>
        /// Creates instance of <see cref="CommonOps"/>.
        /// </summary>
        /// <param name="canvas">Drawing canvas to which all heights and widths will be scaled.</param>
        public CommonOps(ScreenCanvas canvas)
        {
            Canvas = canvas;
        }

        /// <summary>
        /// Drawing canvas to which all heights and widths will be scaled.
        /// </summary>
        /// <remarks>
        /// Angle 0 is pointing "down", 90 is "left" on the canvas
        /// </remarks>
        protected readonly ScreenCanvas Canvas;

        /// <summary>
        /// Refresh rate.
        /// </summary>
        public const double FPS = 60;

        /// <summary>
        /// Conversion from degrees to radians.
        /// </summary>
        protected const double RADIANS_PER_DEGREE = Math.PI / 180;

        /// <summary>
        /// Amount of radians in a full circle (i.e. 360 degrees)
        /// </summary>
        protected const double RADIANS_PER_CIRCLE = Math.PI * 2;

        /// <summary>
        /// Horizontal width (effective) of the drawing plane.
        /// </summary>
        protected const int CanvasWidth = 10000;

        /// <summary>
        /// Vertical heigth (effective) of the drawing plane.
        /// </summary>
        protected const int CanvasHeight = 7500;

        /// <summary>
        /// Static random number generator.
        /// </summary>
        protected static Random Random = new Random();
    }
}
