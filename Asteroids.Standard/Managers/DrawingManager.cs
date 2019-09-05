using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Asteroids.Standard.Base;
using Asteroids.Standard.Components;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Helpers;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Managers
{
    /// <summary>
    /// Manages and optimizes the drawing of the state of <see cref="ScreenObject"/>s 
    /// stored in a <see cref="CacheManager"/> to a <see cref="ScreenCanvas"/>.
    /// </summary>
    internal class DrawingManager
    {
        private readonly CacheManager _cache;
        private readonly ScreenCanvas _canvas;

        /// <summary>
        /// Creates a new instance of <see cref="DrawingManager"/>
        /// </summary>
        /// <param name="cache">Screen object cache to draw.</param>
        /// <param name="canvas">Canvas to draw cache to.</param>
        public DrawingManager(CacheManager cache, ScreenCanvas canvas)
        {
            _cache = cache;
            _canvas = canvas;
        }

        #region Drawing Primatives

        private void DrawPolygon(IList<Point> points, DrawColor color = DrawColor.White)
        {
            _canvas.LoadPolygon(points, color);
        }

        private void DrawVector(Point origin, int offsetX, int offsetY, DrawColor color)
        {
            _canvas.LoadVector(origin, offsetX, offsetY, color);
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

                var pt1 = _cache.ShipPoints[Ship.PointThrust1];
                var pt2 = _cache.ShipPoints[Ship.PointThrust2];

                thrustPoints.Add(pt1);
                thrustPoints.Add(pt2);

                // random thrust effect
                int size = RandomizeHelper.Random.Next(200) + 100;
                var radians = _cache.Ship.GetRadians();

                thrustPoints.Add(new Point(
                    (pt1.X + pt2.X) / 2 + (int)(size * Math.Sin(radians)),
                    (pt1.Y + pt2.Y) / 2 + (int)(-size * Math.Cos(radians))
                ));

                // Draw thrust directly to ScreenCanvas; it's not part of the object
                DrawPolygon(thrustPoints, RandomizeHelper.GetRandomFireColor());
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

            var pt1 = _cache.MissilePoints[Missile.PointThrust1];
            var pt2 = _cache.MissilePoints[Missile.PointThrust2];

            thrustPoints.Add(pt1);
            thrustPoints.Add(pt2);

            // random thrust effect
            var size = RandomizeHelper.Random.Next(50) + 50;
            var radians = _cache.Saucer.Missile.GetRadians();

            thrustPoints.Add(new Point(
                (pt1.X + pt2.X) / 2 + (int)(size * Math.Sin(radians)),
                (pt1.Y + pt2.Y) / 2 + (int)(-size * Math.Cos(radians))
            ));

            // Draw thrust directly to ScreenCanvas; it's not part of the object
            DrawPolygon(thrustPoints, RandomizeHelper.GetRandomFireColor());
        }

        /// <summary>
        /// Draw all available Bullets.
        /// </summary>
        private void DrawBullets()
        {
            foreach (var bullet in _cache.GetBulletsInFlight())
                DrawPolygon(bullet.PolygonPoints, RandomizeHelper.GetRandomFireColor());
        }

        /// <summary>
        /// Draw all asteroids in the belt.
        /// </summary>
        private void DrawBelt()
        {
            var asteroids = _cache
                 .Asteroids
                 .Where(a => a.ScreenObject.Size != Asteroid.AsteroidSize.Dne);

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
                DrawVector(new Point(point.X, point.Y), 1, 1, RandomizeHelper.GetRandomFireColor());
        }

        #endregion

    }
}
