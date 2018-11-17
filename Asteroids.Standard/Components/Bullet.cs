using System;
using System.Collections.Generic;
using System.Drawing;
using Asteroids.Standard.Base;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Bullet is a missile fired by an object (ship or UFO)
    /// </summary>
    class Bullet : ScreenObject
    {
        int iLife;
        const double speedPerSec = 1000 / ScreenCanvas.FPS;

        public Bullet() : base(new Point(0, 0))
        {
            iLife = 0;
        }

        protected override void InitPoints()
        {
            ClearPoints();
            AddPoints(PointsTemplate);
        }

        public bool IsAvailable => iLife == 0;

        public void Disable()
        {
            iLife = 0;
        }

        ///// <summary>
        ///// Determines if the bullet is <see cref="Available()"/>.
        ///// </summary>
        ///// <param name="location">Current <see cref="Point"/> location.</param>
        ///// <returns>Indication if the bullet is consided available.</returns>
        //public bool AcquireLoc(out Point location)
        //{
        //    location = currLoc;
        //    return !Available();
        //}

        /// <summary>
        /// Fire the bullet from a parent ship.
        /// </summary>
        /// <param name="parentShip">Parent <see cref="Ship"/> the bullet was fired from.</param>
        public void Shoot(Ship parentShip)
        {
            iLife = (int)ScreenCanvas.FPS; // bullets live 1 sec
            currLoc = parentShip.GetCurrLoc();
            radians = parentShip.GetRadians();

            double SinVal = Math.Sin(radians);
            double CosVal = Math.Cos(radians);

            velocityX = (int)(-100 * SinVal) + parentShip.GetVelocityX();
            velocityY = (int)(100 * CosVal) + parentShip.GetVelocityY();
        }


        /// <summary>
        /// Decrement the bullets life and move.
        /// </summary>
        /// <returns></returns>
        public override bool Move()
        {
            // only draw things that are not available
            if (IsAvailable)
                iLife -= 1;

            return base.Move();
        }

        //public override void Draw()
        //{
        //    // only draw things that are not available
        //    if (!Available())
        //        DrawPolygons(GetPoints(), GetRandomFireColor());
        //}

        #region Statics

        /// <summary>
        /// Non-transformed point template for creating a new bullet.
        /// </summary>
        private static IList<Point> PointsTemplate = new List<Point>();

        /// <summary>
        /// Setup the point templates.
        /// </summary>
        static Bullet()
        {
            const int bulletSize = 35;

            PointsTemplate.Add(new Point(0, -bulletSize));
            PointsTemplate.Add(new Point(bulletSize, 0));
            PointsTemplate.Add(new Point(0, bulletSize));
            PointsTemplate.Add(new Point(-bulletSize, 0));
        }

        #endregion
    }
}
