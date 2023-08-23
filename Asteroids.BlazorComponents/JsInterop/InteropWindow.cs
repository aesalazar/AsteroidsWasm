using System;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Asteroids.BlazorComponents.JsInterop
{
    /// <summary>
    /// Proxy for drawing on a JavaScript Window.
    /// </summary>
    public sealed class InteropWindow : IDisposable
    {
        /// <summary>
        /// Creates a new instance of <see cref="InteropWindow"/>.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime bridge.</param>
        public InteropWindow(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        #region Properties

        /// <summary>
        /// JavaScript runtime bridge.
        /// </summary>
        private readonly IJSRuntime _jsRuntime;

        /// <summary>
        /// JavaScript method container name.
        /// </summary>
        private const string JsInteropAsteroidsWindow = nameof(JsInteropAsteroidsWindow);

        /// <summary>
        /// JavaScript method to prep the Window for receiving draw commands.
        /// </summary>
        private const string initialize = nameof(initialize);

        /// <summary>
        /// JavaScript reference created when registering this instance.
        /// </summary>
        private DotNetObjectReference<InteropWindow> _dotNetReference;

        #endregion

        #region Methods

        /// <summary>
        /// Call JavaScript to prep the Window.
        /// </summary>
        public async Task Initialize()
        {
            _dotNetReference = DotNetObjectReference.Create(this);
            
            await _jsRuntime.InvokeVoidAsync(
                $"{InteropConstants.JsInteropWindowClassName}.{InteropConstants.JsInteropRegistrationMethodName}",
                _dotNetReference
            );
        }

        /// <summary>
        /// Clears any references to JavaScript.
        /// </summary>
        public void Dispose()
        {
            _dotNetReference?.Dispose();
        }

        #endregion

        #region JavaScript Calls and Events

        /// <summary>
        /// Fires when the Window in ready in JavaScript.
        /// </summary>
        public event EventHandler<Rectangle> Initialized;

        /// <summary>
        /// Fires when the Window is resized in JavaScript.
        /// </summary>
        public event EventHandler<Rectangle> SizeChanged;

        /// <summary>
        /// Called from JavaScript when the Window is resized.
        /// </summary>
        [JSInvokable]
        public Task<bool> JsUpdateWindowSize(int width, int height)
        {
            SizeChanged?.Invoke(null, new Rectangle(0, 0, width, height));
            return Task.FromResult(true);
        }

        /// <summary>
        /// Called from JavaScript when the cavas is ready and sized.
        /// </summary>
        [JSInvokable]
        public Task<bool> JsWindowInitialized(int width, int height)
        {
            Initialized?.Invoke(null, new Rectangle(0, 0, width, height));
            return Task.FromResult(true);
        }

        #endregion

    }
}
