using System;
using System.Drawing;
using Asteroids.Standard.Enums;

namespace Asteroids.Standard.Helpers
{
    /// <summary>
    /// Helper functions to generate random data.
    /// </summary>
    internal static class RandomizeHelper
    {
        /// <summary>
        /// Static random number generator.
        /// </summary>
        public static Random Random { get; } = new Random();

        /// <summary>
        /// Generates a random color for any fire or explosion.
        /// </summary>
        /// <returns>Random <see cref="DrawColor"/>.</returns>
        public static DrawColor GetRandomFireColor()
        {
            var idx = Random.Next(ColorHelper.DrawColorList.Count);
            return ColorHelper.DrawColorList[idx];
        }

        /// <summary>
        /// Generates a randomized <see cref="Point"/> based on a provided
        /// point with a variance applied.
        /// </summary>
        /// <param name="basePoint">Starting point to base new point off of.</param>
        /// <param name="maxVariance">Max variation to apply to the X and Y values.</param>
        /// <returns>Generated point.</returns>
        public static Point GetRandomPoint(Point basePoint, double maxVariance)
        {
            var varianceX = maxVariance
                * Random.NextDouble()
                * (Random.Next(2) % 2 == 0 ? 1 : -1);

            var varianceY = maxVariance
                * Random.NextDouble()
                * (Random.Next(2) % 2 == 0 ? 1 : -1);

            return new Point(
                Convert.ToInt32(basePoint.X * (1 + varianceX))
                , Convert.ToInt32(basePoint.Y * (1 + varianceY)));
        }
    }
}
