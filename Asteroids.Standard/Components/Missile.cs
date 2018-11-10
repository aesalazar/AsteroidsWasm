using System;
using System.Collections.Generic;
using System.Drawing;
using Asteroids.Standard.Base;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Guided missle to target a <see cref="Ship"/>.
    /// </summary>
    class Missile : ScreenObject
    {
        private const double Velocity = 2000 / FPS;

        /// <summary>
        /// Creates a new instace of <see cref="Missile"/>.
        /// </summary>
        /// <param name="saucer">Parent <see cref="Saucer"/>.</param>
        /// <param name="canvas"><see cref="ScreenCanvas"/> to draw on.</param>
        public Missile(Saucer saucer, ScreenCanvas canvas) : base(saucer.GetCurrLoc(), canvas)
        {
            ExplosionLength = 1;
        }

        protected override void InitPoints()
        {
            ClearPoints();
            AddPoints(PointsTemplate);
        }

        /// <summary>
        /// Moves in the direction of the target <see cref="Ship"/>.
        /// </summary>
        /// <param name="ship">Ship to target.</param>
        /// <returns>Indication if the move was successful.</returns>
        public bool Move(Ship ship)
        {
            //point at the ship
            Align(ship.GetCurrLoc());

            //adjust velocity
            velocityX = -Math.Sin(radians) * Velocity;
            velocityY = Math.Cos(radians) * Velocity;

            return Move();
        }

        public override void Draw()
        {
            if (!IsAlive)
                return;

            //Draw the ship
            base.Draw();

            //Add the thrust
            var alPoly = new List<Point>
            {
                Capacity = 3
            };

            var pts = GetPoints();

            alPoly.Add(pts[PointThrust1]);
            alPoly.Add(pts[PointThrust2]);

            int iThrustSize = Random.Next(30) + 20; // random thrust effect

            alPoly.Add(new Point(
                (pts[PointThrust1].X + pts[PointThrust2].X) / 2 + (int)(iThrustSize * Math.Sin(radians)),
                (pts[PointThrust1].Y + pts[PointThrust2].Y) / 2 + (int)(-iThrustSize * Math.Cos(radians))
            ));

            // Draw thrust directly to ScreenCanvas
            DrawPolygons(alPoly, GetRandomFireColor());
        }

        #region Statics

        /// <summary>
        /// Non-transformed point template for creating a new missile.
        /// </summary>
        private static IList<Point> PointsTemplate = new List<Point>();

        /// <summary>
        /// Index location in <see cref="PointsTemplate"/> for thrust point 1.
        /// </summary>
        private static int PointThrust1;

        /// <summary>
        /// Index location in <see cref="PointsTemplate"/> for thrust point 2.
        /// </summary>
        private static int PointThrust2;

        /// <summary>
        /// Setup the <see cref="PointsTemplate"/>.
        /// </summary>
        static Missile()
        {
            const int s = 12;

            PointsTemplate.Add(new Point(0, -4 * s));
            PointsTemplate.Add(new Point(s, -3 * s));
            PointsTemplate.Add(new Point(s, 3 * s));
            PointsTemplate.Add(new Point(2 * s, 4 * s));

            PointsTemplate.Add(new Point(s, 4 * s)); //Midpoint for thrust
            PointsTemplate.Add(new Point(-s, 4 * s)); //Midpoint for thrust

            PointsTemplate.Add(new Point(-2 * s, 4 * s));
            PointsTemplate.Add(new Point(-s, 3 * s));
            PointsTemplate.Add(new Point(-s, -3 * s));

            PointThrust1 = 4;
            PointThrust2 = 5;
        }

        #endregion
    }
}
