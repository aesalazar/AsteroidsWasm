using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Generates a ranom color for any fire or explosion.
        /// </summary>
        /// <returns>Color hex string.</returns>
        private string GetRandomFireColor()
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

        private void DrawPolygon(IList<Point> points)
        {
            DrawPolygon(points, ColorHexStrings.WhiteHex);
        }

        private void DrawPolygon(IList<Point> points, string colorHex)
        {
            _canvas.LoadPolygon(points, colorHex);
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
            if (_cache.Saucer == null || !_cache.Saucer.IsAlive == false)
                return;

            //Draw the saucer
            DrawPolygon(_cache.SaucerPoints);

            //Draw its missile
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
        /// Draw all avilable Bullets.
        /// </summary>
        private void DrawBullets()
        {
            //Only avilable bullets are in the cache
            foreach (var bullet in _cache.Bullets)
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
            foreach (var explosion in _cache.Explosions)
                explosion.Draw(_canvas, GetRandomFireColor());
           
        }

        #endregion

    }
}
