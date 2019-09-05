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
    internal class Bullet : ScreenObject
    {
        private int _remainingFrames;

        public Bullet() : base(new Point(0, 0))
        {
            _remainingFrames = 0;
            InitPoints();
        }

        /// <summary>
        /// Setup the point template for the bullet.
        /// </summary>
        private void InitPoints()
        {
            ClearPoints();
            AddPoints(PointsTemplate);
        }

        /// <summary>
        /// Indicates if the bullet is current shooting.
        /// </summary>
        public bool IsInFlight => _remainingFrames > 0;

        /// <summary>
        /// Prevents the bullet from being redrawn.
        /// </summary>
        public void Disable()
        {
            _remainingFrames = 0;
        }

        /// <summary>
        /// Fire the bullet from a parent ship.
        /// </summary>
        /// <param name="parentShip">Parent <see cref="Ship"/> the bullet was fired from.</param>
        public void Shoot(Ship parentShip)
        {
            _remainingFrames = (int)ScreenCanvas.FramesPerSecond; // bullets live 1 sec
            CurrLoc = parentShip.GetCurrLoc();
            Radians = parentShip.GetRadians();

            var sinVal = Math.Sin(Radians);
            var cosVal = Math.Cos(Radians);

            VelocityX = (int)(-100 * sinVal) + parentShip.GetVelocityX();
            VelocityY = (int)(100 * cosVal) + parentShip.GetVelocityY();
        }

        /// <summary>
        /// Decrement the bullets life and move.
        /// </summary>
        /// <returns></returns>
        public override bool Move()
        {
            // only draw if in flight
            if (!IsInFlight)
                return false;

            _remainingFrames -= 1;
            return base.Move();
        }

        #region Statics

        /// <summary>
        /// Non-transformed point template for creating a new bullet.
        /// </summary>
        private static readonly IList<Point> PointsTemplate = new List<Point>();

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
