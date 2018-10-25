using System;
using System.Collections.Generic;
using System.Drawing;
using Asteroids.Standard.Base;
using Asteroids.Standard.Helpers;
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

        public Asteroid(ASTEROID_SIZE size, ScreenCanvas canvas) : base(new Point(0, 0), canvas)
        {
            aSize = size;

            // Can't place the object randomly in constructor - stinky
            currLoc.X = Random.Next(2) * (CanvasWidth - 1);
            currLoc.Y = Random.Next(CanvasHeight - 1);

            RandomVelocity();

            // can't figure out how to have iSize set before
            // base constructor, which calls into InitPoints,
            // so clear and do it again
            InitPoints();
        }

        public Asteroid(Asteroid astCopy, ScreenCanvas canvas) : base(astCopy.currLoc, canvas)
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
            rotateSpeed = (Random.Next(10000) - 5000) / FPS;

            // choose a velocity for the asteroid (smaller asteroids can go faster)
            velocityX = ((Random.NextDouble() * 2000 - 1000) * ((ASTEROID_SIZE.LARGE - aSize + 1) * 1.05)) / FPS;
            velocityY = ((Random.NextDouble() * 2000 - 1000) * ((ASTEROID_SIZE.LARGE - aSize + 1) * 1.05)) / FPS;
        }

        public ASTEROID_SIZE ReduceSize()
        {
            if (aSize != ASTEROID_SIZE.DNE)
                aSize -= 1;

            InitPoints();
            RandomVelocity();

            return aSize;
        }

        protected override void InitPoints()
        {
            ClearPoints();

            switch (aSize)
            {
                case ASTEROID_SIZE.DNE:
                    AddPoints(PointsTemplateDne);
                    break;
                case ASTEROID_SIZE.SMALL:
                    AddPoints(PointsTemplateSmall);
                    break;
                case ASTEROID_SIZE.MEDIUM:
                    AddPoints(PointsTemplateMedium);
                    break;
                case ASTEROID_SIZE.LARGE:
                    AddPoints(PointsTemplateLarge);
                    break;
                default:
                    throw new NotImplementedException($"Asteroid Size '{aSize}'");
            }
        }

        public override bool Move()
        {
            // only draw things that are not available
            if (aSize != ASTEROID_SIZE.DNE)
                Rotate(rotateSpeed);

            return base.Move();
        }

        public override void Draw()
        {
            // only draw things that are not available
            if (aSize != ASTEROID_SIZE.DNE)
                base.Draw();
        }

        public bool CheckPointInside(Point ptCheck)
        {
            var dist = ptCheck.DistanceTo(currLoc);
            var size = (int)aSize * SIZE_INCR;
            return dist <= size;
        }

        #region Statics

        /// <summary>
        /// Non-transformed point template for creating a non-sized asteroid.
        /// </summary>
        private static IList<Point> PointsTemplateDne = new List<Point>();

        /// <summary>
        /// Non-transformed point template for creating a small-sized asteroid.
        /// </summary>
        private static IList<Point> PointsTemplateSmall = new List<Point>();

        /// <summary>
        /// Non-transformed point template for creating a medium-sized asteroid.
        /// </summary>
        private static IList<Point> PointsTemplateMedium = new List<Point>();

        /// <summary>
        /// Non-transformed point template for creating a large-sized asteroid.
        /// </summary>
        private static IList<Point> PointsTemplateLarge = new List<Point>();

        /// <summary>
        /// Setup the point templates.
        /// </summary>
        static Asteroid()
        {

            var addPoint = new Action<IList<Point>, double, ASTEROID_SIZE>((l, radPt, aSize) =>
            {
                l.Add(new Point(
                    (int)(Math.Sin(radPt) * -((int)aSize * SIZE_INCR))
                    , (int)(Math.Cos(radPt) * ((int)aSize * SIZE_INCR))
                ));
            });

            for (int i = 0; i < 9; i++)
            {
                double radPt = i * (360 / 9) * (Math.PI / 180);
                addPoint(PointsTemplateDne, radPt, ASTEROID_SIZE.DNE);
                addPoint(PointsTemplateSmall, radPt, ASTEROID_SIZE.SMALL);
                addPoint(PointsTemplateMedium, radPt, ASTEROID_SIZE.MEDIUM);
                addPoint(PointsTemplateLarge, radPt, ASTEROID_SIZE.LARGE);
            }
        }

        #endregion
    }
}
