using System;
using System.Collections.Generic;
using System.Drawing;
using Asteroids.Standard.Helpers;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Summary description for Asteroid.
    /// </summary>
    internal sealed class Asteroid : ScreenObjectBase
    {
        /// <summary>
        /// Current rotation speed of the asteroid.
        /// </summary>
        private double _rotateSpeed;

        /// <summary>
        /// Creates a new instance of <see cref="Asteroid"/>.
        /// </summary>
        /// <param name="size">Initial <see cref="AsteroidSize"/>.</param>
        public Asteroid(AsteroidSize size) : base(new Point(0, 0))
        {
            Size = size;

            // Can't place the object randomly in constructor - stinky
            CurrentLocation.X = RandomizeHelper.Random.Next(2) * (ScreenCanvas.CanvasWidth - 1);
            CurrentLocation.Y = RandomizeHelper.Random.Next(ScreenCanvas.CanvasHeight - 1);

            RandomVelocity();
            InitPoints();
        }

        /// <summary>
        /// Creates a new instance of <see cref="Asteroid"/>.
        /// </summary>
        /// <param name="asteroid"><see cref="Asteroid"/> to clone.</param>
        public Asteroid(Asteroid asteroid) : base(asteroid.CurrentLocation)
        {
            Size = asteroid.Size;
            RandomVelocity();

            // can't figure out how to have Size set before
            // base constructor, which calls into InitPoints,
            // so clear and do it again                  
            InitPoints();
        }

        /// <summary>
        /// Sets the rotational spine of the asteroid randomly based on its current <see cref="AsteroidSize"/>.
        /// </summary>
        private void RandomVelocity()
        {
            const double fps = ScreenCanvas.FramesPerSecond;
            var sizeFactor = (AsteroidSize.Large - Size + 1) * 1.05;

            // choose random rotate speed
            _rotateSpeed = (RandomizeHelper.Random.Next(10000) - 5000) / fps;

            // choose a velocity for the asteroid (smaller asteroids can go faster)
            VelocityX = (RandomizeHelper.Random.NextDouble() * 2000 - 1000) * sizeFactor / fps;
            VelocityY = (RandomizeHelper.Random.NextDouble() * 2000 - 1000) * sizeFactor / fps;
        }

        /// <summary>
        /// Current <see cref="AsteroidSize"/>.
        /// </summary>
        public AsteroidSize Size { get; private set; }

        /// <summary>
        /// Reduce the size by one level.
        /// </summary>
        /// <returns>The new reduce size.</returns>
        public AsteroidSize ReduceSize()
        {
            if (Size != AsteroidSize.Dne)
                Size -= 1;

            InitPoints();
            RandomVelocity();

            return Size;
        }

        /// <summary>
        /// Sets the point template based on asteroid size.
        /// </summary>
        private void InitPoints()
        {
            ClearPoints();

            switch (Size)
            {
                case AsteroidSize.Dne:
                    AddPoints(PointsTemplateDne);
                    break;
                case AsteroidSize.Small:
                    AddPoints(PointsTemplateSmall);
                    break;
                case AsteroidSize.Medium:
                    AddPoints(PointsTemplateMedium);
                    break;
                case AsteroidSize.Large:
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
            if (Size != AsteroidSize.Dne)
                Rotate(_rotateSpeed);

            return base.Move();
        }

        #region Statics

        /// <summary>
        /// Size of a screen asteroid.
        /// </summary>
        public enum AsteroidSize { Dne = 0, Small, Medium, Large }

        /// <summary>
        /// Increment between asteroids sizes.
        /// </summary>
        public const int SizeIncrement = 220;

        /// <summary>
        /// Non-transformed point template for creating a non-sized asteroid.
        /// </summary>
        private static readonly IList<Point> PointsTemplateDne = new List<Point>();

        /// <summary>
        /// Non-transformed point template for creating a small-sized asteroid.
        /// </summary>
        private static readonly IList<Point> PointsTemplateSmall = new List<Point>();

        /// <summary>
        /// Non-transformed point template for creating a medium-sized asteroid.
        /// </summary>
        private static readonly IList<Point> PointsTemplateMedium = new List<Point>();

        /// <summary>
        /// Non-transformed point template for creating a large-sized asteroid.
        /// </summary>
        private static readonly IList<Point> PointsTemplateLarge = new List<Point>();

        /// <summary>
        /// Setup the point templates.
        /// </summary>
        static Asteroid()
        {

            var addPoint = new Action<IList<Point>, double, AsteroidSize>((l, radPt, aSize) =>
            {
                l.Add(new Point(
                    (int)(Math.Sin(radPt) * -((int)aSize * SizeIncrement))
                    , (int)(Math.Cos(radPt) * ((int)aSize * SizeIncrement))
                ));
            });

            for (var i = 0; i < 9; i++)
            {
                var radPt = i * (360 / 9) * (Math.PI / 180);
                addPoint(PointsTemplateDne, radPt, AsteroidSize.Dne);
                addPoint(PointsTemplateSmall, radPt, AsteroidSize.Small);
                addPoint(PointsTemplateMedium, radPt, AsteroidSize.Medium);
                addPoint(PointsTemplateLarge, radPt, AsteroidSize.Large);
            }
        }

        #endregion
    }
}
