using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
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
        /// <param name="canvasElement">Canvas element to apply to.</param>
        public async Task<string> Initialize(ElementRef canvasElement)
        {
            return await JSRuntime.Current.InvokeAsync<string>(
                $"{JsInteropAsteroidsCanvas}.{initialize}"
                , canvasElement
            );
        }

        /// <summary>
        /// Call JavaScript to clear the canvas.
        /// </summary>
        public async Task<string> Clear()
        {
            return await JSRuntime.Current.InvokeAsync<string>(
                $"{JsInteropAsteroidsCanvas}.{clear}"
            );
        }

        /// <summary>
        /// Call JavaScript to draw a line on the canvas.
        /// </summary>
        /// <param name="colorHex">HTML color hex, e.g. #000000</param>
        /// <param name="point1">Starting vertex point.</param>
        /// <param name="point2">Ending vertex point.</param>
        public async Task<string> DrawLine(string colorHex, Point point1, Point point2)
        {
            return await JSRuntime.Current.InvokeAsync<string>(
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
        public async Task<string> DrawPolygon(string colorHex, IEnumerable<Point> points)
        {
            return await JSRuntime.Current.InvokeAsync<string>(
                $"{JsInteropAsteroidsCanvas}.{drawPolygon}"
                , colorHex
                , points
            );
        }

        #endregion

    }
}
