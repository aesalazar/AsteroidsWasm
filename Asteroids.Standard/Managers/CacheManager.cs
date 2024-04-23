using Asteroids.Standard.Components;
using Asteroids.Standard.Screen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Asteroids.Standard.Managers
{
    /// <summary>
    /// Manages object collection and scoring for a <see cref="Game"/> in an effort
    /// to optimize locking and point manipulation.
    /// </summary>
    internal sealed class CacheManager
    {
        #region Constructor

        /// <summary>
        /// Create a new instance of <see cref="CacheManager"/>.
        /// </summary>
        public CacheManager(ScoreManager score, Ship? ship, AsteroidBelt belt, IList<Bullet> bullets)
        {
            Score = score;
            Ship = ship;
            Belt = belt;
            _bullets = bullets.ToList();

            _bulletLock = new object();
            _explosionLock = new object();
            _asteroidsLock = new object();

            _explosions = new List<Explosion>();
            _bulletsInFlight = new List<CachedObject<Bullet>>();
            _bulletsAvailable = new List<CachedObject<Bullet>>();
            _asteroids = new List<CachedObject<Asteroid>>();

            Repopulate();
        }

        #endregion

        #region Objects

        //Read-only
        private readonly object _bulletLock;
        private readonly object _explosionLock;
        private readonly object _asteroidsLock;

        private readonly List<Explosion> _explosions;
        private readonly List<Bullet> _bullets;
        private readonly List<CachedObject<Bullet>> _bulletsInFlight;
        private readonly List<CachedObject<Bullet>> _bulletsAvailable;
        private readonly List<CachedObject<Asteroid>> _asteroids;

        public ScoreManager Score { get; }

        /// <summary>
        /// Main Ship; <see langword=""="null"/> if it does not currently exist.
        /// </summary>
        public Ship? Ship { get; private set; }

        /// <summary>
        /// Attacking Saucer; <see langword=""="null"/> if it does not currently exist.
        /// </summary>
        public Saucer? Saucer { get; private set; }

        /// <summary>
        /// Belt for <see cref="Asteroid"/>s.
        /// </summary>
        public AsteroidBelt Belt { get; private set; }

        /// <summary>
        /// Collection of points associated with the <see cref="Saucer"/>, if present.
        /// </summary>
        public IList<Point>? SaucerPoints { get; private set; }

        /// <summary>
        /// Collection of points associated with the <see cref="Missile"/>, if present.
        /// </summary>
        public IList<Point>? MissilePoints { get; private set; }

        /// <summary>
        /// Collection of points associated with the <see cref="Ship"/>, if present.
        /// </summary>
        public IList<Point>? ShipPoints { get; private set; }

        #endregion

        #region Prep

        /// <summary>
        /// Resets all cache based on the current state of <see cref="ScreenObjectBase"/>s.
        /// </summary>
        public void Repopulate()
        {
            UpdateShip(Ship);
            UpdateSaucer(Saucer);
            UpdateBelt(Belt);

            lock (_bulletLock)
            {
                _bulletsInFlight.Clear();
                _bulletsAvailable.Clear();

                foreach (var bullet in _bullets)
                {
                    if (bullet.IsInFlight)
                        _bulletsInFlight.Add(new CachedObject<Bullet>(bullet));
                    else
                        _bulletsAvailable.Add(new CachedObject<Bullet>(bullet));
                }
            }
        }

        #endregion

        #region Mutable Objects

        /// <summary>
        /// Updates the Ship cache.
        /// </summary>
        public void UpdateShip(Ship? ship)
        {
            Ship = ship;

            ShipPoints = Ship?.IsAlive == true
                ? Ship.GetPoints()
                : null;
        }

        /// <summary>
        /// Updates both the saucer and missile cache.
        /// </summary>
        public void UpdateSaucer(Saucer? saucer)
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
        /// Updates the <see cref="AsteroidBelt"/> and cached <see cref="Asteroids"/>.
        /// </summary>
        public void UpdateBelt(AsteroidBelt belt)
        {
            Belt = belt;

            var list = Belt
                .GetAsteroids()
                .Select(a => new CachedObject<Asteroid>(a))
                .ToList();

            lock (_asteroidsLock)
            {
                _asteroids.Clear();
                _asteroids.AddRange(list);
            }
        }

        /// <summary>
        /// Returns the current collection of cached Asteroids in a thread-safe manor.
        /// </summary>
        /// <returns>Disconnected list of cached Asteroids.</returns>
        public IList<CachedObject<Asteroid>> GetAsteroids()
        {
            lock (_asteroidsLock)
            {
                return _asteroids.ToList();
            }
        }

        /// <summary>
        /// Adds a new <see cref="Asteroid"/> to <see cref="Asteroids"/> and
        /// updates <see cref="Belt"/>.
        /// </summary>
        public void AddAsteroid(Asteroid asteroid)
        {
            lock (_asteroidsLock)
            {
                _asteroids.Add(new CachedObject<Asteroid>(asteroid));
                Belt.SetAsteroids(_asteroids.Select(c => c.ScreenObject).ToList());
            }
        }

        /// <summary>
        /// Removes an <see cref="Asteroid"/> from <see cref="Asteroids"/> and
        /// updates <see cref="Belt"/>.
        /// </summary>
        public void RemoveAsteroid(int index)
        {
            lock (_asteroidsLock)
            {
                _asteroids.RemoveAt(index);
                Belt.SetAsteroids(_asteroids.Select(c => c.ScreenObject).ToList());
            }
        }

        /// <summary>
        /// Updates the missile cache.
        /// </summary>
        private void UpdateMissile(Missile? missile)
        {
            MissilePoints = missile?.IsAlive == true
                ? missile.GetPoints()
                : null;
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

        /// <summary>
        /// Gets collection of <see cref="Explosion"/>s in a thread-safe manner.
        /// </summary>
        /// <returns>Collection of explosions.</returns>
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

        #region Bullets

        /// <summary>
        /// Gets the current collection of Bullets in flight in a thread-safe manner.
        /// </summary>
        /// <returns>Collection of bullets in flight.</returns>
        public IList<CachedObject<Bullet>> GetBulletsInFlight()
        {
            lock (_bulletLock)
                return _bulletsInFlight.ToList();
        }

        /// <summary>
        /// Gets the current collection of Bullets not in flight in a thread-safe manner.
        /// </summary>
        /// <returns>Collection of bullets not in flight.</returns>
        public IList<CachedObject<Bullet>> GetBulletsAvailable()
        {
            lock (_bulletLock)
                return _bulletsAvailable.ToList();
        }

        #endregion

        #region Classes

        /// <summary>
        /// <see cref="ScreenObject"/> currently in <see cref="CacheManager"/>.
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        public class CachedObject<T> where T : ScreenObjectBase
        {
            /// <summary>
            /// Creates a new instance of <see cref="CachedObject{T}"/>.
            /// </summary>
            /// <param name="screenObject">Reference to instance of <see cref="T"/>.</param>
            public CachedObject(T screenObject)
            {
                ScreenObject = screenObject;
                Location = ScreenObject.GetCurrentLocation();
                PolygonPoints = ScreenObject.GetPoints();
            }

            /// <summary>
            /// Reference to instance of <see cref="T"/>.
            /// </summary>
            public T ScreenObject { get; }

            /// <summary>
            /// Current location of translated center.
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
