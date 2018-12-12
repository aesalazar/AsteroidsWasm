using System;
using System.Collections.Generic;
using System.Drawing;
using Asteroids.Standard.Base;
using Asteroids.Standard.Helpers;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Summary description for Asteroid.
    /// </summary>
    class Asteroid : ScreenObject
    {
        protected double rotateSpeed;

        /// <summary>
        /// Creates a new instance of <see cref="Asteroid"/>.
        /// </summary>
        /// <param name="size">Initial <see cref="ASTEROID_SIZE"/>.</param>
        public Asteroid(ASTEROID_SIZE size) : base(new Point(0, 0))
        {
            Size = size;

            // Can't place the object randomly in constructor - stinky
            currLoc.X = RandomizeHelper.Random.Next(2) * (ScreenCanvas.CANVAS_WIDTH - 1);
            currLoc.Y = RandomizeHelper.Random.Next(ScreenCanvas.CANVAS_HEIGHT - 1);

            RandomVelocity();

            // can't figure out how to have Size set before
            // base constructor, which calls into InitPoints,
            // so clear and do it again
            InitPoints();
        }

        /// <summary>
        /// Creates a new instance of <see cref="Asteroid"/>.
        /// </summary>
        /// <param name="asteroid"><see cref="Asteroid"/> to clone.</param>
        public Asteroid(Asteroid asteroid) : base(asteroid.currLoc)
        {
            Size = asteroid.Size;
            RandomVelocity();

            // can't figure out how to have Size set before
            // base constructor, which calls into InitPoints,
            // so clear and do it again                  
            InitPoints();
        }

        /// <summary>
        /// Sets the rotational spine of the asteroid randomly based on its current <see cref="ASTEROID_SIZE"/>.
        /// </summary>
        protected void RandomVelocity()
        {
            const double fps = ScreenCanvas.FPS;
            var sizeFactor = (ASTEROID_SIZE.LARGE - Size + 1) * 1.05;

            // choose random rotate speed
            rotateSpeed = (RandomizeHelper.Random.Next(10000) - 5000) / fps;

            // choose a velocity for the asteroid (smaller asteroids can go faster)
            velocityX = (RandomizeHelper.Random.NextDouble() * 2000 - 1000) * sizeFactor / fps;
            velocityY = (RandomizeHelper.Random.NextDouble() * 2000 - 1000) * sizeFactor / fps;
        }

        /// <summary>
        /// Current <see cref="ASTEROID_SIZE"/>.
        /// </summary>
        public ASTEROID_SIZE Size { get; private set; }

        /// <summary>
        /// Reduce the size by one level.
        /// </summary>
        /// <returns>The new reduce size.</returns>
        public ASTEROID_SIZE ReduceSize()
        {
            if (Size != ASTEROID_SIZE.DNE)
                Size -= 1;

            InitPoints();
            RandomVelocity();

            return Size;
        }

        /// <summary>
        /// Sets the point template based on asteroid size.
        /// </summary>
        protected override void InitPoints()
        {
            ClearPoints();

            switch (Size)
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
                    throw new NotImplementedException($"Asteroid Size '{Size}'");
            }
        }

        /// <summary>
        /// Rotates and moves the asteroid.
        /// </summary>
        /// <returns>Indication if the move was successful.</returns>
        public override bool Move()
        {
            // only draw things that are not available
            if (Size != ASTEROID_SIZE.DNE)
                Rotate(rotateSpeed);

            return base.Move();
        }

        #region Statics

        /// <summary>
        /// Size of a screen asteroid.
        /// </summary>
        public enum ASTEROID_SIZE { DNE = 0, SMALL, MEDIUM, LARGE }

        /// <summary>
        /// Increment between asteroids sizes.
        /// </summary>
        public const int SIZE_INCREMENT = 220;

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
                    (int)(Math.Sin(radPt) * -((int)aSize * SIZE_INCREMENT))
                    , (int)(Math.Cos(radPt) * ((int)aSize * SIZE_INCREMENT))
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
