using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        private enum STATE { ALIVE, EXPLODING, DONE };
        private STATE _state;
        private int _currentPass = 0;

        private Missile _missile;

        /// <summary>
        /// Creates a new instance of <see cref="Saucer"/>.
        /// </summary>
        /// <param name="location">Absolute origin (bottom-left) of the object.</param>
        /// <param name="canvas">Canvas to draw on.</param>
        public Saucer(Point location, ScreenCanvas canvas) : base(location, canvas)
        {
            _state = STATE.ALIVE;
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
            if (!IsAlive() || currLoc.X + velocityX >= CanvasWidth && ++_currentPass >= MaximumPasses)
            {
                _state = STATE.DONE;
                return false;
            }

            return base.Move();
        }

        /// <summary>
        /// Adjusts velocity to match the targeted <see cref="Ship"/> if it is
        /// <see cref="Ship.IsAlive()"/>, otherwise it continues straight.
        /// </summary>
        /// <param name="ship"><see cref="Ship"/> to target.</param>
        public void Target(Ship ship)
        {
            if (_missile == null)
                _missile = new Missile(this, Canvas);

            if (ship?.IsAlive() == true)
                _missile.Move(ship);
            else
                _missile.Move();
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
        /// Determine if a point is in contact with the saucer.
        /// </summary>
        /// <param name="ptCheck">Point to check.</param>
        /// <returns>Score of <see cref="KillScore"/> if inside; otherwise 0.</returns>
        public int CheckPointCollision(Point ptCheck)
        {
            var points = GetPoints()
                .Select(pt => new Point(
                    pt.X + currLoc.X
                    , pt.Y + currLoc.Y
                )).ToList();
                
            return ptCheck.IsInsidePolygon(points)
                ? KillScore
                : 0;
        }

        /// <summary>
        /// Blow up the saucer.
        /// </summary>
        /// <param name="explosions">Explosion collection to add to.</param>
        public void Explode(Explosions explosions)
        {
            _state = STATE.EXPLODING;
            velocityX = 0;
            velocityY = 0;

            var ptCheck = new Point(0);

            var points = GetPoints();
            foreach (var ptExp in points)
            {
                ptCheck.X = ptExp.X + currLoc.X;
                ptCheck.Y = ptExp.Y + currLoc.Y;
                explosions.AddExplosion(ptCheck);
            }

            PlaySound(this, ActionSound.Explode1);
        }

        /// <summary>
        /// Indicates if the Saucer is <see cref="STATE.ALIVE"/>
        /// </summary>
        public bool IsAlive()
        {
            return _state == STATE.ALIVE;
        }

        /// <summary>
        /// Draw the Flying Saucer.
        /// </summary>
        public override void Draw()
        {
            switch (_state)
            {
                case STATE.ALIVE:
                    base.Draw();
                    _missile?.Draw();
                    break;
            }
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
