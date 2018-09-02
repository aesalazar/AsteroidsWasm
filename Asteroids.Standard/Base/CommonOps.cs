using System;

namespace Asteroids.Standard.Base
{
    public abstract class CommonOps
    {
        public const double FPS = 60;

        protected const int iMaxX = 10000;
        protected const int iMaxY = 7500;
        protected static Random rndGen = new Random();
    }
}
