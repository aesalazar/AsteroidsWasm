using System.Drawing;
using Asteroids.Standard.Base;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Summary description for Explosion.
    /// </summary>
    class Explosion : ScreenObject
    {
        const int NUM_EXP_POINTS = 22; // more points is more dazzling
        const int EXPLOSION_LIFE = (int)FPS / 2; // default life of explosion is 1/2 sec
        int lifeLeft;
        Point[] ptPoints;
        Point[] ptPointsVelocity;

        public Explosion(Point ptExplosion, double timeFactor, ScreenCanvas canvas) : base(ptExplosion, canvas)
        {
            lifeLeft = (int)(EXPLOSION_LIFE * timeFactor);
            ptPoints = new Point[NUM_EXP_POINTS];
            ptPointsVelocity = new Point[NUM_EXP_POINTS];
            for (int i = 0; i < NUM_EXP_POINTS; i++)
            {
                ptPoints[i] = ptExplosion;
                ptPointsVelocity[i] = new Point((int)((Random.Next(1200) - 600) / FPS),
                                                (int)((Random.Next(1200) - 600) / FPS));
            }
        }

        protected override void InitPoints()
        {
            // Explosions are self-drawn
            // so do not initialize points in the base class
        }

        public override bool Move()
        {
            if (lifeLeft > 0)
            {
                for (int i = 0; i < NUM_EXP_POINTS; i++)
                {
                    ptPoints[i].X += ptPointsVelocity[i].X;
                    ptPoints[i].Y += ptPointsVelocity[i].Y;

                    if (ptPoints[i].X < 0)
                        ptPoints[i].X = CanvasWidth - 1;
                    if (ptPoints[i].X >= CanvasWidth)
                        ptPoints[i].X = 0;
                    if (ptPoints[i].Y < 0)
                        ptPoints[i].Y = CanvasHeight - 1;
                    if (ptPoints[i].Y >= CanvasHeight)
                        ptPoints[i].Y = 0;
                }
                lifeLeft -= 1;
                return true;
            }
            else
                return false;
        }

        public override void Draw()
        {
            for (int i = 0; i < NUM_EXP_POINTS; i++)
            {
                Point ptDraw = new Point((int)(ptPoints[i].X / (double)CanvasWidth * Canvas.Size.Width),
                                         (int)(ptPoints[i].Y / (double)CanvasHeight * Canvas.Size.Height));

                Point ptDraw2 = new Point(ptDraw.X + 1, ptDraw.Y + 1);

                Canvas.AddLine(ptDraw, ptDraw2, GetRandomFireColor());
            }
        }
    }
}
