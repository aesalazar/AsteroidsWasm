﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;

namespace Asteroids.Wpf.Core.Classes
{
    /// <summary>
    /// Control to paint vectors based on <see cref="WriteableBitmap"/>.
    /// </summary>
    public class GraphicContainer : Image, IGraphicContainer, IDisposable
    {
        private readonly Dispatcher _mainDispatcher = Dispatcher.CurrentDispatcher;
        private IDictionary<DrawColor, Color> _colorCache;
        private WriteableBitmap _bitmap;

        /// <summary>
        /// Creates a new instance of <see cref="GraphicContainer"/>.
        /// </summary>
        public GraphicContainer()
        {
            SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// Resize the <see cref="WriteableBitmap"/> based on new control size.
        /// </summary>
        private void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            //Resize the current bitmap
            _bitmap = _bitmap.Resize(
                (int)e.NewSize.Width
                , (int)e.NewSize.Height
                , WriteableBitmapExtensions.Interpolation.Bilinear
            );

            Source = _bitmap;
        }

        /// <summary>
        /// Initialize the <see cref="WriteableBitmap"/> with the current width and height.
        /// </summary>
        public Task Initialize(IDictionary<DrawColor, string> drawColorMap)
        {
            //Cache the colors
            _colorCache = new ReadOnlyDictionary<DrawColor, Color>(
                drawColorMap.ToDictionary(
                    kvp => kvp.Key
                    , kvp => (Color) (ColorConverter.ConvertFromString(kvp.Value) ?? Colors.White)
                )
            );

            //Since the control has no size yet simply draw a size bitmap
            _bitmap = BitmapFactory.New(0, 0);
            Source = _bitmap;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Draws the collection of <see cref="IGraphicLine"/>s and <see cref="IGraphicPolygon"/>s
        /// to the screen.
        /// </summary>
        public async Task Draw(IEnumerable<IGraphicLine> lines, IEnumerable<IGraphicPolygon> polygons)
        {
            try
            {
                await _mainDispatcher.InvokeAsync(() =>
                {
                    _bitmap.Clear();

                    foreach (var gline in lines)
                    {
                        _bitmap.DrawLine(
                            gline.Point1.X
                            , gline.Point1.Y
                            , gline.Point2.X
                            , gline.Point2.Y
                            , _colorCache[gline.Color]
                        );
                    }

                    foreach (var gpoly in polygons)
                    {
                        var points = new int[gpoly.Points.Count * 2 + 2];

                        for (int i = 0, c = 0; i < points.Length - 2; i += 2, c++)
                        {
                            var p = gpoly.Points[c];
                            points[i] = p.X;
                            points[i + 1] = p.Y;
                        }

                        var first = gpoly.Points.First();
                        points[^2] = first.X;
                        points[^1] = first.Y;

                        _bitmap.DrawPolyline(
                            points
                            , _colorCache[gpoly.Color]
                        );
                    }
                });
            }
            catch (Exception)
            {
                //Ignore
            }
        }

        /// <summary>
        /// Cleanup handlers.
        /// </summary>
        public void Dispose()
        {
            SizeChanged -= OnSizeChanged;
        }
    }
}
