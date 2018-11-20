using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Asteroids.Standard.Base;
using Asteroids.Standard.Components;
using Asteroids.Standard.Helpers;

namespace Asteroids.Standard.Screen
{
    class DrawingManager : CommonOps
    {
        private readonly ScreenObjectCache _cache;
        private readonly ScreenCanvas _canvas;

        /// <summary>
        /// Manages drawing to a canvas.
        /// </summary>
        public DrawingManager(ScreenObjectCache cache, ScreenCanvas canvas)
        {
            _cache = cache;
            _canvas = canvas;
        }

        #region Drawing Primatives

        private void DrawPolygon(IList<Point> points)
        {
            DrawPolygon(points, ColorHexStrings.WhiteHex);
        }

        private void DrawPolygon(IList<Point> points, string colorHex)
        {
            _canvas.LoadPolygon(points, colorHex);
        }

        private void DrawVector(Point origin, int offsetX, int offsetY, string colorHex)
        {
            _canvas.LoadVector(origin, offsetX, offsetY, colorHex);
        }

        #endregion

        #region Draw Objects

        /// <summary>
        /// Draw all objects to the canvas.
        /// </summary>
        public void DrawObjects()
        {
            DrawShip();
            DrawSaucer();
            DrawBullets();
            DrawBelt();
            DrawExplosions();
        }

        /// <summary>
        /// Draw Ship.
        /// </summary>
        private void DrawShip()
        {
            if (!_cache.Ship.IsAlive)
                return;

            DrawPolygon(_cache.ShipPoints);

            //Draw flame if thrust is on
            if (_cache.Ship.IsThrustOn)
            {
                // We have points transformed so we know where the bottom of the ship is
                var thrustPoints = new List<Point>
                {
                    Capacity = 3
                };

                var pts = _cache.ShipPoints;
                var pt1 = pts[Ship.PointThrust1];
                var pt2 = pts[Ship.PointThrust2];

                thrustPoints.Add(pt1);
                thrustPoints.Add(pt2);

                // random thrust effect
                int size = Random.Next(200) + 100;
                var radians = _cache.Ship.GetRadians();

                thrustPoints.Add(new Point(
                    (pt1.X + pt2.X) / 2 + (int)(size * Math.Sin(radians)),
                    (pt1.Y + pt2.Y) / 2 + (int)(-size * Math.Cos(radians))
                ));

                // Draw thrust directly to ScreenCanvas; it's not part of the object
                DrawPolygon(thrustPoints, GetRandomFireColor());
            }
        }

        /// <summary>
        /// Draw the Flying Saucer and Missile.
        /// </summary>
        private void DrawSaucer()
        {
            if (_cache.Saucer?.IsAlive != true)
                return;

            //Draw the saucer
            DrawPolygon(_cache.SaucerPoints);

            //Draw its missile
            if (_cache.MissilePoints?.Any() != true)
                return;

            DrawPolygon(_cache.MissilePoints);

            //Draw flame for the missile
            var thrustPoints = new List<Point>
            {
                Capacity = 3
            };

            var pts = _cache.MissilePoints;
            var pt1 = pts[Missile.PointThrust1];
            var pt2 = pts[Missile.PointThrust2];

            thrustPoints.Add(pt1);
            thrustPoints.Add(pt2);

            // random thrust effect
            int size = Random.Next(200) + 100;
            var radians = _cache.Saucer.Missile.GetRadians();

            thrustPoints.Add(new Point(
                (pt1.X + pt2.X) / 2 + (int)(size * Math.Sin(radians)),
                (pt1.Y + pt2.Y) / 2 + (int)(-size * Math.Cos(radians))
            ));

            // Draw thrust directly to ScreenCanvas; it's not part of the object
            DrawPolygon(thrustPoints, GetRandomFireColor());
        }

        /// <summary>
        /// Draw all available Bullets.
        /// </summary>
        private void DrawBullets()
        {
            foreach (var bullet in _cache.BulletsInFlight)
                DrawPolygon(bullet.PolygonPoints, GetRandomFireColor());
        }

        /// <summary>
        /// Draw all asteroids in the belt.
        /// </summary>
        private void DrawBelt()
        {
            var asteroids = _cache
                 .Asteroids
                 .Where(a => a.ScreenObject.Size != Asteroid.ASTEROID_SIZE.DNE);

            foreach (var asteroid in asteroids)
                DrawPolygon(asteroid.PolygonPoints);
        }

        /// <summary>
        /// Draw each explosion's progress.
        /// </summary>
        private void DrawExplosions()
        {
            var explosions = _cache.GetExplosions();

            foreach (var explosion in explosions)
                DrawExplosion(explosion);
        }

        /// <summary>
        /// Draws an explosion to the canvas.
        /// </summary>
        private void DrawExplosion(Explosion explosion)
        {
            foreach (var point in explosion.Points)
                DrawVector(new Point(point.X, point.Y), 1, 1, GetRandomFireColor());
        }

        #endregion

    }
}
