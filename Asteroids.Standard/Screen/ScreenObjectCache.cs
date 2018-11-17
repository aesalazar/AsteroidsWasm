using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Asteroids.Standard.Base;
using Asteroids.Standard.Components;

namespace Asteroids.Standard.Screen
{
    class ScreenObjectCache
    {
        #region Constructor

        /// <summary>
        /// Manages object collection and scoring for a <see cref="Game"/>.
        /// </summary>
        public ScreenObjectCache(Score score, Ship ship, AsteroidBelt belt, Explosions explosions, IList<Bullet> bullets)
        {
            Score = score;
            Ship = ship;
            Belt = belt;

            _explosions = explosions;
            _bullets = bullets;
            Repopulate();
        }

        #endregion

        #region Objects

        //Read-only
        private Explosions _explosions { get; }
        private IList<Bullet> _bullets;

        public Score Score { get; }

        //Live and die
        public Ship Ship { get; private set; }
        public Saucer Saucer { get; private set; }
        public AsteroidBelt Belt { get; private set; }

        //Optimize to avoid repeat traversal
        public IList<CachedObject<Asteroid>> Asteroids { get; private set; }
        public IList<CachedObject<Bullet>> Bullets { get; private set; }
        public IList<Explosion> Explosions { get; private set; }
        public IList<Point> SaucerPoints { get; private set; }
        public IList<Point> MissilePoints { get; private set; }
        public IList<Point> ShipPoints { get; private set; }

        #endregion

        #region Prep

        /// <summary>
        /// Resets all cache based on the current state of <see cref="ScreenObject"/>s.
        /// </summary>
        /// <param name="ship">Current ship.</param>
        /// <param name="belt">Current asteroid belt.</param>
        public void Repopulate()
        {
            UpdatedShip(Ship);
            UpdateSaucer(Saucer);
            UpdateBelt(Belt);

            Bullets = _bullets
                .Where(b => b.IsAvailable)
                .Select(b => new CachedObject<Bullet>(b))
                .ToList();

            Explosions = new List<Explosion>(_explosions.GetExplosions());
        }

        #endregion

        #region Mutable Objects

        /// <summary>
        /// Updates the Ship cache.
        /// </summary>
        public void UpdatedShip(Ship ship)
        {
            Ship = ship;

            ShipPoints = Ship?.IsAlive == true
                ? Ship.GetPoints()
                : null;
        }

        /// <summary>
        /// Updates both the saucer and missile cache.
        /// </summary>
        public void UpdateSaucer(Saucer saucer)
        {
            Saucer = saucer;
            SaucerPoints = Saucer?.GetPoints();

            MissilePoints = Saucer?.Missile?.IsAlive == true
                   ? Saucer.Missile.GetPoints()
                   : null;
        }

        /// <summary>
        /// Updates the <see cref="AsteroidBelt"/> and cached <see cref="Asteroids"/>.
        /// </summary>
        public void UpdateBelt(AsteroidBelt belt)
        {
            Belt = belt;

            Asteroids = Belt?
                .GetAsteroids()
                .Select(a => new CachedObject<Asteroid>(a))
                .ToList();
        }

        #endregion

        #region Classes

        /// <summary>
        /// <see cref="Asteroid"/> currently in <see cref="ScreenObjectCache"/>.
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        public class CachedObject<T> where T : ScreenObject
        {
            /// <summary>
            /// Creates a new instance of <see cref="CachedObject{T}"/>.
            /// </summary>
            /// <param name="screenObject">Reference to instance of <see cref="T"/>.</param>
            public CachedObject(T screenObject)
            {
                ScreenObject = screenObject;
                Location = ScreenObject.GetCurrLoc();
                PolygonPoints = ScreenObject.GetPoints();
            }

            /// <summary>
            /// Reference to instance of <see cref="Asteroid"/>.
            /// </summary>
            public T ScreenObject { get; }

            /// <summary>
            /// Current location of translated center;
            /// </summary>
            public Point Location { get; }

            /// <summary>
            /// Currrent collection of translated polygon points.
            /// </summary>
            public IList<Point> PolygonPoints { get; }
        }

        #endregion

    }
}
