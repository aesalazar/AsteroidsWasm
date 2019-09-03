using System;
using Asteroids.Standard.Colors;
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
        public static Random Random = new Random();

        /// <summary>
        /// Generates a random color for any fire or explosion.
        /// </summary>
        /// <returns>Random <see cref="DrawColor"/>.</returns>
        public static DrawColor GetRandomFireColor()
        {
            var idx = Random.Next(DrawColors.DrawColorList.Count);
            return DrawColors.DrawColorList[idx];
        }
    }
}
