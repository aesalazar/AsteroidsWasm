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
        const double speedPerSec = 1000 / FPS;
        public Bullet() : base(new Point(0, 0))
        {
            iLife = 0;
        }

        protected override void InitPoints()
        {
            ClearPoints();
            AddPoints(PointsTemplate);
        }

        public bool Available()
        {
            return (iLife == 0);
        }

        public void Disable()
        {
            iLife = 0;
        }

        public bool AcquireLoc(ref Point ptLoc)
        {
            ptLoc = currLoc;
            return (!Available());
        }

        public void Shoot(Point locParent, double radParent,
           double velXParent, double velYParent)
        {
            iLife = (int)(FPS * 1); // bullets live 1 sec
            currLoc = locParent;
            radians = radParent;
            double SinVal = Math.Sin(radians);
            double CosVal = Math.Cos(radians);

            velocityX = (int)(-100 * SinVal) + velXParent;
            velocityY = (int)(100 * CosVal) + velYParent;
        }

        public new void Move()
        {
            // only draw things that are not available
            if (!Available())
            {
                iLife -= 1;
            }
            base.Move();
        }

        public new void Draw(ScreenCanvas sc, int iPictX, int iPictY)
        {
            // only draw things that are not available
            if (!Available())
            {
                base.Draw(sc, iPictX, iPictY, GetRandomFireColor());
            }
        }

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
