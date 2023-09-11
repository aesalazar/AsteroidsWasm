using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Asteroids.BlazorComponents.JsInterop
{
    /// <summary>
    /// Proxy for receiving key press events from JavaScript.
    /// </summary>
    public sealed class InteropKeyPress : IDisposable
    {
        #region Properties

        /// <summary>
        /// JavaScript reference created when registering this instance.
        /// </summary>
        private DotNetObjectReference<InteropKeyPress>? _dotNetReference;

        #endregion

        #region Events

        /// <summary>
        /// Fires when a KeyUp message is received from JavaScript.
        /// </summary>
        public event EventHandler<ConsoleKey>? KeyUp;

        /// <summary>
        /// Fires when a KeyDown message is received from JavaScript.
        /// </summary>
        public event EventHandler<ConsoleKey>? KeyDown;

        #endregion

        #region Methods

        /// <summary>
        /// Registers this instance with JavaScript.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime to hook into.</param>
        public async Task Initialize(IJSRuntime jSRuntime)
        {
            _dotNetReference = DotNetObjectReference.Create(this);

            await jSRuntime.InvokeVoidAsync(
                $"{InteropConstants.JsInteropKeyPressClassName}.{InteropConstants.JsInteropRegistrationMethodName}",
                _dotNetReference
            );
        }

        /// <summary>
        /// Called by JavaScript when a Key Down event fires.
        /// </summary>
        /// <param name="e"><see cref="ConsoleKey"/> number.</param>
        /// <returns>
        /// JavaScript Promise with the converted <see cref="ConsoleKey"/> value or <see langword="null"/> if
        /// no equivalent is found.
        /// </returns>
        [JSInvokable]
        public Task<bool> JsKeyDown(int e)
        {
            var found = false;
            var consoleKey = default(ConsoleKey);

            try
            {
                consoleKey = (ConsoleKey)e;
                found = true;
            }
            catch
            {
                Console.WriteLine($"Cound not find {nameof(ConsoleKey)} for JS key value {e})");
            }

            if (found)
                KeyDown?.Invoke(null, consoleKey);

            return Task.FromResult(found);
        }

        /// <summary>
        /// Called by JavaScript when a Key Up event fires.
        /// </summary>
        /// <param name="e"><see cref="ConsoleKey"/> number.</param>
        /// <returns>
        /// JavaScript Promise with the converted <see cref="ConsoleKey"/> value or <see langword="null"/> if
        /// no equivalent is found.
        /// </returns>
        [JSInvokable]
        public Task<bool> JsKeyUp(int e)
        {
            var found = false;
            var consoleKey = default(ConsoleKey);

            try
            {
                consoleKey = (ConsoleKey)e;
                found = true;
            }
            catch
            {
                Console.WriteLine($"Cound not find {nameof(ConsoleKey)} for JS key value {e})");
            }

            if (found)
                KeyUp?.Invoke(null, consoleKey);

            return Task.FromResult(found);
        }

        /// <summary>
        /// Clears any references to JavaScript.
        /// </summary>
        public void Dispose()
        {
            _dotNetReference?.Dispose();
        }

        #endregion
    }
}