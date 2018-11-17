using System;
using System.Collections.Generic;
using System.Drawing;
using Asteroids.Standard.Base;
using Asteroids.Standard.Enums;
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
        /// Indicates if the ship is currently acceleration via thrust.
        /// </summary>
        public bool IsThrustOn { get; private set; }

        /// <summary>
        /// Creates and immediately draws an instance of <see cref="Ship"/>.
        /// </summary>
        /// <param name="canvas">Canvas to draw on.</param>
        public Ship() : base(new Point(ScreenCanvas.CANVAS_WIDTH / 2, ScreenCanvas.CANVAS_HEIGHT / 2))
        {
            IsThrustOn = false;
        }

        protected override void InitPoints()
        {
            ClearPoints();
            AddPoints(PointsTemplate);
        }

        public bool Hyperspace()
        {
            bool bSafeHyperspace = Random.Next(10) != 1;
            currLoc.X = Random.Next((int)(ScreenCanvas.CANVAS_WIDTH * .8)) + (int)(ScreenCanvas.CANVAS_WIDTH * .1);
            currLoc.Y = Random.Next((int)(ScreenCanvas.CANVAS_HEIGHT * .8)) + (int)(ScreenCanvas.CANVAS_HEIGHT * .1);
            return bSafeHyperspace;
        }

        public override IList<Explosion> Explode()
        {
            PlaySound(this, ActionSound.Explode1);
            PlaySound(this, ActionSound.Explode2);
            PlaySound(this, ActionSound.Explode3);

            return base.Explode();
        }

        public void DecayThrust()
        {
            IsThrustOn = false;

            velocityX = velocityX * (1 - 1 / ScreenCanvas.FPS);
            velocityY = velocityY * (1 - 1 / ScreenCanvas.FPS);
        }

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

        public void RotateLeft()
        {
            Rotate(-ROTATE_SPEED);
        }

        public void RotateRight()
        {
            Rotate(ROTATE_SPEED);
        }

        //public override void Draw()
        //{
        //    if (!IsAlive)
        //        return;

        //    base.Draw();

        //    //Draw flame if thrust is on
        //    if (bThrustOn)
        //    {
        //        // We have points transformed so we know where the bottom of the ship is
        //        var alPoly = new List<Point>
        //        {
        //            Capacity = 3
        //        };

        //        var pts = GetPoints();

        //        alPoly.Add(pts[PointThrust1]);
        //        alPoly.Add(pts[PointThrust2]);

        //        int iThrustSize = Random.Next(200) + 100; // random thrust effect

        //        alPoly.Add(new Point(
        //            (pts[PointThrust1].X + pts[PointThrust2].X) / 2 + (int)(iThrustSize * Math.Sin(radians)),
        //            (pts[PointThrust1].Y + pts[PointThrust2].Y) / 2 + (int)(-iThrustSize * Math.Cos(radians))
        //        ));

        //        // Draw thrust directly to ScreenCanvas; it's not really part of the ship object
        //        DrawPolygons(alPoly, GetRandomFireColor());
        //    }
        //}

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
