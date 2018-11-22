using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Asteroids.Standard.Components;
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
        /// <summary>
        /// Creates a new instance of <see cref="ScreenObject"/>.
        /// </summary>
        /// <param name="location">Absolute origin (bottom-left) of the object.</param>
        public ScreenObject(Point location) : base()
        {
            IsAlive = true;

            _updatePointsLock = new object();
            _updatePointsTransformedLock = new object();

            //templatrs are drawn nose "up"
            radians = 180 * ScreenCanvas.RADIANS_PER_DEGREE;

            _points = new List<Point>();
            PointsTransformed = new List<Point>();

            currLoc = location;

            InitPoints();
        }

        #region State

        /// <summary>
        /// Relative time length at which the object explodes.
        /// </summary>
        protected int ExplosionLength = ScreenCanvas.DEFAULT_EXPLOSION_LENGTH;

        /// <summary>
        /// Indicates if the object is alive.
        /// </summary>
        public bool IsAlive { get; protected set; }

        /// <summary>
        /// Blow up the object.
        /// </summary>
        /// <returns>Explosion collection.</param>
        public virtual IList<Explosion> Explode()
        {
            IsAlive = false;

            velocityX = 0;
            velocityY = 0;

            return GetPoints()
                .Select(p => new Explosion(p, ExplosionLength))
                .ToList();
        }

        #endregion

        #region Points for Drawing Object

        private readonly object _updatePointsLock;
        private readonly object _updatePointsTransformedLock;

        /// <summary>
        /// Points is used for the internal cartesian system.
        /// </summary>
        private IList<Point> _points;

        /// <summary>
        /// Points is used for the internal cartesian system with rotaton angle appiled.
        /// </summary>
        protected IList<Point> PointsTransformed; // exposed to simplify explosions

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
            lock (_updatePointsLock)
                _points.Add(point);

            lock (_updatePointsTransformedLock)
                PointsTransformed.Add(point);

            return PointsTransformed.Count - 1;
        }

        /// <summary>
        /// Add points to internal collection used to calculate drawn polygons.
        /// </summary>
        /// <param name="points"><see cref="Point"/> collection to add to internal collections.</param>
        /// <returns>Index of the last point inserted.</returns>
        public int AddPoints(IList<Point> points)
        {
            lock (_updatePointsLock)
                foreach (var point in points)
                    _points.Add(point);

            lock (_updatePointsTransformedLock)
                foreach (var point in points)
                    PointsTransformed.Add(point);

            return PointsTransformed.Count - 1;
        }

        /// <summary>
        /// Returns transformed <see cref="Point"/>s to generate object 
        /// polygon in a thead-safe manner relative to current location on the 
        /// <see cref="ScreenCanvas"/>.
        /// </summary>
        /// <returns>Collection of <see cref="Point"/>s.</returns>
        public IList<Point> GetPoints()
        {
            var points = new List<Point>();

            lock (_updatePointsTransformedLock)
                foreach (var pt in PointsTransformed)
                    points.Add(new Point(
                        pt.X + currLoc.X
                        , pt.Y + currLoc.Y
                    ));

            return points;
        }

        /// <summary>
        /// Clears all insternal and transformed <see cref="Point"/>s used to generate 
        /// polygons in a thead-safe manner.
        /// </summary>
        public void ClearPoints()
        {
            lock (_updatePointsLock)
                _points.Clear();

            lock (_updatePointsTransformedLock)
                PointsTransformed.Clear();
        }

        #endregion

        #region Rotation

        /// <summary>
        /// Get the current rotational radians.
        /// </summary>
        public double GetRadians() => radians;

        /// <summary>
        /// Current rotation.
        /// </summary>
        protected double radians;

        /// <summary>
        /// Rotates all internal <see cref="Point"/>s used to generate polygons on draw
        /// based on the alignment with the target point.
        /// </summary>
        /// <param name="alignPoint"><see cref="Point"/> to target.</param>
        protected void Align(Point alignPoint)
        {
            radians = GeometryHelper.GetAngle(currLoc, alignPoint);
            RotateInternal();
        }

        /// <summary>
        /// Rotates all internal <see cref="Point"/>s used to generate polygons on draw
        /// by a number of decimal degrees.
        /// </summary>
        /// <param name="degrees">Rotation amount in degrees.</param>
        protected void Rotate(double degrees)
        {
            //Get radians in 1/FPS'th increment
            var radiansAdjust = degrees * ScreenCanvas.RADIANS_PER_DEGREE;
            radians += radiansAdjust / ScreenCanvas.FPS;
            radians = radians % ScreenCanvas.RADIANS_PER_CIRCLE;

            RotateInternal();
        }

        /// <summary>
        /// Rotates all internal <see cref="Point"/>s used to generate polygons on draw
        /// based on decimal degrees.
        /// </summary>
        private void RotateInternal()
        {
            double SinVal = Math.Sin(radians);
            double CosVal = Math.Cos(radians);

            //Get points with some thread safety
            var newPointsTransformed = new List<Point>();

            var points = new List<Point>();
            lock (_updatePointsLock)
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
                PointsTransformed.Clear();
                foreach (var pt in newPointsTransformed)
                    PointsTransformed.Add(pt);
            }
        }

        #endregion

        #region Movement

        /// <summary>
        /// Get the current absolute origin (top-left) of the object.
        /// </summary>
        public Point GetCurrLoc() => currLoc;

        protected Point currLoc;

        /// <summary>
        /// Get the current velocity along the X axis.
        /// </summary>
        public double GetVelocityX() => velocityX;

        protected double velocityX;

        /// <summary>
        /// Get the current velocity along the Y axis.
        /// </summary>
        public double GetVelocityY() => velocityY;

        protected double velocityY;

        /// <summary>
        /// Move the object a single increment based on <see cref="GetVelocityX"/>
        /// and <see cref="GetVelocityY"/> and set current location.
        /// </summary>
        /// <returns>Indication of the move being completed successfully.</returns>
        public virtual bool Move()
        {
            currLoc.X += (int)velocityX;
            currLoc.Y += (int)velocityY;

            if (currLoc.X < 0)
                currLoc.X = ScreenCanvas.CANVAS_WIDTH - 1;
            if (currLoc.X >= ScreenCanvas.CANVAS_WIDTH)
                currLoc.X = 0;

            if (currLoc.Y < 0)
                currLoc.Y = ScreenCanvas.CANVAS_HEIGHT - 1;
            if (currLoc.Y >= ScreenCanvas.CANVAS_HEIGHT)
                currLoc.Y = 0;

            return true;
        }

        #endregion
        
    }
}
