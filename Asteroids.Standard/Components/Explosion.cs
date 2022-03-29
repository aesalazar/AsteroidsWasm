using System.Drawing;
using Asteroids.Standard.Helpers;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Screen explosion with sized incremented by current frame.
    /// </summary>
    internal sealed class Explosion
    {
        /// <summary>
        /// More points is more dazzling.
        /// </summary>
        private const int NumExpPoints = 22;

        /// <summary>
        /// Default life of explosion is 1/2 sec.
        /// </summary>
        private const int ExplosionLife = (int)ScreenCanvas.FramesPerSecond / 2;

        /// <summary>
        /// Creates a new instance of <see cref="Explosion"/>.
        /// </summary>
        /// <param name="ptExplosion">Origin point to start at.</param>
        /// <param name="timeFactor">Length of the explosion relative to the <see cref="ScreenCanvas.FramesPerSecond"/>.</param>
        public Explosion(Point ptExplosion, double timeFactor = ScreenCanvas.DefaultExplosionLength)
        {
            _framesRemaining = (int)(ExplosionLife * timeFactor);
            Points = new Point[NumExpPoints];
            Velocities = new Point[NumExpPoints];

            for (var i = 0; i < NumExpPoints; i++)
            {
                Points[i] = ptExplosion;
                Velocities[i] = new Point(
                    (int)((RandomizeHelper.Random.Next(1200) - 600) / ScreenCanvas.FramesPerSecond)
                    , (int)((RandomizeHelper.Random.Next(1200) - 600) / ScreenCanvas.FramesPerSecond)
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

            for (var i = 0; i < NumExpPoints; i++)
            {
                Points[i].X += Velocities[i].X;
                Points[i].Y += Velocities[i].Y;

                if (Points[i].X < 0)
                    Points[i].X = ScreenCanvas.CanvasWidth - 1;
                if (Points[i].X >= ScreenCanvas.CanvasWidth)
                    Points[i].X = 0;
                if (Points[i].Y < 0)
                    Points[i].Y = ScreenCanvas.CanvasHeight - 1;
                if (Points[i].Y >= ScreenCanvas.CanvasHeight)
                    Points[i].Y = 0;
            }

            _framesRemaining -= 1;
            return true;
        }
    }
}
