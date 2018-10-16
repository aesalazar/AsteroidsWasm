using System;
using System.Drawing;
using Asteroids.Standard.Base;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Summary description for CAsteroid.
    /// </summary>
    class Asteroid : ScreenObject
    {
        const int SIZE_INCR = 220;
        public enum ASTEROID_SIZE { DNE = 0, SMALL, MEDIUM, LARGE }
        protected double rotateSpeed;
        protected ASTEROID_SIZE aSize;

        public Asteroid(ASTEROID_SIZE size) : base(new Point(0, 0))
        {
            aSize = size;

            // Can't place the object randomly in constructor - stinky
            currLoc.X = rndGen.Next(2) * (iMaxX - 1);
            currLoc.Y = rndGen.Next(iMaxY - 1);

            RandomVelocity();

            // can't figure out how to have iSize set before
            // base constructor, which calls into InitPoints,
            // so clear and do it again
            InitPoints();
        }

        public Asteroid(Asteroid astCopy) : base(astCopy.currLoc)
        {
            aSize = astCopy.aSize;
            RandomVelocity();

            // can't figure out how to have iSize set before
            // base constructor, which calls into InitPoints,
            // so clear and do it again                  
            InitPoints();
        }


        protected void RandomVelocity()
        {
            // choose random rotate speed
            rotateSpeed = (rndGen.Next(10000) - 5000) / FPS;
            // choose a velocity for the asteroid (smaller asteroids can go faster)
            velocityX = ((rndGen.NextDouble() * 2000 - 1000) * ((ASTEROID_SIZE.LARGE - aSize + 1) * 1.05)) / FPS;
            velocityY = ((rndGen.NextDouble() * 2000 - 1000) * ((ASTEROID_SIZE.LARGE - aSize + 1) * 1.05)) / FPS;
        }

        public ASTEROID_SIZE ReduceSize()
        {
            if (aSize != ASTEROID_SIZE.DNE)
                aSize -= 1;

            InitPoints();
            RandomVelocity();

            return aSize;
        }

        public override void InitPoints()
        {
            double radPt;

            ClearPoints();

            for (int i = 0; i < 9; i++)
            {
                radPt = i * (360 / 9) * (Math.PI / 180);
                AddPoint(new Point(
                    (int)(Math.Sin(radPt) * -((int)aSize * SIZE_INCR))
                    , (int)(Math.Cos(radPt) * ((int)aSize * SIZE_INCR))
                ));
            }
        }

        public override bool Move()
        {
            // only draw things that are not available
            if (aSize != ASTEROID_SIZE.DNE)
                Rotate(rotateSpeed);

            return base.Move();
        }

        public override void Draw(ScreenCanvas sc, int iPictX, int iPictY)
        {
            // only draw things that are not available
            if (aSize != ASTEROID_SIZE.DNE)
            {
                //Rotate(rotateSpeed);
                base.Draw(sc, iPictX, iPictY);
            }
        }

        public bool CheckPointInside(Point ptCheck)
        {
            // first check to see if point is close before we do more complicated check
            int dist = (int)Math.Sqrt(Math.Pow(ptCheck.X - currLoc.X, 2) + Math.Pow(ptCheck.Y - currLoc.Y, 2));
            if (Math.Sqrt(Math.Pow(ptCheck.X - currLoc.X, 2) + Math.Pow(ptCheck.Y - currLoc.Y, 2)) <= ((int)aSize * SIZE_INCR))
                return true;
            return false;
        }
    }
}
