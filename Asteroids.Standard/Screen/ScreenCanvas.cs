using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Asteroids.Standard.Components;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;

namespace Asteroids.Standard.Screen
{

    /// <summary>
    /// Drawing canvas to which all heights and widths will be scaled.
    /// </summary>
    /// <remarks>
    /// Angle 0 is pointing "down", 90 is "left" on the canvas
    /// </remarks>
    internal sealed class ScreenCanvas
    {
        private readonly object _updatePointsLock;
        private readonly object _updatePolysLock;
        private readonly IList<(Point[], DrawColor)> _points;
        private readonly IList<(Point[], DrawColor)> _polys;

        private Point _lastPoint;
        private DrawColor _lastPen;

        /// <summary>
        /// Creates a new instance of <see cref="ScreenCanvas"/>.
        /// </summary>
        /// <param name="size">Initial actual size.</param>
        public ScreenCanvas(Rectangle size)
        {
            Size = size;

            _updatePointsLock = new object();
            _updatePolysLock = new object();
            _points = new List<(Point[], DrawColor)>();
            _polys = new List<(Point[], DrawColor)>();

            //Set in case a call to add to end is made prior to creating a line
            _lastPoint = new Point(0, 0);
            _lastPen = DrawColor.White;
        }

        /// <summary>
        /// Current ACTUAL size of the canvas.
        /// </summary>
        public Rectangle Size { get; set; }

        /// <summary>
        /// Clears all stored points and polygons.
        /// </summary>
        public void Clear()
        {
            lock (_updatePointsLock)
                _points.Clear();

            lock (_updatePolysLock)
                _polys.Clear();
        }

        /// <summary>
        /// Draws all stored lines and points to the <see cref="IGraphicContainer"/>.
        /// </summary>
        public async Task Draw(IGraphicContainer container)
        {
            var pts = new List<(Point[], DrawColor)>();
            lock (_updatePointsLock)
                pts.AddRange(_points);

            var polys = new List<(Point[], DrawColor)>();
            lock (_updatePolysLock)
                polys.AddRange(_polys);

            //Send lines
            var glines = pts.Select(a => new GraphicLine
            {
                Color = a.Item2,
                Point1 = a.Item1[0],
                Point2 = a.Item1[1]
            }).ToList();

            //Send polygons
            var gpolys = polys
                .Select(a => new GraphicPolygon(a.Item2, a.Item1))
                .ToList();

            await container.Draw(glines, gpolys);
        }

        /// <summary>
        /// Adds a line between two points with a pen color without translation.
        /// </summary>
        public void AddLine(Point ptStart, Point ptEnd, DrawColor penColor)
        {
            _lastPoint = ptEnd;
            _lastPen = penColor;

            var pts = new [] { ptStart, ptEnd };
            lock (_updatePointsLock)
                _points.Add((pts, penColor));
        }

        /// <summary>
        /// Adds a line between two points with a white pen without translation.
        /// </summary>
        public void AddLine(Point ptStart, Point ptEnd)
        {
            AddLine(ptStart, ptEnd, DrawColor.White);
        }

        /// <summary>
        /// Adds a line from the last <see cref="Point"/> added from the last Line without
        /// translation.
        /// </summary>
        public void AddLineTo(Point ptEnd)
        {
            AddLine(_lastPoint, ptEnd, _lastPen);
        }

        /// <summary>
        /// Add a collection of <see cref="Point"/>s that make up a polygon to the interal collection
        /// without translation.
        /// </summary>
        /// <param name="polygonPoints">Collection of points to draw on the canvas.</param>
        /// <param name="penColor">Hex color to apply to the polygon.</param>
        public void AddPolygon(Point[] polygonPoints, DrawColor penColor)
        {
            lock (_updatePolysLock)
                _polys.Add((polygonPoints, penColor));
        }

        /// <summary>
        /// Translates to Canvas coordinates and adds a collection of points a polygon.
        /// </summary>
        /// <param name="polygonPoints">Collection of points to draw on the canvas.</param>
        /// <param name="penColor">Hex color to apply to the polygon.</param>
        public void LoadPolygon(IList<Point> polygonPoints, DrawColor penColor)
        {
            var ptsPoly = new Point[polygonPoints.Count];
            for (var i = 0; i < polygonPoints.Count; i++)
            {
                ptsPoly[i].X = (int)(polygonPoints[i].X / CanvasWidthDouble * Size.Width);
                ptsPoly[i].Y = (int)(polygonPoints[i].Y / CanvasHeightDouble * Size.Height);
            }

            AddPolygon(ptsPoly, penColor);
        }

        /// <summary>
        /// Translates to Canvas coordinates and adds a line vector.
        /// </summary>
        /// <param name="origin">Staring point for the line vector.</param>
        /// <param name="canvasOffsetX">Offset X to be added AFTER translation of the origin.</param>
        /// <param name="canvasOffsetY">Offset Y to be added AFTER translation of the origin.</param>
        /// <param name="penColor">Hex color to apply to the line vector.</param>
        public void LoadVector(Point origin, int canvasOffsetX, int canvasOffsetY, DrawColor penColor)
        {
            var ptDraw = new Point(
                (int)(origin.X / CanvasWidthDouble * Size.Width),
                (int)(origin.Y / CanvasHeightDouble * Size.Height)
            );

            var ptDraw2 = new Point(ptDraw.X + canvasOffsetX, ptDraw.Y + canvasOffsetY);
            AddLine(ptDraw, ptDraw2, penColor);
        }

        #region Statics

        /// <summary>
        /// Refresh rate.
        /// </summary>
        public const double FramesPerSecond = 60;

        /// <summary>
        /// Conversion from degrees to radians.
        /// </summary>
        public const double RadiansPerDegree = Math.PI / 180;

        /// <summary>
        /// Amount of radians in a full circle (i.e. 360 degrees)
        /// </summary>
        public const double RadiansPerCircle = Math.PI * 2;

        /// <summary>
        /// Horizontal width (effective) of the drawing plane.
        /// </summary>
        /// <remarks>
        /// All points and polygons will be drawn using this value and then 
        /// translated to the actual value set by <see cref="Size"/>.
        /// </remarks>
        public const int CanvasWidth = 10000;

        /// <summary>
        /// <see cref="CanvasWidth"/> as <see langword="double"/> to avoid casting.
        /// </summary>
        public const double CanvasWidthDouble = CanvasWidth;

        /// <summary>
        /// Vertical height (effective) of the drawing plane.
        /// </summary>
        /// <remarks>
        /// All points and polygons will be drawn using this value and then 
        /// translated to the actual value set by <see cref="Size"/>.
        /// </remarks>
        public const int CanvasHeight = 7500;

        /// <summary>
        /// <see cref="CanvasHeight"/> as <see langword="double"/> to avoid casting.
        /// </summary>
        public const double CanvasHeightDouble = CanvasHeight;

        /// <summary>
        /// Default explosion time factor relative to the <see cref="FramesPerSecond"/>.
        /// </summary>
        public const int DefaultExplosionLength = 1;

        #endregion
    }
}
