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
    /// Summary description for AsteroidBelt.
    /// </summary>
    class AsteroidBelt : CommonOps
    {
        //private const int SAFE_DISTANCE = 2000;

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

        //public void Move()
        //{
        //    var asteroids = new List<Asteroid>();

        //    lock (_updateAsteroidsLock)
        //        asteroids.AddRange(_asteroids);

        //    foreach (var asteroid in asteroids)
        //        asteroid.Move();
        //}

        //public bool IsCenterSafe()
        //{
        //    bool bCenterSafe = true;
        //    Point ptAsteroid;

        //    var asteroids = new List<Asteroid>();

        //    lock (_updateAsteroidsLock)
        //        asteroids.AddRange(_asteroids);

        //    foreach (var asteroid in asteroids)
        //    {
        //        ptAsteroid = asteroid.GetCurrLoc();
        //        if (ptAsteroid.DistanceTo(ScreenCanvas.CANVAS_WIDTH / 2, ScreenCanvas.CANVAS_HEIGHT / 2) <= SAFE_DISTANCE)
        //        {
        //            bCenterSafe = false;
        //            break;
        //        }
        //    }
        //    return bCenterSafe;
        //}

        //public void Draw()
        //{
        //    var asteroids = new List<Asteroid>();

        //    lock (_updateAsteroidsLock)
        //        asteroids.AddRange(_asteroids);

        //    foreach (var asteroid in asteroids)
        //        asteroid.Draw();
        //}

        //public int CheckPointCollisions(IList<Point> ptsCheck)
        //{
        //    //get the asteroids
        //    var asteroids = new List<Asteroid>();

        //    lock (_updateAsteroidsLock)
        //        asteroids.AddRange(_asteroids);

        //    //Go through each
        //    int pointValue = 0;

        //    for (int i = asteroids.Count - 1; i >= 0; i--)
        //    {
        //        if (asteroids[i].ContainsAnyPoint(ptsCheck))
        //        {
        //            Asteroid.ASTEROID_SIZE sizeReduced = asteroids[i].ReduceSize();
        //            switch (sizeReduced)
        //            {
        //                case Asteroid.ASTEROID_SIZE.DNE:
        //                    pointValue = 250; // destroyed small - MEDIUM 100 pts
        //                    PlaySound(this, ActionSound.Explode3);
        //                    asteroids.RemoveAt(i);
        //                    break;
        //                case Asteroid.ASTEROID_SIZE.SMALL:
        //                    pointValue = 100; // destroyed large - MEDIUM 100 pts
        //                    PlaySound(this, ActionSound.Explode2);
        //                    break;
        //                case Asteroid.ASTEROID_SIZE.MEDIUM:
        //                    pointValue = 50; // destroyed large - 50 pts
        //                    PlaySound(this, ActionSound.Explode1);
        //                    break;
        //            }
        //            // Add a new asteroid if it wasn't small
        //            if (sizeReduced != Asteroid.ASTEROID_SIZE.DNE)
        //                asteroids.Add(new Asteroid(asteroids[i]));

        //            break;
        //        }
        //    }

        //    //Update
        //    _asteroids = asteroids;

        //    return pointValue;
        //}
    }
}
