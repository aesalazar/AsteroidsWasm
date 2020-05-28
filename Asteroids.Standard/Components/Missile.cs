using System;
using System.Collections.Generic;
using System.Drawing;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Guided missile to target a <see cref="Ship"/>.
    /// </summary>
    internal class Missile : ScreenObjectBase
    {
        private const double Velocity = 2000 / ScreenCanvas.FramesPerSecond;

        /// <summary>
        /// Creates a new instance of <see cref="Missile"/>.
        /// </summary>
        /// <param name="saucer">Parent <see cref="Saucer"/>.</param>
        public Missile(Saucer saucer) : base(saucer.GetCurrentLocation())
        {
            ExplosionLength = 1;
            InitPoints();
        }

        /// <summary>
        /// Setup the points template for the missile.
        /// </summary>
        private void InitPoints()
        {
            ClearPoints();
            AddPoints(PointsTemplate);
        }

        /// <summary>
        /// Moves in the direction of the target <see cref="Point"/>.
        /// </summary>
        /// <param name="target">Point to target.</param>
        /// <returns>Indication if the move was successful.</returns>
        public bool Move(Point target)
        {
            //point at the ship
            Align(target);

            //adjust velocity
            VelocityX = -Math.Sin(Radians) * Velocity;
            VelocityY = Math.Cos(Radians) * Velocity;

            return Move();
        }

        #region Statics

        /// <summary>
        /// Non-transformed point template for creating a new missile.
        /// </summary>
        private static readonly IList<Point> PointsTemplate = new List<Point>();

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
