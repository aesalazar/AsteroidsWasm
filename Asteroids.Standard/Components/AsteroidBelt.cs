using System.Collections.Generic;
using Asteroids.Standard.Base;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Summary description for AsteroidBelt.
    /// </summary>
    class AsteroidBelt : CommonOps
    {
        private readonly object _updateAsteroidsLock;
        private IList<Asteroid> _asteroids;

        public AsteroidBelt(int iNumAsteroids) : this(iNumAsteroids, Asteroid.ASTEROID_SIZE.LARGE)
        {
        }

        public AsteroidBelt(int iNumAsteroids, Asteroid.ASTEROID_SIZE iMinSize) : base()
        {
            _updateAsteroidsLock = new object();
            StartBelt(iNumAsteroids, iMinSize);
        }

        public void StartBelt(int iNumAsteroids, Asteroid.ASTEROID_SIZE iMinSize)
        {
            var asteroids = new List<Asteroid>();
            Asteroid.ASTEROID_SIZE aAsteroidSize;
            for (int i = 0; i < iNumAsteroids; i++)
            {
                aAsteroidSize = Asteroid.ASTEROID_SIZE.LARGE - Random.Next(Asteroid.ASTEROID_SIZE.LARGE - iMinSize + 1);
                asteroids.Add(new Asteroid(aAsteroidSize));
            }

            lock (_updateAsteroidsLock)
                _asteroids = asteroids;
        }

        /// <summary>
        /// Gets current collection of <see cref="Asteroid"/>s in
        /// a thread-safe manner.
        /// </summary>
        /// <returns>Collection of asteroids.</returns>
        public IList<Asteroid> GetAsteroids()
        {
            lock (_updateAsteroidsLock)
                return new List<Asteroid>(_asteroids);
        }

        /// <summary>
        /// Overwrites the current collection of <see cref="Asteroid"/>s with a new set.
        /// </summary>
        /// <param name="newAsteroids">New collection to replace current.</param>
        public void SetAsteroids(IList<Asteroid> newAsteroids)
        {
            lock (_updateAsteroidsLock)
                _asteroids = new List<Asteroid>(newAsteroids);
        }

        /// <summary>
        /// Current asteroid count (thread-safe).
        /// </summary>
        public int Count()
        {
            lock (_updateAsteroidsLock)
                return _asteroids.Count;
        }

    }
}
