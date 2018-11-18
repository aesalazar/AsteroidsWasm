using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Asteroids.Standard.Base;
using Asteroids.Standard.Components;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Helpers;
using static Asteroids.Standard.Sounds.ActionSounds;

namespace Asteroids.Standard.Screen
{
    /// <summary>
    /// Core game engine that manages object interation and screen painting.
    /// </summary>
    public class Game : CommonOps
    {
        #region Fields and Constructor

        private const int SAUCER_SCORE = 1000;
        private const int PAUSE_INTERVAL = (int)ScreenCanvas.FPS;

        private readonly Score _score;
        private readonly TextDraw _textDraw;
        private readonly ScreenCanvas _canvas;

        private readonly ScreenObjectCache _cache;
        private readonly CollisionManager _collisionManager;
        private readonly DrawingManager _drawingManager;

        private bool _inProcess;
        private int _currentLevel;
        private bool _paused;
        private int _pauseTimer;

        private int _neededSaucerPoints = SAUCER_SCORE;

        public Game(Score score, TextDraw textDraw, ScreenCanvas canvas) : base()
        {
            _score = score;
            _textDraw = textDraw;
            _canvas = canvas;

            //Start with 4 asteroids
            _currentLevel = 4; 
            _inProcess = true;

            //Setup caches with a new ship
            _cache = new ScreenObjectCache(
                _score
                , new Ship()
                , new AsteroidBelt(_currentLevel)
                , new Explosions()
                , Enumerable.Range(0,4).Select(i => new Bullet()).ToList()
            );

            _collisionManager = new CollisionManager(_cache);
            _drawingManager = new DrawingManager(_cache, _canvas);

            //Unpaused
            _paused = false;
            _pauseTimer = PAUSE_INTERVAL;
        }

        #endregion

        #region Game State

        /// <summary>
        /// Indicates if the game is not processing.
        /// </summary>
        public bool IsDone()
        {
            return !_inProcess;
        }

        /// <summary>
        /// Sets paused indicators.
        /// </summary>
        public void Pause()
        {
            _pauseTimer = PAUSE_INTERVAL;
            _paused = !_paused;
        }

        #endregion

        #region Ship Controls

        private bool _isShipActive => !_paused && _cache.Ship.IsAlive;

        public void Thrust(bool bThrustOn)
        {
            if (!_isShipActive)
                return;

            _cache.Ship.DecayThrust();

            if (bThrustOn)
                _cache.Ship.Thrust();
        }

        public void Left()
        {
            if (_isShipActive)
                _cache.Ship.RotateLeft();
        }

        public void Right()
        {
            if (_isShipActive)
                _cache.Ship.RotateRight();
        }

        public void Hyperspace()
        {
            if (!_isShipActive)
                return;

            if (!_cache.Ship.Hyperspace())
                _collisionManager.AddExplosions(_cache.Ship.Explode());
        }

        public void Shoot()
        {
            if (_paused)
                return;

            if (_cache.Ship.IsAlive)
            {
                //Fire bullets that are not already moving
                foreach (var bullet in _cache.BulletsAvailable)
                {
                    bullet.ScreenObject.Shoot(_cache.Ship);
                    PlaySound(this, ActionSound.Fire);
                    return;
                }
            }
            else if (_cache.Explosions.Count() == 0 && _score.HasReserveShips())
            {
                _score.DecrementReserveShips();
                _cache.UpdatedShip(new Ship());
            }
        }

        #endregion

        //private bool CheckAsteroidHit(IList<Point> pointsToCheck)
        //{
        //    int pointValue = _belt.CheckPointCollisions(pointsToCheck);
        //    if (pointValue > 0)
        //    {
        //        _score.AddScore(pointValue);
        //        return true;
        //    }
        //    return false;
        //}

        //private bool CheckSaucerHit(IList<Point> pointsToCheck)
        //{
        //    if (_saucer == null || !_saucer.IsAlive)
        //        return false;

        //    var pointValue = _saucer.CheckPointScore(pointsToCheck);
        //    if (pointValue > 0)
        //    {
        //        _score.AddScore(pointValue);
        //        _saucer.Explode(_explosions);
        //        return true;
        //    }

        //    return false;
        //}

        //private bool CheckMissileHit(IList<Point> polygonPoints)
        //{
        //    if (_saucer == null || _saucer.Missile == null || !_saucer.Missile.IsAlive)
        //        return false;

        //    var missilePts = _saucer.Missile.GetPoints();
        //    var missileHit = polygonPoints.ContainsAnyPoint(missilePts);

        //    if (missileHit)
        //        _saucer.Missile.Explode(_explosions);

        //    return missileHit;
        //}

        #region Paint the Screen

        public void DrawScreen()
        {
            if (_paused)
            {
                // Pause flashes on and off
                if (_pauseTimer > PAUSE_INTERVAL / 2)
                {
                    _textDraw.DrawText(
                        "PAUSE"
                        , TextDraw.Justify.CENTER
                        , ScreenCanvas.CANVAS_HEIGHT / 3
                        , 200, 400
                    );
                }

                if (--_pauseTimer < 0)
                    _pauseTimer = PAUSE_INTERVAL;
            }
            else // Do all game processing if game is not paused
            {
                _collisionManager.Reset();
                var origScore = _score.CurrentScore;

                // If no ship displaying, after explosions are done
                // get a new one - or end the game
                if (!_cache.Ship.IsAlive && _cache.Explosions.Count() == 0)
                {
                    if (!_score.HasReserveShips())
                    {
                        // Game over
                        _inProcess = false;
                    }
                    else if (_collisionManager.IsCenterSafe())
                    {
                        _score.DecrementReserveShips();
                        _cache.UpdatedShip(new Ship());
                    }
                }

                // Create a new asteroid belt if no explosions and no asteroids
                if ((_cache.Explosions.Count() == 0) && _cache.Belt.Count() == 0)
                    _cache.UpdateBelt(new AsteroidBelt(++_currentLevel));

                // Move all objects starting with the ship
                _cache.Ship.Move();

                //Move the saucer and its missile
                if (_cache.Saucer != null)
                {
                    if (_cache.Saucer.Move())
                    {
                        //Aim for the ship
                        _cache.Saucer.Target(_cache.Ship.GetCurrLoc());
                    }
                    else
                    {
                        //Saucer has completed its passes
                        _cache.UpdateSaucer(null);
                        _neededSaucerPoints = SAUCER_SCORE;
                    }
                }

                //Move the bullets, asteroids, and explosions
                _collisionManager.MoveBullets();
                _collisionManager.MoveAsteroids();
                _collisionManager.MoveExplosions();

                // Check bullets for collisions        
                foreach (var bullet in _cache.BulletsInFlight)
                {
                    var points = new List<Point> { bullet.Location };

                    if (_collisionManager.AsteroidBeltCollision(points) 
                        || _collisionManager.SaucerCollision(points) 
                        || _collisionManager.MissileCollision(bullet.ScreenObject.GetPoints()))
                    {
                        _collisionManager.AddExplosion(bullet.Location);
                        bullet.ScreenObject.Disable();
                    }
                }

                // Check ship for collisions
                if (_cache.Ship.IsAlive)
                {
                    var shipPoints = _cache.ShipPoints;

                    if (_collisionManager.AsteroidBeltCollision(shipPoints)
                        || _collisionManager.MissileCollision(shipPoints)
                        || _collisionManager.AsteroidBeltCollision(shipPoints)
                        )
                        foreach (var explosion in _cache.Ship.Explode())
                            _collisionManager.AddExplosion(explosion);
                }

                //See if a bullet or the ship hit the saucer
                if (_cache.Saucer != null && !_cache.Saucer.IsAlive)
                {
                    _cache.UpdateSaucer(null);
                    _neededSaucerPoints = SAUCER_SCORE;
                }

                //See if the score is enough to show a suacer
                else if (_cache.Saucer == null)
                {
                    _neededSaucerPoints -= _score.CurrentScore - origScore;

                    if (_neededSaucerPoints <= 0)
                    {
                        var pt = new Point(
                            Random.Next(2) == 0 ? 0 : ScreenCanvas.CANVAS_WIDTH
                            , (Random.Next(10, 100) * ScreenCanvas.CANVAS_HEIGHT) / 100
                        );

                        _cache.UpdateSaucer(new Saucer(pt));
                    }
                }
            }

            //Commit the changes
            _collisionManager.Complete();
            _cache.Repopulate();

            // Draw all objects
            _drawingManager.DrawObjects();
            _score.Draw();
        }

        #endregion

    }
}
