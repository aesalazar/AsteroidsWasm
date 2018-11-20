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
        public const int DEFAULT_LENGTH = 1;
        private const int NUM_EXP_POINTS = 22; // more points is more dazzling
        private const int EXPLOSION_LIFE = (int)ScreenCanvas.FPS / 2; // default life of explosion is 1/2 sec

        public Point[] Points { get; }
        public Point[] Velocities { get; }

        private int _framesRemaining;

        public Explosion(Point ptExplosion, double timeFactor = DEFAULT_LENGTH) // : base(ptExplosion)
        {
            _framesRemaining = (int)(EXPLOSION_LIFE * timeFactor);
            Points = new Point[NUM_EXP_POINTS];
            Velocities = new Point[NUM_EXP_POINTS];

            for (int i = 0; i < NUM_EXP_POINTS; i++)
            {
                Points[i] = ptExplosion;
                Velocities[i] = new Point(
                    (int)((Random.Next(1200) - 600) / ScreenCanvas.FPS)
                    , (int)((Random.Next(1200) - 600) / ScreenCanvas.FPS)
                );
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
            if (_framesRemaining > 0)
            {
                for (int i = 0; i < NUM_EXP_POINTS; i++)
                {
                    Points[i].X += Velocities[i].X;
                    Points[i].Y += Velocities[i].Y;

                    if (Points[i].X < 0)
                        Points[i].X = ScreenCanvas.CANVAS_WIDTH - 1;
                    if (Points[i].X >= ScreenCanvas.CANVAS_WIDTH)
                        Points[i].X = 0;
                    if (Points[i].Y < 0)
                        Points[i].Y = ScreenCanvas.CANVAS_HEIGHT - 1;
                    if (Points[i].Y >= ScreenCanvas.CANVAS_HEIGHT)
                        Points[i].Y = 0;
                }
                _framesRemaining -= 1;
                return true;
            }
            else
                return false;
        }

        //public void Draw(ScreenCanvas canvas)
        //{
        //    for (int i = 0; i < NUM_EXP_POINTS; i++)
        //    {
        //        Point ptDraw = new Point(
        //            (int)(Points[i].X / (double)ScreenCanvas.CANVAS_WIDTH * canvas.Size.Width),
        //            (int)(Points[i].Y / (double)ScreenCanvas.CANVAS_HEIGHT * canvas.Size.Height)
        //        );

        //        Point ptDraw2 = new Point(ptDraw.X + 1, ptDraw.Y + 1);

        //        canvas.AddLine(ptDraw, ptDraw2, GetRandomFireColor());
        //    }
        //}
    }
}
