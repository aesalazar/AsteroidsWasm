using System.Collections.Generic;
using Asteroids.Standard.Helpers;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Summary description for AsteroidBelt.
    /// </summary>
    internal sealed class AsteroidBelt
    {
        private readonly object _updateAsteroidsLock;
        private IList<Asteroid> _asteroids;

        public AsteroidBelt(int iNumAsteroids) 
            : this(iNumAsteroids, Asteroid.AsteroidSize.Large)
        {
        }

        public AsteroidBelt(int iNumAsteroids, Asteroid.AsteroidSize iMinSize)
        {
            _updateAsteroidsLock = new object();
            _asteroids = new List<Asteroid>();
            StartBelt(iNumAsteroids, iMinSize);
        }

        public void StartBelt(int iNumAsteroids, Asteroid.AsteroidSize iMinSize)
        {
            var minSize = Asteroid.AsteroidSize.Large - iMinSize + 1;
            var asteroids = new List<Asteroid>();

            for (int i = 0; i < iNumAsteroids; i++)
            {
                var size = Asteroid.AsteroidSize.Large - RandomizeHelper.Random.Next(minSize);
                asteroids.Add(new Asteroid(size));
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
