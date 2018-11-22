using System.Drawing;
using Asteroids.Standard.Base;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Screen explosion with sized incremented by current frame.
    /// </summary>
    class Explosion : CommonOps
    {
        private const int NUM_EXP_POINTS = 22; // more points is more dazzling
        private const int EXPLOSION_LIFE = (int)ScreenCanvas.FPS / 2; // default life of explosion is 1/2 sec

        /// <summary>
        /// Creates a new instance of <see cref="Explosion"/>.
        /// </summary>
        /// <param name="ptExplosion">Origin point to start at.</param>
        /// <param name="timeFactor">Legth of the explosion relative to the <see cref="ScreenCanvas.FPS"/>.</param>
        public Explosion(Point ptExplosion, double timeFactor = ScreenCanvas.DEFAULT_EXPLOSION_LENGTH)
        {
            _framesRemaining = (int)(EXPLOSION_LIFE * timeFactor);
            Points = new Point[NUM_EXP_POINTS];
            Velocities = new Point[NUM_EXP_POINTS];

            for (int i = 0; i < NUM_EXP_POINTS; i++)
            {
                Points[i] = ptExplosion;
                Velocities[i] = new Point(
                    (int)((Random.Next(1200) - 600) / ScreenCanvas.FPS)
                    , (int)((Random.Next(1200) - 600) / ScreenCanvas.FPS)
                );
            }
        }

        /// <summary>
        /// Current state of drawing points.
        /// </summary>
        public Point[] Points { get; }

        /// <summary>
        /// Velocity of each drawing point.
        /// </summary>
        public Point[] Velocities { get; }

        /// <summary>
        /// Number of refresh calls until complete.
        /// </summary>
        private int _framesRemaining;

        /// <summary>
        /// Moves the explosion particles by one frame if possible.
        /// </summary>
        /// <returns>Indication if the explosion could be move and not at EOF.</returns>
        public bool Move()
        {
            if (_framesRemaining <= 0)
                return false;

            for (int i = 0; i < NUM_EXP_POINTS; i++)
            {
                Points[i].X += Velocities[i].X;
                Points[i].Y += Velocities[i].Y;

                if (Points[i].X < 0)
                    Points[i].X = ScreenCanvas.CANVAS_WIDTH - 1;
                if (Points[i].X >= ScreenCanvas.CANVAS_WIDTH)
                    Points[i].X = 0;
                if (Points[i].Y < 0)
                    Points[i].Y = ScreenCanvas.CANVAS_HEIGHT - 1;
                if (Points[i].Y >= ScreenCanvas.CANVAS_HEIGHT)
                    Points[i].Y = 0;
            }

            _framesRemaining -= 1;
            return true;
        }
    }
}
