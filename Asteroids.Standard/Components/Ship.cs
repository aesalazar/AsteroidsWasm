using System;
using System.Collections.Generic;
using System.Drawing;
using Asteroids.Standard.Base;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Helpers;
using Asteroids.Standard.Screen;
using static Asteroids.Standard.Sounds.ActionSounds;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Primary craft for the user to control.
    /// </summary>
    class Ship : ScreenObject
    {
        const double ROTATE_SPEED = 12000 / ScreenCanvas.FPS;

        /// <summary>
        /// Creates and immediately draws an instance of <see cref="Ship"/>.
        /// </summary>
        /// <param name="canvas">Canvas to draw on.</param>
        public Ship() : base(new Point(ScreenCanvas.CANVAS_WIDTH / 2, ScreenCanvas.CANVAS_HEIGHT / 2))
        {
            IsThrustOn = false;
            ExplosionLength = 2;
        }

        /// <summary>
        /// Initialize the internal point collections base on the template.
        /// </summary>
        protected override void InitPoints()
        {
            ClearPoints();
            AddPoints(PointsTemplate);
        }

        /// <summary>
        /// Indicates if the ship is currently accelerating via thrust.
        /// </summary>
        public bool IsThrustOn { get; private set; }

        /// <summary>
        /// Jump to another part of the canvas with a 10% of failure.
        /// </summary>
        /// <returns>Indication if the jump was considered a failure.</returns>
        public bool Hyperspace()
        {
            const int w = ScreenCanvas.CANVAS_WIDTH;
            const int h = ScreenCanvas.CANVAS_HEIGHT;

            currLoc.X = RandomizeHelper.Random.Next((int)(0.1 * w), (int)(0.9 * w));
            currLoc.Y = RandomizeHelper.Random.Next((int)(0.1 * h), (int)(0.9 * h));

            return RandomizeHelper.Random.Next(10) != 1;
        }

        /// <summary>
        /// Blows up the ship.
        /// </summary>
        /// <returns>Collection of the ships last location polygon.</returns>
        public override IList<Explosion> Explode()
        {
            PlaySound(this, ActionSound.Explode1);
            PlaySound(this, ActionSound.Explode2);
            PlaySound(this, ActionSound.Explode3);

            return base.Explode();
        }

        /// <summary>
        /// Reduces speed by 1 frame's worth.
        /// </summary>
        public void DecayThrust()
        {
            IsThrustOn = false;

            velocityX = velocityX * (1 - 1 / ScreenCanvas.FPS);
            velocityY = velocityY * (1 - 1 / ScreenCanvas.FPS);
        }

        /// <summary>
        /// Increase speed by 1 frame's worth.
        /// </summary>
        public void Thrust()
        {
            IsThrustOn = true;

            double SinVal = Math.Sin(radians);
            double CosVal = Math.Cos(radians);
            double addThrust = 90 / ScreenCanvas.FPS;
            double maxThrustSpeed = 5000 / ScreenCanvas.FPS;
            double incX, incY;

            incX = -(addThrust * SinVal);
            incY = addThrust * CosVal;

            velocityX += incX;
            if (velocityX > maxThrustSpeed)
                velocityX = maxThrustSpeed;
            if (velocityX < -maxThrustSpeed)
                velocityX = -maxThrustSpeed;
            velocityY += incY;
            if (velocityY > maxThrustSpeed)
                velocityY = maxThrustSpeed;
            if (velocityY < -maxThrustSpeed)
                velocityY = -maxThrustSpeed;

            PlaySound(this, ActionSound.Thrust);
        }

        /// <summary>
        /// Rotate the ship left by one frame's worth.
        /// </summary>
        public void RotateLeft()
        {
            Rotate(-ROTATE_SPEED);
        }

        /// <summary>
        /// Rotate the ship right by one frame's worth.
        /// </summary>
        public void RotateRight()
        {
            Rotate(ROTATE_SPEED);
        }

        #region Statics

        /// <summary>
        /// Non-transformed point template for creating a new ship.
        /// </summary>
        private static IList<Point> PointsTemplate = new List<Point>();

        /// <summary>
        /// Index location in <see cref="PointsTemplate"/> for thrust point 1.
        /// </summary>
        public static int PointThrust1 { get; }

        /// <summary>
        /// Index location in <see cref="PointsTemplate"/> for thrust point 2.
        /// </summary>
        public static int PointThrust2 { get; }

        /// <summary>
        /// Setup the <see cref="PointsTemplate"/>.
        /// </summary>
        static Ship()
        {
            const int shipWidthHalf = 100;
            const int shipHeightHalf = shipWidthHalf * 2;
            const int shipHeightInUp = (int)(shipHeightHalf * .6);
            const int shipWidthInSide = (int)(shipWidthHalf * .3);

            PointsTemplate.Add(new Point(0, -shipHeightHalf));
            PointsTemplate.Add(new Point(shipWidthHalf / 2, 0)); // midpoint for collisions
            PointsTemplate.Add(new Point(shipWidthHalf, shipHeightHalf));
            PointsTemplate.Add(new Point(shipWidthInSide, shipHeightInUp));
            PointsTemplate.Add(new Point(-shipWidthInSide, shipHeightInUp));
            PointsTemplate.Add(new Point(-shipWidthHalf, shipHeightHalf));
            PointsTemplate.Add(new Point(-shipWidthHalf / 2, 0)); // midpoint for collisions

            PointThrust1 = 3;
            PointThrust2 = 4;
        }

        #endregion
    }
}
