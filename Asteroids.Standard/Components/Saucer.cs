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
    /// Flying saucer to attack primary ship with guided missiles.
    /// </summary>
    class Saucer : ScreenObject
    {
        public const int MaximumPasses = 3;
        public const int KillScore = 1000;
        private const double Velocity = 3000 / FPS;

        private int _currentPass = 0;

        /// <summary>
        /// Guided <see cref="Missile"/> for targeting a <see cref="Ship"/>.
        /// </summary>
        public Missile Missile { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="Saucer"/>.
        /// </summary>
        /// <param name="location">Absolute origin (bottom-left) of the object.</param>
        /// <param name="canvas">Canvas to draw on.</param>
        public Saucer(Point location, ScreenCanvas canvas) : base(location, canvas)
        {
            SetVelocity();
        }

        /// <summary>
        /// Populates the base template collection of points to draw.
        /// </summary>
        protected override void InitPoints()
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
            //Stop if the next move will put it over the allow number of passes
            if (!IsAlive || currLoc.X + velocityX >= CanvasWidth && ++_currentPass >= MaximumPasses)
                return false;

            return base.Move();
        }

        /// <summary>
        /// Adjusts velocity to match the targeted <see cref="Ship"/> if it is
        /// <see cref="Ship.IsAlive()"/>, otherwise it continues straight.
        /// </summary>
        /// <param name="ship"><see cref="Ship"/> to target.</param>
        public void Target(Ship ship)
        {
            var isShip = ship?.IsAlive == true;
            var isMissile = Missile?.IsAlive == true;

            if (!isShip)
            {
                //No ship so simply move the missile if it exists
                if (isMissile)
                    Missile.Move();
            }
            else
            {
                //Make sure there is a missile and then target the ship
                if (!isMissile)
                    Missile = new Missile(this, Canvas);

                Missile.Move(ship);
            }
        }

        /// <summary>
        /// Updates the X-axis velocity.
        /// </summary>
        protected void SetVelocity()
        {
            var factor = currLoc.X < CanvasWidth / 2 ? 1 : -1;

            velocityX = factor * Velocity;
            velocityY = 0;
            PlaySound(this, ActionSound.Saucer);
        }

        /// <summary>
        /// Determine score if a point is in contact with the saucer.
        /// </summary>
        /// <param name="ptsCheck">Point collection to check.</param>
        /// <returns>Score of <see cref="KillScore"/> if inside; otherwise 0.</returns>
        public int CheckPointScore(IList<Point> ptsCheck)
        {
            return GetPoints().ContainsAnyPoint(ptsCheck) ? KillScore : 0;
        }

        /// <summary>
        /// Blow up the saucer.
        /// </summary>
        /// <param name="explosions">Explosion collection to add to.</param>
        public override void Explode(Explosions explosions)
        {
            base.Explode(explosions);
            Missile.Explode(explosions);
            PlaySound(this, ActionSound.Explode1);
        }

        /// <summary>
        /// Draw the Flying Saucer.
        /// </summary>
        public override void Draw()
        {
            if (!IsAlive)
                return;

            base.Draw();
            Missile?.Draw();
        }

        #region Statics

        private const int SizeLong = 300;
        private const int SizeMedium = SizeLong * 2 / 3;
        private const int SizeShort = SizeLong / 3;

        /// <summary>
        /// Non-transformed point template for creating a new flying saucer.
        /// </summary>
        private static IList<Point> PointsTemplate = new List<Point>();

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
