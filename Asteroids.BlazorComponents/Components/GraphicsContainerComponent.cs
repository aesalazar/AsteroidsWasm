using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Asteroids.BlazorComponents.JsInterop;
using Asteroids.Standard;
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
            Task.Factory.StartNew(async () =>
                await _controller.Initialize(new Rectangle(0, 0, CanvasWidth, CanvasHeight))
            );
        }

        #endregion

        #region Implementation of IGraphicContainer

        public async Task Initialize(GameController controller, Rectangle rectangle)
        {
            _canvas = new InteropCanvas();
            await SetDimensions(rectangle);
            await _canvas.Initialize(CanvasId);
        }

        public Task SetDimensions(Rectangle rectangle)
        {
            CanvasWidth = rectangle.Width;
            CanvasHeight = rectangle.Height;

            return Task.CompletedTask;
        }

        public async Task Activate()
        {
            await _canvas.Clear();
            await _controller.Repaint(this);
        }

        public async Task DrawLine(string colorHex, Point point1, Point point2)
        {
            await _canvas.DrawLine(colorHex, point1, point2);
        }

        public async Task DrawPolygon(string colorHex, IEnumerable<Point> points)
        {
            await _canvas.DrawPolygon(colorHex, points);
        }

        #endregion
    }
}
