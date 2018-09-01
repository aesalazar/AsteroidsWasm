using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Asteroids.Standard.Helpers;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Base
{
    /// <summary>
    /// ScreenObject - defines an object to be displayed on screen
    /// This object is based on a cartesian coordinate system 
    /// centered at 0, 0
    /// </summary>
    abstract class ScreenObject : CommonOps
    {
        // points is used for the internal cartesian system
        protected IList<Point> points;
        public IList<Point> pointsTransformed; // exposed to simplify explosions
        protected Point currLoc;
        protected double velocityX;
        protected double velocityY;
        protected double radians;

        public Point GetCurrLoc() { return currLoc; }
        public double GetVelocityX() { return velocityX; }
        public double GetVelocityY() { return velocityY; }
        public double GetRadians() { return radians; }

        public ScreenObject(Point location)
        {
            radians = 180 * Math.PI / 180;
            points = new List<Point>
            {
                Capacity = 20
            };
            pointsTransformed = new List<Point>
            {
                Capacity = 20
            };
            velocityX = 0;
            velocityY = 0;
            currLoc = location;

            InitPoints();
        }

        public abstract void InitPoints();

        // Used to add points to a polygon
        public int AddPoint(Point pt)
        {
            points.Add(pt);
            pointsTransformed.Add(pt);
            return pointsTransformed.Count - 1;
        }

        protected void Rotate(double degrees)
        {
            double radiansAdjust = degrees * 0.0174532925;
            radians += radiansAdjust / FPS;
            double SinVal = Math.Sin(radians);
            double CosVal = Math.Cos(radians);

            pointsTransformed.Clear();
            var ptTransformed = new Point(0, 0);
            for (int i = 0; i < points.Count; i++)
            {
                var pt = points[i];
                ptTransformed.X = (int)(pt.X * CosVal + pt.Y * SinVal);
                ptTransformed.Y = (int)(pt.X * SinVal - pt.Y * CosVal);
                pointsTransformed.Add(ptTransformed);
            }
        }

        protected void DrawPolyToSC(IList<Point> alPoly, ScreenCanvas sc, int iPictX, int iPictY, string penColor)
        {
            var ptsPoly = new Point[alPoly.Count];
            for (int i = 0; i < alPoly.Count; i++)
            {
                ptsPoly[i].X = (int)((currLoc.X + alPoly[i].X) / (double)iMaxX * iPictX);
                ptsPoly[i].Y = (int)((currLoc.Y + alPoly[i].Y) / (double)iMaxY * iPictY);
            }
            sc.AddPolygon(ptsPoly, penColor);
        }

        public virtual bool Move()
        {
            currLoc.X += (int)velocityX;
            currLoc.Y += (int)velocityY;
            if (currLoc.X < 0)
                currLoc.X = iMaxX - 1;
            if (currLoc.X >= iMaxX)
                currLoc.X = 0;
            if (currLoc.Y < 0)
                currLoc.Y = iMaxY - 1;
            if (currLoc.Y >= iMaxY)
                currLoc.Y = 0;

            return true;
        }

        protected string GetRandomFireColor()
        {
            string penDraw;
            switch (rndGen.Next(3))
            {
                case 0:
                    penDraw = ColorHexStrings.RedHex;
                    break;
                case 1:
                    penDraw = ColorHexStrings.YellowHex;
                    break;
                case 2:
                    penDraw = ColorHexStrings.OrangeHex;
                    break;
                default:
                    penDraw = ColorHexStrings.WhiteHex;
                    break;
            }
            return penDraw;
        }

        public virtual void Draw(ScreenCanvas sc, int iPictX, int iPictY)
        {
            DrawPolyToSC(pointsTransformed, sc, iPictX, iPictY, ColorHexStrings.WhiteHex);
        }

        public virtual void Draw(ScreenCanvas sc, int iPictX, int iPictY, string penColor)
        {
            DrawPolyToSC(pointsTransformed, sc, iPictX, iPictY, penColor);
        }
    }
}
