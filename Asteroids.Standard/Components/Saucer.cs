using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Screen;
using static Asteroids.Standard.Sounds.ActionSounds;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Flying saucer to attack primary ship with guided missiles.
    /// </summary>
    internal sealed class Saucer : ScreenObjectBase
    {
        public const int MaximumPasses = 3;
        public const int KillScore = 1000;
        private const double Velocity = 3000 / ScreenCanvas.FramesPerSecond;

        private int _currentPass;

        /// <summary>
        /// Creates a new instance of <see cref="Saucer"/>.
        /// </summary>
        /// <param name="location">Absolute origin (bottom-left) of the object.</param>
        public Saucer(Point location) : base(location)
        {
            ExplosionLength = 2;
            SetVelocity();
            InitPoints();
        }

        /// <summary>
        /// Guided <see cref="Missile"/> for targeting a <see cref="Ship"/>.
        /// </summary>
        public Missile Missile { get; private set; }

        /// <summary>
        /// Populates the base template collection of points to draw.
        /// </summary>
        private void InitPoints()
        {
            ClearPoints();
            AddPoints(PointsTemplate);
        }

        /// <summary>
        /// Move the saucer if it has not completed <see cref="MaximumPasses"/>.
        /// </summary>
        /// <returns>Indication if the move was completed.</returns>
        public override bool Move()
        {
            if (!IsAlive)
                return false;

            //Stop if the next move will put it over the allow number of passes
            var x = CurrentLocation.X + VelocityX;

            if ((x <= 0 || x >= ScreenCanvas.CanvasWidth)
                && (Interlocked.Increment(ref _currentPass) >= MaximumPasses))
                return false;

            return base.Move();
        }

        /// <summary>
        /// Moves <see cref="Missile"/> towards the <see cref="target"/>.
        /// </summary>
        /// <param name="target">
        /// <see cref="Point"/> to target; <see langword="null"/> moves <see cref="Missile"/> forward.</param>
        public void Target(Point? target)
        {
            var isMissile = Missile?.IsAlive == true;

            if (target.HasValue)
            {
                //Make sure there is a missile
                if (!isMissile)
                    Missile = new Missile(this);

                //move towards the target
                Missile.Move(target.Value);
            }
            else if (isMissile)
            {
                //No target but the missile is alive so move forward
                Missile.Move();
            }
        }

        /// <summary>
        /// Updates the X-axis velocity.
        /// </summary>
        private void SetVelocity()
        {
            var factor = CurrentLocation.X < ScreenCanvas.CanvasWidth / 2 ? 1 : -1;

            VelocityX = factor * Velocity;
            VelocityY = 0;
            PlaySound(this, ActionSound.Saucer);
        }

        /// <summary>
        /// Blow up the saucer.
        /// </summary>
        /// <returns>Explosion collection to add to.</returns>
        public override IList<Explosion> Explode()
        {
            PlaySound(this, ActionSound.Explode1);
            var explosions = base.Explode();

            if (Missile?.IsAlive == true)
                foreach (var exp in Missile.Explode())
                    explosions.Add(exp);

            return explosions;
        }

        #region Statics

        private const int SizeLong = 300;
        private const int SizeMedium = SizeLong * 2 / 4;
        private const int SizeShort = SizeLong / 4;

        /// <summary>
        /// Non-transformed point template for creating a new flying saucer.
        /// </summary>
        private static readonly IList<Point> PointsTemplate = new List<Point>();

        /// <summary>
        /// Setup the <see cref="Saucer"/>.
        /// </summary>
        static Saucer()
        {
            PointsTemplate.Add(new Point(-SizeLong, 0));
            PointsTemplate.Add(new Point(-SizeMedium, -SizeShort));
            PointsTemplate.Add(new Point(-SizeShort, -SizeShort));
            PointsTemplate.Add(new Point(-SizeShort, -SizeMedium));
            PointsTemplate.Add(new Point(SizeShort, -SizeMedium));
            PointsTemplate.Add(new Point(SizeShort, -SizeShort));
            PointsTemplate.Add(new Point(SizeMedium, -SizeShort));
            PointsTemplate.Add(new Point(SizeLong, 0));
            PointsTemplate.Add(new Point(SizeMedium, SizeShort));
            PointsTemplate.Add(new Point(-SizeMedium, SizeShort));
        }

        #endregion
    }
}
