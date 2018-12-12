using System;

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
        /// Generates a ranom color for any fire or explosion.
        /// </summary>
        /// <returns>Color hex string.</returns>
        public static string GetRandomFireColor()
        {
            string penDraw;

            switch (Random.Next(3))
            {
                case 0:
                    penDraw = ColorHexStrings.RedHex;
                    break;
                case 1:
                    penDraw = ColorHexStrings.YellowHex;
                    break;
                case 2:
                    penDraw = ColorHexStrings.OrangeHex;
                    break;
                default:
                    penDraw = ColorHexStrings.WhiteHex;
                    break;
            }

            return penDraw;
        }
    }
}
