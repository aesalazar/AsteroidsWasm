using System.Drawing;
using Asteroids.Standard.Base;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Summary description for Explosion.
    /// </summary>
    class Explosion  : CommonOps //: ScreenObject
    {
        public const int DEFAULT_LENGTH = 2;
        const int NUM_EXP_POINTS = 22; // more points is more dazzling
        const int EXPLOSION_LIFE = (int)ScreenCanvas.FPS / 2; // default life of explosion is 1/2 sec
        int lifeLeft;
        Point[] ptPoints;
        Point[] ptPointsVelocity;

        public Explosion(Point ptExplosion, double timeFactor = DEFAULT_LENGTH) // : base(ptExplosion)
        {
            lifeLeft = (int)(EXPLOSION_LIFE * timeFactor);
            ptPoints = new Point[NUM_EXP_POINTS];
            ptPointsVelocity = new Point[NUM_EXP_POINTS];
            for (int i = 0; i < NUM_EXP_POINTS; i++)
            {
                ptPoints[i] = ptExplosion;
                ptPointsVelocity[i] = new Point((int)((Random.Next(1200) - 600) / ScreenCanvas.FPS),
                                                (int)((Random.Next(1200) - 600) / ScreenCanvas.FPS));
            }
        }

        //protected override void InitPoints()
        //{
        //    // Explosions are self-drawn
        //    // so do not initialize points in the base class
        //}

        //public override bool Move()
        public bool Move()
        {
            if (lifeLeft > 0)
            {
                for (int i = 0; i < NUM_EXP_POINTS; i++)
                {
                    ptPoints[i].X += ptPointsVelocity[i].X;
                    ptPoints[i].Y += ptPointsVelocity[i].Y;

                    if (ptPoints[i].X < 0)
                        ptPoints[i].X = ScreenCanvas.CANVAS_WIDTH - 1;
                    if (ptPoints[i].X >= ScreenCanvas.CANVAS_WIDTH)
                        ptPoints[i].X = 0;
                    if (ptPoints[i].Y < 0)
                        ptPoints[i].Y = ScreenCanvas.CANVAS_HEIGHT - 1;
                    if (ptPoints[i].Y >= ScreenCanvas.CANVAS_HEIGHT)
                        ptPoints[i].Y = 0;
                }
                lifeLeft -= 1;
                return true;
            }
            else
                return false;
        }

        public void Draw(ScreenCanvas canvas, string colorHex)
        {
            for (int i = 0; i < NUM_EXP_POINTS; i++)
            {
                Point ptDraw = new Point(
                    (int)(ptPoints[i].X / (double)ScreenCanvas.CANVAS_WIDTH * canvas.Size.Width),
                    (int)(ptPoints[i].Y / (double)ScreenCanvas.CANVAS_HEIGHT * canvas.Size.Height)
                    );

                Point ptDraw2 = new Point(ptDraw.X + 1, ptDraw.Y + 1);

                canvas.AddLine(ptDraw, ptDraw2, colorHex);
            }
        }
    }
}
