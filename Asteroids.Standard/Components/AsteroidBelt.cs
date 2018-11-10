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
        private const int SAFE_DISTANCE = 2000;

        private readonly object _updateAsteroidsLock;
        private IList<Asteroid> _asteroids;

        public AsteroidBelt(int iNumAsteroids, ScreenCanvas canvas) : this(iNumAsteroids, canvas, Asteroid.ASTEROID_SIZE.LARGE)
        {
        }

        public AsteroidBelt(int iNumAsteroids, ScreenCanvas canvas, Asteroid.ASTEROID_SIZE iMinSize) : base(canvas)
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
                asteroids.Add(new Asteroid(aAsteroidSize, Canvas));
            }

            lock (_updateAsteroidsLock)
                _asteroids = asteroids;
        }

        public int Count()
        {
            return _asteroids.Count;
        }

        public void Move()
        {
            var asteroids = new List<Asteroid>();

            lock (_updateAsteroidsLock)
                asteroids.AddRange(_asteroids);

            foreach (var asteroid in asteroids)
                asteroid.Move();
        }

        public bool IsCenterSafe()
        {
            bool bCenterSafe = true;
            Point ptAsteroid;

            var asteroids = new List<Asteroid>();

            lock (_updateAsteroidsLock)
                asteroids.AddRange(_asteroids);

            foreach (var asteroid in asteroids)
            {
                ptAsteroid = asteroid.GetCurrLoc();
                if (ptAsteroid.DistanceTo(CanvasWidth / 2, CanvasHeight / 2) <= SAFE_DISTANCE)
                {
                    bCenterSafe = false;
                    break;
                }
            }
            return bCenterSafe;
        }

        public void Draw()
        {
            var asteroids = new List<Asteroid>();

            lock (_updateAsteroidsLock)
                asteroids.AddRange(_asteroids);

            foreach (var asteroid in asteroids)
                asteroid.Draw();
        }

        public int CheckPointCollisions(IList<Point> ptsCheck)
        {
            //get the asteroids
            var asteroids = new List<Asteroid>();

            lock (_updateAsteroidsLock)
                asteroids.AddRange(_asteroids);

            //Go through each
            var newAsteroids = new List<Asteroid>();
            int pointValue = 0;
            int iCount = asteroids.Count;

            for (int i = iCount - 1; i >= 0; i--)
            {
                if (asteroids[i].ContainsAnyPoint(ptsCheck))
                {
                    Asteroid.ASTEROID_SIZE sizeReduced = asteroids[i].ReduceSize();
                    switch (sizeReduced)
                    {
                        case Asteroid.ASTEROID_SIZE.DNE:
                            pointValue = 250; // destroyed small - MEDIUM 100 pts
                            PlaySound(this, ActionSound.Explode3);
                            asteroids.RemoveAt(i);
                            break;
                        case Asteroid.ASTEROID_SIZE.SMALL:
                            pointValue = 100; // destroyed large - MEDIUM 100 pts
                            PlaySound(this, ActionSound.Explode2);
                            break;
                        case Asteroid.ASTEROID_SIZE.MEDIUM:
                            pointValue = 50; // destroyed large - 50 pts
                            PlaySound(this, ActionSound.Explode1);
                            break;
                    }
                    // Add a new asteroid if it wasn't small
                    if (sizeReduced != Asteroid.ASTEROID_SIZE.DNE)
                        asteroids.Add(new Asteroid(asteroids[i], Canvas));

                    break;
                }
            }

            //Update
            _asteroids = asteroids;

            return pointValue;
        }
    }
}
