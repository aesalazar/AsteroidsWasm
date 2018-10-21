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
        /// Drawing canvas.
        /// </summary>
        public ScreenCanvas Canvas { get; }

        /// <summary>
        /// Refresh rate.
        /// </summary>
        public const double FPS = 60;

        /// <summary>
        /// Horizontal scale factor to avoid decimal errors.
        /// </summary>
        protected const int iMaxX = 10000;

        /// <summary>
        /// Vertical scale factor to avoid decimal errors.
        /// </summary>
        protected const int iMaxY = 7500;

        /// <summary>
        /// Static random number generator.
        /// </summary>
        protected static Random rndGen = new Random();
    }
}
