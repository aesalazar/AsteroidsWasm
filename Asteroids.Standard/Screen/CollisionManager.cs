using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Asteroids.Standard.Components;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Helpers;
using static Asteroids.Standard.Sounds.ActionSounds;

namespace Asteroids.Standard.Screen
{
    class CollisionManager
    {
        #region Fields and constructor

        private const int SAFE_DISTANCE = 2000;

        private readonly ScreenObjectCache _cache;
        private int _currentScore;
        

        /// <summary>
        /// Manages object collection and scoring for a <see cref="Game"/>.
        /// </summary>
        /// <param name="score">Score to update when a collection occurs.</param>
        /// <param name="belt">Asteroid belt to update when asteroids are hit.</param>
        /// <param name="bullets">Bullets to check for impacts and scoring.</param>
        public CollisionManager(ScreenObjectCache cache)
        {
            _cache = cache;
        }

        #endregion

        #region Prep and Completion

        /// <summary>
        /// Clears all current caches for a new set of collision checks.
        /// </summary>
        public void Reset()
        {
            _currentScore = 0;
        }

        /// <summary>
        /// Finalize updates to the <see cref="Score"/> and <see cref="AsteroidBelt"/>.
        /// </summary>
        public void Complete()
        {
            _cache.Score.AddScore(_currentScore);
            _cache.Belt.SetAsteroids(_cache.Asteroids.Select(ca => ca.ScreenObject).ToList());
        }

        #endregion

        #region Asteroids

        /// <summary>
        /// Determines if any point in a collection is in contact with an asteroid in the belt, 
        /// adjusts asteroid sizes, and keeps track of any scored points.
        /// </summary>
        /// <param name="pointsToCheck">Point collection to check.</param>
        /// <returns>Indication if a collection was made.</returns>
        public bool AsteroidBeltCollision(IList<Point> pointsToCheck)
        {
            //Get a copy for collection adds/removes
            var asteroids = _cache.Asteroids.ToList();
            var score = 0;

            //Go through each but break on first hit
            for (var i = asteroids.Count - 1; i >= 0; i--)
            {
                var asteroid = asteroids[i].ScreenObject;
                var location = asteroids[i].Location;
                var points = asteroids[i].PolygonPoints;

                //Got to next point if not a hit
                if (!AsteroidCollision(location, asteroid.Size, pointsToCheck))
                    continue;

                //Hit so reduce the asteroid size by one level
                var newSize = asteroid.ReduceSize();

                switch (newSize)
                {
                    case Asteroid.ASTEROID_SIZE.DNE:
                        score += 250;
                        PlaySound(this, ActionSound.Explode3);

                        //Destroyed so remove
                        _cache.Asteroids.RemoveAt(i);
                        break;

                    case Asteroid.ASTEROID_SIZE.SMALL:
                        score += 100;
                        PlaySound(this, ActionSound.Explode2);
                        break;

                    case Asteroid.ASTEROID_SIZE.MEDIUM:
                        score += 50;
                        PlaySound(this, ActionSound.Explode1);
                        break;
                }

                // Add a new asteroid if it wasn't small
                if (newSize != Asteroid.ASTEROID_SIZE.DNE)
                {
                    var add = new Asteroid(asteroid);
                    _cache.Asteroids.Add(new ScreenObjectCache.CachedObject<Asteroid>(add));
                }

                //Break out of loop
                break;
            }

            _currentScore += score;
            return score > 0;
        }

        /// <summary>
        /// Determine if a point is in contact with the asteroid.
        /// </summary>
        /// <param name="location">Current location of the</param>
        /// <param name="size">Point collection to check.</param>
        /// <param name="pointsToCheck">Point collection to check.</param>
        /// <returns>Indication if the point is inside the polygon.</returns>
        private bool AsteroidCollision(Point location, Asteroid.ASTEROID_SIZE size, IList<Point> pointsToCheck)
        {
            var inside = false;

            foreach (var ptCheck in pointsToCheck)
            {
                var dist = ptCheck.DistanceTo(location);
                var pixel = (int)size * Asteroid.SIZE_INCREMENT;

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
        public bool IsCenterSafe()
        {
            bool safe = true;

            foreach (var asteroid in _cache.Asteroids)
            {
                safe = asteroid
                    .Location
                    .DistanceTo(ScreenCanvas.CANVAS_WIDTH / 2, ScreenCanvas.CANVAS_HEIGHT / 2) <= SAFE_DISTANCE;

                if (!safe)
                    break;
            }

            return safe;
        }

        #endregion

        #region Saucer

        /// <summary>
        /// Determine score if a point is in contact with the saucer.
        /// </summary>
        /// <param name="pointsToCheck">Point collection to check.</param>
        public bool SaucerCollision(IList<Point> pointsToCheck)
        {
            if (_cache.SaucerPoints == null)
                return false;

            var saucerHit = _cache.SaucerPoints.ContainsAnyPoint(pointsToCheck);

            if (saucerHit)
            {
                _currentScore += Saucer.KillScore;

                foreach (var explosion in _cache.Saucer.Explode())
                    _cache.Explosions.Add(explosion);
            }

            return saucerHit;
        }

        #endregion

        #region Missile

        /// <summary>
        /// Determine if a polygon point collection is in contact with the <see cref="Missile"/>.
        /// </summary>
        /// <param name="polygonPoints">Polygon point collection to check.</param>
        public bool MissileCollision(IList<Point> polygonPoints)
        {
            if (_cache.MissilePoints == null)
                return false;

            var missilePts = _cache.Saucer.Missile.GetPoints();
            var missileHit = polygonPoints.ContainsAnyPoint(missilePts);

            if (missileHit)
                foreach (var explosion in _cache.Saucer.Missile.Explode())
                    _cache.Explosions.Add(explosion);

            return missileHit;
        }

        #endregion

        #region Explosions

        /// <summary>
        /// Adds a new explosion to the current queue.
        /// </summary>
        /// <param name="explosion"><see cref="Explosion"/> to load.</param>
        public void AddExplosion(Explosion explosion)
        {
            _cache.Explosions.Add(explosion);
        }

        /// <summary>
        /// Adds a new explosion to the current queue.
        /// </summary>
        /// <param name="point"><see cref="Point"/> to create new <see cref="Explosion"/> to load.</param>
        public void AddExplosion(Point point)
        {
            AddExplosion(new Explosion(point));
        }

        /// <summary>
        /// Adds a new explosions to the current queue.
        /// </summary>
        /// <param name="explosions"><see cref="Explosion"/>s to load.</param>
        public void AddExplosions(IList<Explosion> explosions)
        {
            foreach (var explosion in explosions)
                AddExplosion(explosion);
        }

        #endregion

        #region Move Objects

        /// <summary>
        /// Moves each <see cref="Bullet"/> in the cache.
        /// </summary>
        public void MoveBullets()
        {
            //Move does not use locks
            foreach (var bullet in _cache.BulletsInFlight)
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
        /// Moves each <see cref="Explosions"/> in the cache.
        /// </summary>
        public void MoveExplosions()
        {
            var explosions = _cache.Explosions;

            //Move does not use locks but have to account for removals
            for (int i = explosions.Count - 1; i >= 0; i--)
                if (!explosions[i].Move())
                    explosions.RemoveAt(i);
        }

        #endregion
    }
}
