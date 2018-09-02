using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Asteroids.BlazorComponents.JsInterop;
using Asteroids.Standard;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;
using Microsoft.AspNetCore.Blazor.Components;

namespace Asteroids.BlazorComponents.Components
{
    public class GraphicsContainerComponent : BlazorComponent, IGraphicContainer
    {
        #region Blazor Parameters

        [Parameter]
        protected string CanvasId { get; set; } = nameof(Asteroids) + nameof(BlazorComponents) + nameof(GraphicsContainerComponent);

        [Parameter]
        protected int CanvasWidth { get; set; } = 650;

        [Parameter]
        protected int CanvasHeight { get; set; } = 500;

        #endregion

        #region Constructor and Fields

        private readonly GameController _controller;
        private InteropCanvas _canvas;

        public GraphicsContainerComponent()
        {
            _controller = new GameController(this);
            _controller.Initialize(new Rectangle(0, 0, CanvasWidth, CanvasHeight));
        }

        #endregion

        #region Implementation of IGraphicContainer

        public void Initialize(GameController controller, Rectangle rectangle)
        {
            InteropKeyPress.KeyUp += OnKeyUp;
            InteropKeyPress.KeyDown += OnKeyDown;

            _canvas = new InteropCanvas();
            SetDimensions(rectangle);

            Task.Factory.StartNew(() => _canvas.Initialize(CanvasId));
        }

        public void SetDimensions(Rectangle rectangle)
        {
            CanvasWidth = rectangle.Width;
            CanvasHeight = rectangle.Height;
        }

        public void Activate()
        {
            _canvas.Clear(() => _controller.Repaint(this));
        }

        public void DrawLine(string colorHex, Point point1, Point point2)
        {
            _canvas.DrawLine(colorHex, point1, point2);
        }

        public void DrawPolygon(string colorHex, IEnumerable<Point> points)
        {
            _canvas.DrawPolygon(colorHex, points);
        }

        #endregion

        #region Methods

        private void OnKeyDown(object sender, ConsoleKey e)
        {
            PlayKey key;
            switch (e)
            {
                case ConsoleKey.Escape:
                    //Do not send exit command
                    if (_controller.GameStatus != GameMode.Game)
                        return;

                    key = PlayKey.Escape;
                    break;

                case ConsoleKey.LeftArrow:
                    key = PlayKey.Left;
                    break;

                case ConsoleKey.RightArrow:
                    key = PlayKey.Right;
                    break;

                case ConsoleKey.UpArrow:
                    key = PlayKey.Up;
                    break;

                case ConsoleKey.DownArrow:
                    key = PlayKey.Down;
                    break;

                case ConsoleKey.Spacebar:
                    key = PlayKey.Space;
                    break;

                case ConsoleKey.P:
                    key = PlayKey.P;
                    break;

                default:
                    return;
            }

            _controller.KeyDown(key);

        }

        private void OnKeyUp(object sender, ConsoleKey e)
        {
            PlayKey key;
            switch (e)
            {
                case ConsoleKey.Escape:
                    //Do not send exit command
                    if (_controller.GameStatus != GameMode.Game)
                        return;

                    key = PlayKey.Escape;
                    break;

                case ConsoleKey.LeftArrow:
                    key = PlayKey.Left;
                    break;

                case ConsoleKey.RightArrow:
                    key = PlayKey.Right;
                    break;

                case ConsoleKey.UpArrow:
                    key = PlayKey.Up;
                    break;

                case ConsoleKey.DownArrow:
                    key = PlayKey.Down;
                    break;

                case ConsoleKey.Spacebar:
                    key = PlayKey.Space;
                    break;

                case ConsoleKey.P:
                    key = PlayKey.P;
                    break;

                default:
                    return;
            }

            _controller.KeyUp(key);
        }

        #endregion
    }
}
