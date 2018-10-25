using System;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Base
{
    public abstract class CommonOps
    {
        public CommonOps(ScreenCanvas canvas)
        {
            Canvas = canvas;
        }

        /// <summary>
        /// Drawing canvas to which all heights and widths will be scaled.
        /// </summary>
        protected readonly ScreenCanvas Canvas;

        /// <summary>
        /// Refresh rate.
        /// </summary>
        public const double FPS = 60;

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
