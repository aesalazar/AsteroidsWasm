using System;
using Asteroids.Standard.Enums;

namespace Asteroids.Standard.Helpers
{
    /// <summary>
    /// Helper functions to generate random data.
    /// </summary>
    public static class RandomizeHelper
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
    }
}
