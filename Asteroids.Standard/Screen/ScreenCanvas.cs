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
    public class ScreenCanvas
    {
        private readonly object _updatePointsLock;
        private readonly object _updatePolysLock;
        private readonly IList<Tuple<Point[], string>> _points;
        private readonly IList<Tuple<Point[], string>> _polys;

        private Point _lastPoint;
        private string _lastPen;

        public ScreenCanvas()
        {
            _updatePointsLock = new object();
            _updatePolysLock = new object();
            _points = new List<Tuple<Point[], string>>();
            _polys = new List<Tuple<Point[], string>>();

            //Set in case a call to add to end is made prior to creating a line
            _lastPoint = new Point(0, 0);
            _lastPen = ColorHexStrings.TransparentHex;
        }

        public void Clear()
        {
            lock (_updatePointsLock)
                _points.Clear();

            lock (_updatePolysLock)
                _polys.Clear();
        }

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

        public void AddLine(Point ptStart, Point ptEnd, string penColor)
        {
            _lastPoint = ptEnd;
            _lastPen = penColor;

            var pts = new Point[] { ptStart, ptEnd };
            lock (_updatePointsLock)
                _points.Add(new Tuple<Point[], string>(pts, penColor));
        }

        public void AddLine(Point ptStart, Point ptEnd)
        {
            AddLine(ptStart, ptEnd, ColorHexStrings.WhiteHex);
        }

        public void AddLineTo(Point ptEnd)
        {
            AddLine(_lastPoint, ptEnd, _lastPen);
        }

        public void AddPolygon(Point[] ptArray, string penColor)
        {
            lock (_updatePolysLock)
                _polys.Add(new Tuple<Point[], string>(ptArray, penColor));
        }
    }
}
