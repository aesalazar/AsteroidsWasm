using System;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Asteroids.BlazorComponents.JsInterop
{
    /// <summary>
    /// Proxy for drawing on a JavaScript Window.
    /// </summary>
    public class InteropWindow
    {
        #region Properties

        /// <summary>
        /// JavaScript method container name.
        /// </summary>
        private const string JsInteropAsteroidsWindow = nameof(JsInteropAsteroidsWindow);

        /// <summary>
        /// JavaScript method to prep the Window for receiving draw commands.
        /// </summary>
        private const string initialize = nameof(initialize);

        #endregion

        #region Methods

        /// <summary>
        /// Call JavaScript to prep the Window.
        /// </summary>
        public async Task<string> Initialize()
        {
            return await JSRuntime.Current.InvokeAsync<string>(
                $"{JsInteropAsteroidsWindow}.{initialize}"
            );
        }

        #endregion

        #region JavaScript Calls and Events

        /// <summary>
        /// Fires when the Window in ready in JavaScript.
        /// </summary>
        public static event EventHandler<Rectangle> Initialized;

        /// <summary>
        /// Fires when the Window is resized in JavaScript.
        /// </summary>
        public static event EventHandler<Rectangle> SizeChanged;

        /// <summary>
        /// Called from JavaScript when the Window is resized.
        /// </summary>
        [JSInvokable]
        public static Task<bool> UpdateWindowSize(int width, int height)
        {
            SizeChanged?.Invoke(null, new Rectangle(0, 0, width, height));
            return Task.FromResult(true);
        }

        /// <summary>
        /// Called from JavaScript when the cavas is ready and sized.
        /// </summary>
        [JSInvokable]
        public static Task<bool> WindowInitialized(int width, int height)
        {
            Initialized?.Invoke(null, new Rectangle(0, 0, width, height));
            return Task.FromResult(true);
        }

        #endregion

    }
}
