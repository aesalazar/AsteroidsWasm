using System;
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
        private IList<Point> _points;
        protected IList<Point> _pointsTransformed; // exposed to simplify explosions

        private readonly object updatePointsLock;
        protected readonly object _updatePointsTransformedLock;

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
            updatePointsLock = new object();
            _updatePointsTransformedLock = new object();

            radians = 180 * Math.PI / 180;

            _points = new List<Point>
            {
                Capacity = 20
            };

            _pointsTransformed = new List<Point>
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
            lock (updatePointsLock)
                _points.Add(pt);

            lock (_updatePointsTransformedLock)
                _pointsTransformed.Add(pt);

            return _pointsTransformed.Count - 1;
        }

        public IList<Point> GetPoints()
        {
            var points = new List<Point>();
            lock (_updatePointsTransformedLock)
                points.AddRange(_pointsTransformed);

            return points;
        }

        public void ClearPoints()
        {
            lock (updatePointsLock)
                _points.Clear();

            lock (_updatePointsTransformedLock)
                _pointsTransformed.Clear();
        }

        protected void Rotate(double degrees)
        {
            double radiansAdjust = degrees * 0.0174532925;
            radians += radiansAdjust / FPS;

            double SinVal = Math.Sin(radians);
            double CosVal = Math.Cos(radians);

            //Get points with some thread safety
            var newPointsTransformed = new List<Point>();

            var points = new List<Point>();
            lock (updatePointsLock)
                points.AddRange(_points);

            //Retransform the points
            var ptTransformed = new Point(0, 0);
            for (int i = 0; i < _points.Count; i++)
            {
                var pt = _points[i];
                ptTransformed.X = (int)(pt.X * CosVal + pt.Y * SinVal);
                ptTransformed.Y = (int)(pt.X * SinVal - pt.Y * CosVal);
                newPointsTransformed.Add(ptTransformed);
            }

            //Add the points
            lock (_updatePointsTransformedLock)
            {
                _pointsTransformed.Clear();
                foreach (var pt in newPointsTransformed)
                    _pointsTransformed.Add(pt);
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
            DrawPolyToSC(_pointsTransformed, sc, iPictX, iPictY, ColorHexStrings.WhiteHex);
        }

        public virtual void Draw(ScreenCanvas sc, int iPictX, int iPictY, string penColor)
        {
            DrawPolyToSC(_pointsTransformed, sc, iPictX, iPictY, penColor);
        }
    }
}
