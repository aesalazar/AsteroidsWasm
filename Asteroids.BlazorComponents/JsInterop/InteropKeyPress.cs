using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Asteroids.BlazorComponents.JsInterop
{
    /// <summary>
    /// Proxy for receiving key press events from JavaScript.
    /// </summary>
    public static class InteropKeyPress
    {
        /// <summary>
        /// Fires when a KeyUp message is received from JavaScript.
        /// </summary>
        public static event EventHandler<ConsoleKey> KeyUp;

        /// <summary>
        /// Fires when a KeyDown message is received from JavaScript.
        /// </summary>
        public static event EventHandler<ConsoleKey> KeyDown;

        /// <summary>
        /// Called by JavaScript when a Key Down event fires.
        /// </summary>
        /// <param name="e"><see cref="ConsoleKey"/> number.</param>
        /// <returns>
        /// JavaScript Promise with the converted <see cref="ConsoleKey"/> value or <see cref="null"/> if
        /// no equivalent is found.
        /// </returns>
        [JSInvokable]
        public static Task<bool> JsKeyDown(int e)
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
        /// JavaScript Promise with the converted <see cref="ConsoleKey"/> value or <see cref="null"/> if
        /// no equivalent is found.
        /// </returns>
        [JSInvokable]
        public static Task<bool> JsKeyUp(int e)
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
    }
}