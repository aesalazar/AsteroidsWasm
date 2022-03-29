using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Asteroids.Standard.Components;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Helpers;
using Asteroids.Standard.Screen;
using static Asteroids.Standard.Managers.CacheManager;
using static Asteroids.Standard.Sounds.ActionSounds;

namespace Asteroids.Standard.Managers
{
    /// <summary>
    /// Manages object collection and scoring for a <see cref="Game"/>.
    /// </summary>
    internal sealed class CollisionManager
    {
        #region Fields and constructor

        private const int SafeDistance = 2000;
        private readonly CacheManager _cache;

        /// <summary>
        /// Creates a new instance of <see cref="CollisionManager"/>.
        /// </summary>
        /// <param name="cache"><see cref="CacheManager"/> to obtain objects from.</param>
        public CollisionManager(CacheManager cache)
        {
            _cache = cache;
        }

        #endregion

        #region Asteroids

        /// <summary>
        /// Determines if any point in a collection is in contact with one and only one asteroid 
        /// in the belt, adjusts asteroid sizes, and keeps track of any scored points.
        /// </summary>
        /// <param name="pointsToCheck">Point collection to check.</param>
        /// <returns>Indication if a collection.</returns>
        public bool AsteroidBeltCollision(IList<Point> pointsToCheck)
        {
            //Get a copy for collection adds/removes
            var asteroids = _cache.Asteroids.ToList();
            var score = 0;

            //Go through each but break on first hit
            for (var i = asteroids.Count - 1; i >= 0; i--)
            {
                var cache = asteroids[i];
                var asteroid = cache.ScreenObject;

                //Got to next point if not a hit
                if (!AsteroidCollision(cache, pointsToCheck))
                    continue;

                //Hit so reduce the asteroid size by one level
                var newSize = asteroid.ReduceSize();

                switch (newSize)
                {
                    case Asteroid.AsteroidSize.Dne:
                        score += 250;
                        PlaySound(this, ActionSound.Explode3);

                        //Destroyed so remove
                        _cache.RemoveAsteroid(i);
                        break;

                    case Asteroid.AsteroidSize.Small:
                        score += 100;
                        PlaySound(this, ActionSound.Explode2);
                        break;

                    case Asteroid.AsteroidSize.Medium:
                        score += 50;
                        PlaySound(this, ActionSound.Explode1);
                        break;

                    case Asteroid.AsteroidSize.Large:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException($"{nameof(Asteroid.AsteroidSize)}: {newSize.ToString()}");
                }

                // Add a new asteroid if it wasn't small
                if (newSize != Asteroid.AsteroidSize.Dne)
                    _cache.AddAsteroid(new Asteroid(asteroid));

                //Break out of loop
                break;
            }

            _cache.Score.AddScore(score);
            return score > 0;
        }

        /// <summary>
        /// Determine if any point in a collection is in contact with the asteroid.
        /// </summary>
        /// <param name="asteroid">Asteroid cached object.</param>>
        /// <param name="pointsToCheck">Object points to check for collision.</param>>
        /// <returns>Indication if the point is inside the polygon.</returns>
        private static bool AsteroidCollision(CachedObject<Asteroid> asteroid, IEnumerable<Point> pointsToCheck)
        {
            var inside = false;

            foreach (var ptCheck in pointsToCheck)
            {
                var dist = ptCheck.DistanceTo(asteroid.Location);
                var pixel = (int)asteroid.ScreenObject.Size * Asteroid.SizeIncrement;

                if (dist > pixel)
                    continue;

                inside = true;
                break;
            }

            return inside;
        }

        /// <summary>
        /// Determines if the center of the <see cref="AsteroidBelt"/> is clear to draw a <see cref="Ship"/>.
        /// </summary>
        /// <returns>Indication of the center of the canvase being safe.</returns>
        public bool IsCenterSafe()
        {
            bool safe = true;

            foreach (var asteroid in _cache.Asteroids)
            {
                var separation = asteroid
                    .Location
                    .DistanceTo(
                        ScreenCanvas.CanvasWidth / 2
                        , ScreenCanvas.CanvasHeight / 2
                    );

                safe = separation >= SafeDistance;

                if (!safe)
                    break;
            }

            return safe;
        }

        #endregion

        #region Saucer

        /// <summary>
        /// Registers a score and explosions if any point in a collection is within
        /// the boundaries of the <see cref="CacheManager.Saucer"/>.
        /// </summary>
        /// <param name="pointsToCheck">Point collection to check.</param>
        /// <returns>Indication if any point is within the Saucer.</returns>
        public bool SaucerCollision(IList<Point> pointsToCheck)
        {
            if (_cache.SaucerPoints == null)
                return false;

            var saucerHit = _cache.SaucerPoints.ContainsAnyPoint(pointsToCheck);

            if (saucerHit)
            {
                _cache.Score.AddScore(Saucer.KillScore);

                foreach (var explosion in _cache.Saucer.Explode())
                    _cache.AddExplosion(explosion);
            }

            return saucerHit;
        }

        #endregion

        #region Missile

        /// <summary>
        /// Registers explosions if any point in a collection is within
        /// the boundaries of the <see cref="Saucer.Missile"/>.
        /// </summary>
        /// <param name="polygonPoints">Point collection to check.</param>
        /// <returns>Indication if the point is within the missile.</returns>
        public bool MissileCollision(IList<Point> polygonPoints)
        {
            if (_cache.MissilePoints == null)
                return false;

            var missileHit = polygonPoints.ContainsAnyPoint(_cache.MissilePoints);

            if (missileHit)
                foreach (var explosion in _cache.Saucer.Missile.Explode())
                    _cache.AddExplosion(explosion);

            return missileHit;
        }

        #endregion

        #region Move Objects

        /// <summary>
        /// Moves each <see cref="Bullet"/> in the cache.
        /// </summary>
        public void MoveBullets()
        {
            //Move does not use locks
            foreach (var bullet in _cache.GetBulletsInFlight())
                bullet.ScreenObject.Move();
        }

        /// <summary>
        /// Moves each <see cref="Asteroid"/> in the cache.
        /// </summary>
        public void MoveAsteroids()
        {
            //Move does not use locks
            foreach (var asteroid in _cache.Asteroids)
                asteroid.ScreenObject.Move();
        }

        /// <summary>
        /// Moves each <see cref="Explosion"/>s in the cache and removes
        /// any that are complete.
        /// </summary>
        public void MoveExplosions()
        {
            var explosions = _cache.GetExplosions();

            //Move does not use locks but have to account for removals
            for (var i = explosions.Count - 1; i >= 0; i--)
            {
                var explosion = explosions[i];

                if (!explosion.Move())
                    _cache.RemoveExplosion(explosion);
            }
        }

        #endregion
    }
}
