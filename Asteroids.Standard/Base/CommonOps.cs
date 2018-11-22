using System;
using Asteroids.Standard.Helpers;
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
        public CommonOps()
        {
        }

        /// <summary>
        /// Static random number generator.
        /// </summary>
        protected static Random Random = new Random();

        /// <summary>
        /// Generates a ranom color for any fire or explosion.
        /// </summary>
        /// <returns>Color hex string.</returns>
        protected static string GetRandomFireColor()
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
