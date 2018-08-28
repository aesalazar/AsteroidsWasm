using System;
using System.Collections;
using System.Drawing;
using Asteroids.Standard.Base;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Summary description for AsteroidBelt.
    /// </summary>
    class AsteroidBelt : CommonOps
    {
        const int SAFE_DISTANCE = 2000;
        protected ArrayList asteroids;

        public AsteroidBelt(int iNumAsteroids)
        {
            StartBelt(iNumAsteroids, Asteroid.ASTEROID_SIZE.LARGE);
        }

        public AsteroidBelt(int iNumAsteroids, Asteroid.ASTEROID_SIZE iMinSize)
        {
            StartBelt(iNumAsteroids, iMinSize);
        }

        public void StartBelt(int iNumAsteroids, Asteroid.ASTEROID_SIZE iMinSize)
        {
            asteroids = new ArrayList();
            Asteroid.ASTEROID_SIZE aAsteroidSize;
            for (int i = 0; i < iNumAsteroids; i++)
            {
                aAsteroidSize = Asteroid.ASTEROID_SIZE.LARGE - rndGen.Next(Asteroid.ASTEROID_SIZE.LARGE - iMinSize + 1);
                asteroids.Add(new Asteroid(aAsteroidSize));
            }
        }

        public int Count()
        {
            return asteroids.Count;
        }

        public void Move()
        {
            foreach (Asteroid asteroid in asteroids)
                asteroid.Move();
        }

        public bool IsCenterSafe()
        {
            bool bCenterSafe = true;
            Point ptAsteroid;
            foreach (Asteroid asteroid in asteroids)
            {
                ptAsteroid = asteroid.GetCurrLoc();
                if (Math.Sqrt(Math.Pow(ptAsteroid.X - iMaxX / 2, 2) + Math.Pow(ptAsteroid.Y - iMaxY / 2, 2)) <= SAFE_DISTANCE)
                {
                    bCenterSafe = false;
                    break;
                }
            }
            return bCenterSafe;
        }

        public void Draw(ScreenCanvas sc, int iPictX, int iPictY)
        {
            foreach (Asteroid asteroid in asteroids)
                asteroid.Draw(sc, iPictX, iPictY);
        }

        public int CheckPointCollisions(Point ptCheck)
        {
            int pointValue = 0;
            int iCount = asteroids.Count;
            for (int i = iCount - 1; i >= 0; i--)
            {
                if (((Asteroid)asteroids[i]).CheckPointInside(ptCheck))
                {
                    Asteroid.ASTEROID_SIZE sizeReduced = ((Asteroid)asteroids[i]).ReduceSize();
                    switch (sizeReduced)
                    {
                        case Asteroid.ASTEROID_SIZE.DNE:
                            pointValue = 250; // destroyed small - MEDIUM 100 pts
                            PlaySound(this, ActionSounds.Explode3);
                            asteroids.RemoveAt(i);
                            break;
                        case Asteroid.ASTEROID_SIZE.SMALL:
                            pointValue = 100; // destroyed large - MEDIUM 100 pts
                            PlaySound(this, ActionSounds.Explode2);
                            break;
                        case Asteroid.ASTEROID_SIZE.MEDIUM:
                            pointValue = 50; // destroyed large - 50 pts
                            PlaySound(this, ActionSounds.Explode1);
                            break;
                    }
                    // Add a new asteroid if it wasn't small
                    if (sizeReduced != Asteroid.ASTEROID_SIZE.DNE)
                        asteroids.Add(new Asteroid((Asteroid)asteroids[i]));

                    break;
                }
            }
            return pointValue;
        }
    }
}
