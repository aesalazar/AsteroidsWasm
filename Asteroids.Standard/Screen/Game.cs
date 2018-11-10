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
    /// Summary description for CGame.
    /// </summary>
    public class Game : CommonOps
    {
        private Ship ship;
        private Bullet[] shipBullets;
        private AsteroidBelt asteroids;
        private Explosions explosions;
        private Saucer _saucer;

        private bool inProcess;
        private int iLevel;
        private bool paused;
        private const int PAUSE_INTERVAL = (int)FPS;
        private int iPauseTimer;

        private const int SAUCER_SCORE = 1000;
        private int _neededSaucerPoints = SAUCER_SCORE;

        private readonly Score _score;
        private readonly TextDraw _textDraw;

        public Game(Score score, TextDraw textDraw, ScreenCanvas canvas) : base(canvas)
        {
            _score = score;
            _textDraw = textDraw;

            iLevel = 4; // start with 4 asteroids
            inProcess = true;
            ship = new Ship(canvas); // new game - we know ship is alive
            shipBullets = new Bullet[4];
            for (int i = 0; i < 4; i++)
                shipBullets[i] = new Bullet(canvas);
            asteroids = new AsteroidBelt(iLevel, canvas);
            explosions = new Explosions(canvas);
            paused = false;
            iPauseTimer = PAUSE_INTERVAL;
        }

        public bool Done()
        {
            return !inProcess;
        }

        public void Thrust(bool bThrustOn)
        {
            if (!paused && ship.IsAlive)
            {
                ship.DecayThrust();

                if (bThrustOn)
                    ship.Thrust();
            }
        }

        public void Left()
        {
            if (!paused && ship.IsAlive)
                ship.RotateLeft();
        }

        public void Right()
        {
            if (!paused && ship.IsAlive)
                ship.RotateRight();
        }

        public void Hyperspace()
        {
            if (!paused && ship.IsAlive)
                if (!ship.Hyperspace())
                    ship.Explode(explosions);
        }

        public void Shoot()
        {
            if (paused)
                return;

            if (ship.IsAlive)
            {
                var bullets = shipBullets.ToList();
                foreach (Bullet bullet in bullets)
                {
                    if (bullet.Available())
                    {
                        bullet.Shoot(ship);
                        PlaySound(this, ActionSound.Fire);
                        return;
                    }
                }
            }
            else if (explosions.Count() == 0 && _score.HasReserveShips())
            {
                _score.GetNewShip();
                ship = new Ship(Canvas);
            }
        }

        public void Pause()
        {
            iPauseTimer = PAUSE_INTERVAL;
            paused = !paused;
        }

        private bool CheckAsteroidHit(IList<Point> pointsToCheck)
        {
            int pointValue = asteroids.CheckPointCollisions(pointsToCheck);
            if (pointValue > 0)
            {
                _score.AddScore(pointValue);
                return true;
            }
            return false;
        }

        private bool CheckSaucerHit(IList<Point> pointsToCheck)
        {
            if (_saucer == null || !_saucer.IsAlive)
                return false;

            var pointValue = _saucer.CheckPointScore(pointsToCheck);
            if (pointValue > 0)
            {
                _score.AddScore(pointValue);
                _saucer.Explode(explosions);
                return true;
            }

            return false;
        }

        private bool CheckMissileHit(IList<Point> polygonPoints)
        {
            if (_saucer == null || _saucer.Missile == null || !_saucer.Missile.IsAlive)
                return false;

            var missilePts = _saucer.Missile.GetPoints();
            var missileHit = polygonPoints.ContainsAnyPoint(missilePts);

            if (missileHit)
                _saucer.Missile.Explode(explosions);

            return missileHit;
        }

        public void DrawScreen()
        {
            var bullets = shipBullets.ToList();

            if (paused)
            {
                // Pause flashes on and off
                if (iPauseTimer > PAUSE_INTERVAL / 2)
                {
                    _textDraw.DrawText("PAUSE", TextDraw.Justify.CENTER,
                       CanvasHeight / 3, 200, 400);
                }
                if (--iPauseTimer < 0)
                    iPauseTimer = PAUSE_INTERVAL;
            }
            else // Do all game processing if game is not paused
            {
                var origScore = _score.CurrentScore;

                // If no ship displaying, after explosions are done
                // get a new one - or end the game
                if (!ship.IsAlive && explosions.Count() == 0)
                {
                    if (!_score.HasReserveShips())
                    {
                        // Game over
                        inProcess = false;
                    }
                    else if (asteroids.IsCenterSafe())
                    {
                        _score.GetNewShip();
                        ship = new Ship(Canvas);
                    }
                }

                // Create a new asteroid belt if 
                // no explosions and no asteroids
                if ((explosions.Count() == 0) && asteroids.Count() == 0)
                    asteroids = new AsteroidBelt(++iLevel, Canvas);

                // Move all objects starting with the ship
                ship.Move();

                //Move the saucer and its missle
                if (_saucer != null)
                {
                    if (_saucer.Move())
                    {
                        //Aim for the ship
                        _saucer.Target(ship);
                    }
                    else
                    {
                        //Saucer has completed its passes
                        _saucer = null;
                        _neededSaucerPoints = SAUCER_SCORE;
                    }
                }

                //Move the bullets, asteroids, and explosions
                foreach (var bullet in bullets)
                    bullet.Move();

                asteroids.Move();
                explosions.Move();

                // Check bullets for collisions        
                var ptCheck = new Point(0);

                foreach (var bullet in bullets)
                {
                    if (!bullet.AcquireLoc(ref ptCheck))
                        continue;

                    var points = new List<Point> { ptCheck };
                    if (CheckAsteroidHit(points) || CheckSaucerHit(points) || CheckMissileHit(bullet.GetPoints()))
                    {
                        explosions.AddExplosion(ptCheck);
                        bullet.Disable();
                    }
                }

                // Check ship for collisions
                if (ship.IsAlive)
                {
                    var shipPoints = ship.GetPoints();

                    if (CheckSaucerHit(shipPoints) || CheckMissileHit(shipPoints) || CheckAsteroidHit(shipPoints))
                        ship.Explode(explosions);
                }

                //See if a bullet or the ship hit the saucer
                if (_saucer != null && !_saucer.IsAlive)
                {
                    _saucer = null;
                    _neededSaucerPoints = SAUCER_SCORE;
                }

                //See if the score is enough to show a suacer
                else if (_saucer == null)
                {
                    _neededSaucerPoints -= _score.CurrentScore - origScore;

                    if (_neededSaucerPoints <= 0)
                    {
                        var pt = new Point(
                            Random.Next(2) == 0 ? 0 : CanvasWidth
                            , (Random.Next(10, 100) * CanvasHeight) / 100
                        );

                        _saucer = new Saucer(pt, Canvas);
                    }
                }
            }

            // Draw all objects
            ship.Draw();
            _saucer?.Draw();

            foreach (var bullet in bullets)
                bullet.Draw();

            asteroids.Draw();
            explosions.Draw();

            // Draw the score
            _score.Draw();
        }
    }
}
