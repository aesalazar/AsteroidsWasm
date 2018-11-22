using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Asteroids.Standard.Base;
using Asteroids.Standard.Components;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Managers
{
    /// <summary>
    /// Manages object collection and scoring for a <see cref="Game"/> in an effort
    /// to optimize locking and point manipulation.
    /// </summary>
    class CacheManager
    {
        #region Constructor

        /// <summary>
        /// Create a new instance of <see cref="CacheManager"/>.
        /// </summary>
        public CacheManager(ScoreManager score, Ship ship, AsteroidBelt belt, IList<Bullet> bullets)
        {
            Score = score;
            Ship = ship;
            Belt = belt;

            _explosions = new List<Explosion>();

            _bullets = bullets;
            BulletsInFlight = new List<CachedObject<Bullet>>();
            BulletsAvailable = new List<CachedObject<Bullet>>();

            Repopulate();
        }

        #endregion

        #region Objects

        //Read-only
        private readonly object _bulletLock = new object();
        private readonly object _explosionLock = new object();

        private readonly IList<Explosion> _explosions;
        private readonly IList<Bullet> _bullets;

        public ScoreManager Score { get; }

        //Live and die
        public Ship Ship { get; private set; }
        public Saucer Saucer { get; private set; }
        public AsteroidBelt Belt { get; private set; }

        //Optimize to avoid repeat traversal
        public IList<CachedObject<Asteroid>> Asteroids { get; private set; }
        public IList<Point> SaucerPoints { get; private set; }
        public IList<Point> MissilePoints { get; private set; }
        public IList<Point> ShipPoints { get; private set; }

        public IList<CachedObject<Bullet>> BulletsInFlight { get; private set; }
        public IList<CachedObject<Bullet>> BulletsAvailable { get; private set; }

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

            lock (_bulletLock)
            {
                BulletsInFlight.Clear();
                BulletsAvailable.Clear();

                foreach (var bullet in _bullets)
                {
                    if (bullet.IsInFlight)
                        BulletsInFlight.Add(new CachedObject<Bullet>(bullet));
                    else
                        BulletsAvailable.Add(new CachedObject<Bullet>(bullet));
                }
            }
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

            if (Saucer == null)
            {
                SaucerPoints = null;
                MissilePoints = null;
                return;
            }

            SaucerPoints = Saucer.GetPoints();
            UpdateMissile(Saucer.Missile);
        }

        /// <summary>
        /// Updates the missile cache.
        /// </summary>
        public void UpdateMissile(Missile missile)
        {
            MissilePoints = missile?.IsAlive == true
                ? missile.GetPoints()
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

        /// <summary>
        /// Adds a new <see cref="Asteroid"/> to <see cref="Asteroids"/> and
        /// updates <see cref="Belt"/>.
        /// </summary>
        public void AddAsteroid(Asteroid asteroid)
        {
            Asteroids.Add(new CachedObject<Asteroid>(asteroid));
            Belt.SetAsteroids(Asteroids.Select(c => c.ScreenObject).ToList());
        }

        /// <summary>
        /// Removes an <see cref="Asteroid"/> from <see cref="Asteroids"/> and
        /// updates <see cref="Belt"/>.
        /// </summary>
        public void RemoveAsteroid(int idx)
        {
            Asteroids.RemoveAt(idx);
            Belt.SetAsteroids(Asteroids.Select(c => c.ScreenObject).ToList());
        }

        #endregion

        #region Explosions

        /// <summary>
        /// Adds an explosion to the internal collection (thread-safe).
        /// </summary>
        public void AddExplosion(Explosion explosion)
        {
            lock (_explosionLock)
                _explosions.Add(explosion);
        }

        /// <summary>
        /// Adds an explosion based on a <see cref="Point"/> to the internal 
        /// collection (thread-safe).
        /// </summary>
        public void AddExplosion(Point location)
        {
            AddExplosion(new Explosion(location));
        }

        /// <summary>
        /// Adds a new explosions to the current queue.
        /// </summary>
        public void AddExplosions(IList<Explosion> explosions)
        {
            lock (_explosionLock)
                foreach (var explosion in explosions)
                    _explosions.Add(explosion);
        }

        /// <summary>
        /// Removes an explosion from the internal collection (thread-safe).
        /// </summary>
        /// <returns>Indication if the explosion was successfully removed.</returns>
        public bool RemoveExplosion(Explosion explosion)
        {
            lock (_explosionLock)
                return _explosions.Remove(explosion);
        }

        public IList<Explosion> GetExplosions()
        {
            lock (_explosionLock)
                return _explosions.ToList();
        }

        /// <summary>
        /// Current explosion count (thread-safe).
        /// </summary>
        public int ExplosionCount()
        {
            lock (_explosionLock)
                return _explosions.Count;
        }

        #endregion

        #region Classes

        /// <summary>
        /// <see cref="ScreenObject"/> currently in <see cref="CacheManager"/>.
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
