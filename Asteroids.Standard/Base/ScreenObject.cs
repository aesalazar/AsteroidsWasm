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

        /// <summary>
        /// Creates a new instance of <see cref="ScreenObject"/>.
        /// </summary>
        /// <param name="location">Absolute origin (bottom-left) of the object.</param>
        /// <param name="canvas">Canvas to draw on.</param>
        public ScreenObject(Point location, ScreenCanvas canvas) : base(canvas)
        {
            updatePointsLock = new object();
            _updatePointsTransformedLock = new object();

            radians = 180 * Math.PI / 180;

            _points = new List<Point>();
            _pointsTransformed = new List<Point>();

            velocityX = 0;
            velocityY = 0;
            currLoc = location;

            InitPoints();
        }

        /// <summary>
        /// Get the current absolute origin (top-left) of the object.
        /// </summary>
        public Point GetCurrLoc() => currLoc;

        /// <summary>
        /// Get the current velocity along the X axis.
        /// </summary>
        public double GetVelocityX() => velocityX;

        /// <summary>
        /// Get the current velocity along the Y axis.
        /// </summary>
        public double GetVelocityY() => velocityY;

        /// <summary>
        /// Get the current rotational radians.
        /// </summary>
        public double GetRadians() => radians;

        /// <summary>
        /// Generates intial <see cref="Point"/>s needed to render the
        /// when drawing on the screen.
        /// </summary>
        protected abstract void InitPoints();

        /// <summary>
        /// Add point to internal collection used to calculate drawn polygons.
        /// </summary>
        /// <param name="point"><see cref="Point"/> to add to internal collections.</param>
        /// <returns>Index the point was inserted at.</returns>
        public int AddPoint(Point point)
        {
            lock (updatePointsLock)
                _points.Add(point);

            lock (_updatePointsTransformedLock)
                _pointsTransformed.Add(point);

            return _pointsTransformed.Count - 1;
        }

        /// <summary>
        /// Add points to internal collection used to calculate drawn polygons.
        /// </summary>
        /// <param name="points"><see cref="Point"/> collection to add to internal collections.</param>
        /// <returns>Index of the last point inserted.</returns>
        public int AddPoints(IList<Point> points)
        {
            lock (updatePointsLock)
                foreach(var point in points)
                    _points.Add(point);

            lock (_updatePointsTransformedLock)
                foreach(var point in points)
                    _pointsTransformed.Add(point);

            return _pointsTransformed.Count - 1;
        }

        /// <summary>
        /// Returns transformed <see cref="Point"/>s to generate 
        /// polygons in a thead-safe manner.
        /// </summary>
        /// <returns>Collection of <see cref="Point"/>s.</returns>
        public IList<Point> GetPoints()
        {
            var points = new List<Point>();
            lock (_updatePointsTransformedLock)
                points.AddRange(_pointsTransformed);

            return points;
        }

        /// <summary>
        /// Clears all insternal and transformed <see cref="Point"/>s used to generate 
        /// polygons in a thead-safe manner.
        /// </summary>
        public void ClearPoints()
        {
            lock (updatePointsLock)
                _points.Clear();

            lock (_updatePointsTransformedLock)
                _pointsTransformed.Clear();
        }

        /// <summary>
        /// Rotates all internal <see cref="Point"/>s used to generate polygons on draw.
        /// </summary>
        /// <param name="degrees">Rotation amount in degrees.</param>
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

        /// <summary>
        /// Draw a collection of polygons (unpersisted) to a <see cref="ScreenCanvas"/>.
        /// </summary>
        /// <param name="alPoly">Collection of points to draw on the canvas.</param>
        /// <param name="penColor">Hex color to apply to the polygon.</param>
        protected void DrawPolygons(IList<Point> alPoly, string penColor)
        {
            var ptsPoly = new Point[alPoly.Count];
            for (int i = 0; i < alPoly.Count; i++)
            {
                ptsPoly[i].X = (int)((currLoc.X + alPoly[i].X) / (double)CanvasWidth * Canvas.Size.Width);
                ptsPoly[i].Y = (int)((currLoc.Y + alPoly[i].Y) / (double)CanvasHeight * Canvas.Size.Height);
            }

            Canvas.AddPolygon(ptsPoly, penColor);
        }

        /// <summary>
        /// Move the object a single increment based on <see cref="GetVelocityX"/>
        /// and <see cref="GetVelocityY"/>.
        /// </summary>
        /// <returns>Indication of the move being completed successfully.</returns>
        public virtual bool Move()
        {
            currLoc.X += (int)velocityX;
            currLoc.Y += (int)velocityY;

            if (currLoc.X < 0)
                currLoc.X = CanvasWidth - 1;
            if (currLoc.X >= CanvasWidth)
                currLoc.X = 0;

            if (currLoc.Y < 0)
                currLoc.Y = CanvasHeight - 1;
            if (currLoc.Y >= CanvasHeight)
                currLoc.Y = 0;

            return true;
        }

        /// <summary>
        /// Generates a ranom color for any fire or explosion.
        /// </summary>
        /// <returns>Color hex string.</returns>
        protected string GetRandomFireColor()
        {
            string penDraw;

            switch (Random.Next(3))
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

        /// <summary>
        /// Draws the current internal collection of <see cref="Point"/>s to a <see cref="ScreenCanvas"/> 
        /// in the default <see cref="ColorHexStrings.WhiteHex"/>.
        /// </summary>
        public virtual void Draw()
        {
            Draw(ColorHexStrings.WhiteHex);
        }

        /// <summary>
        /// Draws the current internal collection of <see cref="Point"/>s to a <see cref="ScreenCanvas"/>.
        /// </summary>
        /// <param name="penColor">Hex color to apply to the polygon.</param>
        public virtual void Draw(string penColor)
        {
            DrawPolygons(GetPoints(), penColor);
        }
    }
}
