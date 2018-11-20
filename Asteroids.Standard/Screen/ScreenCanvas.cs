using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Asteroids.Standard.Components;
using Asteroids.Standard.Helpers;
using Asteroids.Standard.Interfaces;

namespace Asteroids.Standard.Screen
{

    /// <summary>
    /// Drawing canvas to which all heights and widths will be scaled.
    /// </summary>
    /// <remarks>
    /// Angle 0 is pointing "down", 90 is "left" on the canvas
    /// </remarks>
    public class ScreenCanvas
    {
        private readonly object _updatePointsLock;
        private readonly object _updatePolysLock;
        private readonly IList<Tuple<Point[], string>> _points;
        private readonly IList<Tuple<Point[], string>> _polys;

        private Point _lastPoint;
        private string _lastPen;

        /// <summary>
        /// Creates a new instance of <see cref="ScreenCanvas"/>.
        /// </summary>
        /// <param name="size">Initial actual size.</param>
        public ScreenCanvas(Rectangle size)
        {
            Size = size;

            _updatePointsLock = new object();
            _updatePolysLock = new object();
            _points = new List<Tuple<Point[], string>>();
            _polys = new List<Tuple<Point[], string>>();

            //Set in case a call to add to end is made prior to creating a line
            _lastPoint = new Point(0, 0);
            _lastPen = ColorHexStrings.TransparentHex;
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
            var pts = new List<Tuple<Point[], string>>();
            lock (_updatePointsLock)
                pts.AddRange(_points);

            var polys = new List<Tuple<Point[], string>>();
            lock (_updatePolysLock)
                polys.AddRange(_polys);

            //Send lines
            var glines = pts.Select(tuple => new GraphicLine
            {
                ColorHex = tuple.Item2,
                Point1 = tuple.Item1[0],
                Point2 = tuple.Item1[1]
            }).ToList();

            //Send polygons
            var gpolys = polys.Select(tuple => new GraphicPolygon
            {
                ColorHex = tuple.Item2,
                Points = tuple.Item1
            }).ToList();

            await container.Draw(glines, gpolys);
        }

        /// <summary>
        /// Adds a line between two points with a pen color without translation.
        /// </summary>
        public void AddLine(Point ptStart, Point ptEnd, string penColor)
        {
            _lastPoint = ptEnd;
            _lastPen = penColor;

            var pts = new Point[] { ptStart, ptEnd };
            lock (_updatePointsLock)
                _points.Add(new Tuple<Point[], string>(pts, penColor));
        }

        /// <summary>
        /// Adds a line between two points with a white pen without translation.
        /// </summary>
        public void AddLine(Point ptStart, Point ptEnd)
        {
            AddLine(ptStart, ptEnd, ColorHexStrings.WhiteHex);
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
        public void AddPolygon(Point[] polygonPoints, string penColor)
        {
            lock (_updatePolysLock)
                _polys.Add(new Tuple<Point[], string>(polygonPoints, penColor));
        }

        /// <summary>
        /// Translates to Canvas coordinates and adds a collection of points a polygon.
        /// </summary>
        /// <param name="polygonPoints">Collection of points to draw on the canvas.</param>
        /// <param name="penColor">Hex color to apply to the polygon.</param>
        public void LoadPolygon(IList<Point> polygonPoints, string penColor)
        {
            var ptsPoly = new Point[polygonPoints.Count];
            for (int i = 0; i < polygonPoints.Count; i++)
            {
                ptsPoly[i].X = (int)(polygonPoints[i].X / (double)CANVAS_WIDTH * Size.Width);
                ptsPoly[i].Y = (int)(polygonPoints[i].Y / (double)CANVAS_HEIGHT * Size.Height);
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
        public void LoadVector(Point origin, int canvasOffsetX, int canvasOffsetY, string penColor)
        {
            var ptDraw = new Point(
                (int)(origin.X / (double)CANVAS_WIDTH * Size.Width),
                (int)(origin.Y / (double)CANVAS_HEIGHT * Size.Height)
            );

            var ptDraw2 = new Point(ptDraw.X + canvasOffsetX, ptDraw.Y + canvasOffsetY);
            AddLine(ptDraw, ptDraw2, penColor);
        }

        #region Statics

        /// <summary>
        /// Refresh rate.
        /// </summary>
        public const double FPS = 60;

        /// <summary>
        /// Conversion from degrees to radians.
        /// </summary>
        public const double RADIANS_PER_DEGREE = Math.PI / 180;

        /// <summary>
        /// Amount of radians in a full circle (i.e. 360 degrees)
        /// </summary>
        public const double RADIANS_PER_CIRCLE = Math.PI * 2;

        /// <summary>
        /// Horizontal width (effective) of the drawing plane.
        /// </summary>
        /// <remarks>
        /// All points and polygons will be drawn using this value and then 
        /// translated to the actual value set by <see cref="Size"/>.
        /// </remarks>
        public const int CANVAS_WIDTH = 10000;

        /// <summary>
        /// Vertical heigth (effective) of the drawing plane.
        /// </summary>
        /// <remarks>
        /// All points and polygons will be drawn using this value and then 
        /// translated to the actual value set by <see cref="Size"/>.
        /// </remarks>
        public const int CANVAS_HEIGHT = 7500;

        #endregion
    }
}
