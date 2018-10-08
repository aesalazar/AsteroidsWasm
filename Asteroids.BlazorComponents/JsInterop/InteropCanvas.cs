using System;
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

        #endregion

        #region JavaScript Calls and Events

        /// <summary>
        /// Fires when the canvas in ready in JavaScript.
        /// </summary>
        public static event EventHandler<Rectangle> Initialized;

        /// <summary>
        /// Fires when the canvas is resized in JavaScript.
        /// </summary>
        public static event EventHandler<Rectangle> SizeChanged;

        /// <summary>
        /// Called from JavaScript when the canvas is resized.
        /// </summary>
        [JSInvokable]
        public static Task<bool> UpdateCanvasSize(int width, int height)
        {
            SizeChanged?.Invoke(null, new Rectangle(0, 0, width, height));
            return Task.FromResult(true);
        }

        /// <summary>
        /// Called from JavaScript when the cavas is ready and sized.
        /// </summary>
        [JSInvokable]
        public static Task<bool> CanvasInitialized(int width, int height)
        {
            Initialized?.Invoke(null, new Rectangle(0, 0, width, height));
            return Task.FromResult(true);
        }

        #endregion

    }
}
