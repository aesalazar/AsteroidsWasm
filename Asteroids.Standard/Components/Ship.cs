using System;
using System.Collections.Generic;
using System.Drawing;
using Asteroids.Standard.Base;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Screen;
using static Asteroids.Standard.Sounds.ActionSounds;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Summary description for CShip.
    /// </summary>
    class Ship : ScreenObject
    {
        enum SHIP_STATE { WAITING, ALIVE, EXPLODING, DONE };
        SHIP_STATE state;
        const double ROTATE_SPEED = 12000 / FPS;
        bool bThrustOn;

        public Ship(ScreenCanvas canvas) : base(new Point(iMaxX / 2, iMaxY / 2), canvas)
        {
            bThrustOn = false;
            state = SHIP_STATE.WAITING;
        }

        public Ship(bool bAlive, ScreenCanvas canvas) : this(canvas)
        {
            state = SHIP_STATE.ALIVE;
        }

        protected override void InitPoints()
        {
            ClearPoints();
            AddPoints(PointsTemplate);
        }

        public bool Hyperspace()
        {
            bool bSafeHyperspace = rndGen.Next(10) != 1;
            currLoc.X = rndGen.Next((int)(iMaxX * .8)) + (int)(iMaxX * .1);
            currLoc.Y = rndGen.Next((int)(iMaxY * .8)) + (int)(iMaxY * .1);
            return bSafeHyperspace;
        }

        public void Explode()
        {
            state = SHIP_STATE.EXPLODING;
            velocityX = velocityY = 0;
        }
        public bool IsAlive()
        {
            return state == SHIP_STATE.ALIVE;
        }

        public void DecayThrust()
        {
            bThrustOn = false;

            velocityX = velocityX * (1 - 1 / FPS);
            velocityY = velocityY * (1 - 1 / FPS);
        }

        public void Thrust()
        {
            bThrustOn = true;

            double SinVal = Math.Sin(radians);
            double CosVal = Math.Cos(radians);
            double addThrust = 90 / FPS;
            double maxThrustSpeed = 5000 / FPS;
            double incX, incY;

            incX = -(addThrust * SinVal);
            incY = addThrust * CosVal;

            velocityX += incX;
            if (velocityX > maxThrustSpeed)
                velocityX = maxThrustSpeed;
            if (velocityX < -maxThrustSpeed)
                velocityX = -maxThrustSpeed;
            velocityY += incY;
            if (velocityY > maxThrustSpeed)
                velocityY = maxThrustSpeed;
            if (velocityY < -maxThrustSpeed)
                velocityY = -maxThrustSpeed;

            PlaySound(this, ActionSound.Thrust);
        }

        public void RotateLeft()
        {
            Rotate(-ROTATE_SPEED);
        }
        public void RotateRight()
        {
            Rotate(ROTATE_SPEED);
        }

        public override void Draw()
        {
            switch (state)
            {
                case SHIP_STATE.ALIVE:
                    base.Draw();
                    if (bThrustOn)
                    {
                        // We have points transformed so we know where the bottom of the ship is
                        var alPoly = new List<Point>
                        {
                            Capacity = 3
                        };

                        var pts = GetPoints();

                        alPoly.Add(pts[PointThrust1]);
                        alPoly.Add(pts[PointThrust2]);

                        int iThrustSize = rndGen.Next(200) + 100; // random thrust effect

                        alPoly.Add(new Point(
                            (pts[PointThrust1].X + pts[PointThrust2].X) / 2 + (int)(iThrustSize * Math.Sin(radians)),
                            (pts[PointThrust1].Y + pts[PointThrust2].Y) / 2 + (int)(-iThrustSize * Math.Cos(radians))
                        ));
                        
                        // Draw thrust directly to ScreenCanvas; it's not really part of the ship object
                        DrawPolygons(alPoly, GetRandomFireColor());
                    }
                    break;
            }
        }

        #region Statics

        /// <summary>
        /// Non-transformed point template for creating a new ship.
        /// </summary>
        private static IList<Point> PointsTemplate = new List<Point>();

        /// <summary>
        /// Index location in <see cref="PointsTemplate"/> for thrust point 1.
        /// </summary>
        private static int PointThrust1;

        /// <summary>
        /// Index location in <see cref="PointsTemplate"/> for thrust point 2.
        /// </summary>
        private static int PointThrust2;

        /// <summary>
        /// Setup update the <see cref="PointsTemplate"/>.
        /// </summary>
        static Ship()
        {
            PointsTemplate.Clear();

            const int shipWidthHalf = 100;
            const int shipHeightHalf = shipWidthHalf * 2;
            const int shipHeightInUp = (int)(shipHeightHalf * .6);
            const int shipWidthInSide = (int)(shipWidthHalf * .3);

            PointsTemplate.Add(new Point(0, -shipHeightHalf));
            PointsTemplate.Add(new Point(shipWidthHalf / 2, 0)); // midpoint for collisions
            PointsTemplate.Add(new Point(shipWidthHalf, shipHeightHalf));
            PointsTemplate.Add(new Point(shipWidthInSide, shipHeightInUp));
            PointsTemplate.Add(new Point(-shipWidthInSide, shipHeightInUp));
            PointsTemplate.Add(new Point(-shipWidthHalf, shipHeightHalf));
            PointsTemplate.Add(new Point(-shipWidthHalf / 2, 0)); // midpoint for collisions

            PointThrust1 = 3;
            PointThrust2 = 4;
        }

        #endregion
    }
}
