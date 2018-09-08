using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Asteroids.Standard.Interfaces;
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
        /// JavaScript method to paint the canvas.
        /// </summary>
        private const string paint = nameof(paint);

        /// <summary>
        /// JavaScript method to draw lines on the canvas.
        /// </summary>
        private const string drawLines = nameof(drawLines);

        /// <summary>
        /// JavaScript method to draw polygons on the canvas.
        /// </summary>
        private const string drawPolygons = nameof(drawPolygons);

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
        /// Call JavaScript to paint the canvas with the current queue.
        /// </summary>
        public async Task<string> Paint()
        {
            return await JSRuntime.Current.InvokeAsync<string>(
                $"{JsInteropAsteroidsCanvas}.{paint}"
            );
        }

        /// <summary>
        /// Call JavaScript to queue lines to be drawn on the canvas.
        /// </summary>
        /// <param name="colorHex">HTML color hex, e.g. #000000</param>
        /// <param name="point1">Starting vertex point.</param>
        /// <param name="point2">Ending vertex point.</param>
        public async Task<string> DrawLines(IEnumerable<IGraphicLine> lines)
        {
            return await JSRuntime.Current.InvokeAsync<string>(
                $"{JsInteropAsteroidsCanvas}.{drawLines}"
                , lines
            );
        }

        /// <summary>
        /// Call JavaScript to queue polygons to be drawn on the canvas.
        /// </summary>
        /// <param name="colorHex">HTML color hex, e.g. #000000</param>
        /// <param name="points">Collection of vertex points.</param>
        public async Task<string> DrawPolygons(IEnumerable<IGraphicPolygon> polygons)
        {
            return await JSRuntime.Current.InvokeAsync<string>(
                $"{JsInteropAsteroidsCanvas}.{drawPolygons}"
                , polygons
            );
        }
        #endregion

    }
}
