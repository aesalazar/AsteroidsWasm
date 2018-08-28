using System.Drawing;
using Asteroids.Standard.Base;
using Asteroids.Standard.Components;
using Asteroids.Standard.Enums;

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
        private bool inProcess;
        private int iLevel;
        private bool paused;
        const int PAUSE_INTERVAL = (int)FPS;
        int iPauseTimer;

        public Game()
        {
            iLevel = 4; // start with 4 asteroids
            inProcess = true;
            ship = new Ship(true); // new game - we know ship is alive
            shipBullets = new Bullet[4];
            for (int i = 0; i < 4; i++)
                shipBullets[i] = new Bullet();
            asteroids = new AsteroidBelt(iLevel);
            explosions = new Explosions();
            paused = false;
            iPauseTimer = PAUSE_INTERVAL;
        }

        public bool Done()
        {
            return (!inProcess);
        }

        public void Thrust(bool bThrustOn)
        {
            if (!paused && ship.IsAlive())
            {
                ship.DecayThrust();
                if (bThrustOn)
                    ship.Thrust();
            }
        }

        public void Left()
        {
            if (!paused && ship.IsAlive())
                ship.RotateLeft();
        }

        public void Right()
        {
            if (!paused && ship.IsAlive())
                ship.RotateRight();
        }

        public void Hyperspace()
        {
            if (!paused && ship.IsAlive())
                if (!ship.Hyperspace())
                    ExplodeShip();
        }

        public void Shoot()
        {
            if (!paused && ship.IsAlive())
            {
                foreach (Bullet bullet in shipBullets)
                {
                    if (bullet.Available())
                    {
                        bullet.Shoot(ship.GetCurrLoc(), ship.GetRadians(), ship.GetVelocityX(), ship.GetVelocityY());
                        PlaySound(this, ActionSounds.Fire);
                        return;
                    }
                }
            }
        }

        public void Pause()
        {
            iPauseTimer = PAUSE_INTERVAL;
            paused = !paused;
        }

        private bool CheckPointInAsteroid(Point ptCheck, ref Score score)
        {
            int pointValue = asteroids.CheckPointCollisions(ptCheck);
            if (pointValue > 0)
            {
                score.AddScore(pointValue);
                return true;
            }
            return false;
        }

        private void ExplodeShip()
        {
            Point ptCheck = new Point(0);

            PlaySound(this, ActionSounds.Explode1);
            PlaySound(this, ActionSounds.Explode2);
            PlaySound(this, ActionSounds.Explode3);

            foreach (Point ptExp in ship.pointsTransformed)
            {
                ship.Explode();
                ptCheck.X = ptExp.X + ship.GetCurrLoc().X;
                ptCheck.Y = ptExp.Y + ship.GetCurrLoc().Y;
                explosions.AddExplosion(ptCheck, 2);
            }
        }

        public void DrawScreen(ScreenCanvas sc, int iPictX, int iPictY, ref Score score)
        {
            Point ptCheck = new Point(0);

            if (paused)
            {
                // Pause flashes on and off
                if (iPauseTimer > PAUSE_INTERVAL / 2)
                {
                    TextDraw.DrawText(sc, "PAUSE", TextDraw.Justify.CENTER,
                       iMaxY / 3, 200, 400, iPictX, iPictY);
                }
                if (--iPauseTimer < 0)
                    iPauseTimer = PAUSE_INTERVAL;
            }
            else // Do all game processing if game is not paused
            {
                // If no ship displaying, after explosions are done
                // get a new one - or end the game
                if (!ship.IsAlive() &&
                   (explosions.Count() == 0))
                {
                    if (!score.HasReserveShips())
                    {
                        // Game over
                        inProcess = false;
                    }
                    else
                    {
                        if (asteroids.IsCenterSafe())
                        {
                            score.GetNewShip();
                            ship = new Ship(true);
                        }
                    }
                }

                // Create a new asteroid belt if 
                // no explosions and no asteroids
                if ((explosions.Count() == 0) &&
                   (asteroids.Count() == 0))
                {
                    asteroids = new AsteroidBelt(++iLevel);
                }

                // Move all objects
                ship.Move();
                foreach (Bullet bullet in shipBullets)
                {
                    bullet.Move();
                }
                asteroids.Move();
                explosions.Move();

                // Check bullets for collisions         
                foreach (Bullet bullet in shipBullets)
                {
                    if (bullet.AcquireLoc(ref ptCheck) &&
                       CheckPointInAsteroid(ptCheck, ref score))
                    {
                        explosions.AddExplosion(ptCheck);
                        bullet.Disable();
                    }
                }

                // Check ship for collisions
                if (ship.IsAlive())
                {
                    foreach (Point ptInShip in ship.pointsTransformed)
                    {
                        ptCheck.X = ptInShip.X + ship.GetCurrLoc().X;
                        ptCheck.Y = ptInShip.Y + ship.GetCurrLoc().Y;
                        if (CheckPointInAsteroid(ptCheck, ref score))
                        {
                            ExplodeShip();
                            break;
                        }
                    }
                }
            }

            // Draw all objects
            ship.Draw(sc, iPictX, iPictY);
            foreach (Bullet bullet in shipBullets)
            {
                bullet.Draw(sc, iPictX, iPictY);
            }

            asteroids.Draw(sc, iPictX, iPictY);
            explosions.Draw(sc, iPictX, iPictY);

            // Draw the score
            score.Draw(sc, iPictX, iPictY);
        }
    }
}
