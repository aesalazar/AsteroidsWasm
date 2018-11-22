using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Asteroids.Standard.Base;
using Asteroids.Standard.Components;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Managers;
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

        private readonly ScoreManager _score;
        private readonly TextDraw _textDraw;
        private readonly ScreenCanvas _canvas;

        private readonly CacheManager _cache;
        private readonly CollisionManager _collisionManager;
        private readonly DrawingManager _drawingManager;

        private bool _inProcess;
        private int _currentLevel;
        private bool _paused;
        private int _pauseTimer;

        private int _neededSaucerPoints = SAUCER_SCORE;

        /// <summary>
        /// Creates new instance of <see cref="Game"/>.
        /// </summary>
        /// <param name="score"><see cref="ScoreManager"/> to keep track of points.</param>
        /// <param name="textDraw"><see cref="TextDraw"/> to write text objects to the <see cref="ScreenCanvas"/>.</param>
        /// <param name="canvas"><see cref="ScreenCanvas"/> to draw objects on.</param>
        public Game(ScoreManager score, TextDraw textDraw, ScreenCanvas canvas) : base()
        {
            _score = score;
            _textDraw = textDraw;
            _canvas = canvas;

            //Start with 4 asteroids
            _currentLevel = 4;
            _inProcess = true;

            //Setup caches with a new ship
            _cache = new CacheManager(
                _score
                , new Ship()
                , new AsteroidBelt(_currentLevel)
                , Enumerable.Range(0, 4).Select(i => new Bullet()).ToList()
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

        /// <summary>
        /// Main game-play routine to determine object state and screen refresh.
        /// </summary>
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
                var origScore = _score.CurrentScore;

                // If no ship displaying, after explosions are done
                // get a new one - or end the game
                var noExplosions = _cache.ExplosionCount() == 0;

                if (!_cache.Ship.IsAlive && noExplosions)
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
                if (noExplosions && _cache.Belt.Count() == 0)
                    _cache.UpdateBelt(new AsteroidBelt(++_currentLevel));

                // Move all objects starting with the ship
                _cache.Ship.Move();

                //Move the saucer and its missile
                if (_cache.Saucer != null)
                {
                    if (_cache.Saucer.Move())
                    {
                        //Aim for the ship oe nothing
                        var target = _cache.Ship.IsAlive
                            ? _cache.Ship.GetCurrLoc()
                            : default(Point?);

                        _cache.Saucer.Target(target);
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
                        _cache.AddExplosion(bullet.Location);
                        bullet.ScreenObject.Disable();
                    }
                }

                // Check ship for collisions
                if (_cache.Ship.IsAlive)
                {
                    var shipPoints = _cache.ShipPoints;

                    if (_collisionManager.SaucerCollision(shipPoints)
                        || _collisionManager.MissileCollision(shipPoints)
                        || _collisionManager.AsteroidBeltCollision(shipPoints)
                        )
                        foreach (var explosion in _cache.Ship.Explode())
                            _cache.AddExplosion(explosion);
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
                            , (Random.Next(10, 90) * ScreenCanvas.CANVAS_HEIGHT) / 100
                        );

                        _cache.UpdateSaucer(new Saucer(pt));
                    }
                }
            }

            //Commit the changes
            _cache.Repopulate();

            // Draw all objects
            _drawingManager.DrawObjects();
            _score.Draw();
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
                _cache.AddExplosions(_cache.Ship.Explode());
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
            else if (_cache.ExplosionCount() == 0 && _score.HasReserveShips())
            {
                _score.DecrementReserveShips();
                _cache.UpdatedShip(new Ship());
            }
        }

        #endregion
    }
}
