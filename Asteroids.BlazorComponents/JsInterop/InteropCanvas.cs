using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Asteroids.BlazorComponents.JsInterop
{
    /// <summary>
    /// Proxy for drawing on a JavaScript Canvas.
    /// </summary>
    public class InteropCanvas
    {
        #region Properties

        /// <summary>
        /// JavaScript method container name.
        /// </summary>
        private const string JsInteropAsteroidsCanvas = nameof(JsInteropAsteroidsCanvas);

        /// <summary>
        /// JavaScript method to prep the canvas for receiving draw commands.
        /// </summary>
        private const string initialize = nameof(initialize);

        /// <summary>
        /// JavaScript method to clear the canvas.
        /// </summary>
        private const string clear = nameof(clear);

        /// <summary>
        /// JavaScript method to draw a line on the canvas.
        /// </summary>
        private const string drawLine = nameof(drawLine);

        /// <summary>
        /// JavaScript method to draw a polygon on the canvas.
        /// </summary>
        private const string drawPolygon = nameof(drawPolygon);

        #endregion

        #region Methods
        /// <summary>
        /// Call JavaScript to prep the canvas.
        /// </summary>
        /// <param name="canvasId">Canvas id to apply to.</param>
        public Task<string> Initialize(string canvasId)
        {
            if (string.IsNullOrEmpty(canvasId))
                throw new ArgumentNullException($"Parameter '{nameof(canvasId)}' must have a value");

            if (!string.IsNullOrEmpty(canvasId))
                throw new TypeInitializationException(nameof(InteropCanvas), new Exception($"Instance has already been initialized with canvas id {canvasId}"));

            return JSRuntime.Current.InvokeAsync<string>(
                $"{JsInteropAsteroidsCanvas}.{initialize}"
                , canvasId
            );
        }

        /// <summary>
        /// Call JavaScript to clear the canvas.
        /// </summary>
        /// <param name="callback">Callback method when complete.</param>
        public void Clear(Action callback)
        {
            JSRuntime.Current.InvokeAsync<string>(
                $"{JsInteropAsteroidsCanvas}.{clear}"
            );

            callback();
        }

        /// <summary>
        /// Call JavaScript to draw a line on the canvas.
        /// </summary>
        /// <param name="colorHex">HTML color hex, e.g. #000000</param>
        /// <param name="point1">Starting vertex point.</param>
        /// <param name="point2">Ending vertex point.</param>
        public Task<string> DrawLine(string colorHex, Point point1, Point point2)
        {
            return JSRuntime.Current.InvokeAsync<string>(
                $"{JsInteropAsteroidsCanvas}.{drawLine}"
                , colorHex
                , point1
                , point2
            );
        }

        /// <summary>
        /// Call JavaScript to draw a polygon on the canvas.
        /// </summary>
        /// <param name="colorHex">HTML color hex, e.g. #000000</param>
        /// <param name="points">Collection of vertex points.</param>
        public Task<string> DrawPolygon(string colorHex, IEnumerable<Point> points)
        {
            return JSRuntime.Current.InvokeAsync<string>(
                $"{JsInteropAsteroidsCanvas}.{drawPolygon}"
                , colorHex
                , points
            );
        }

        #endregion

    }
}
