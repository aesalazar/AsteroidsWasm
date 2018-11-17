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
        public CommonOps()
        {
        }

        /// <summary>
        /// Static random number generator.
        /// </summary>
        protected static Random Random = new Random();
    }
}
