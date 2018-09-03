using System;
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
        [JSInvokable]
        public static void JsKeyDown(int e)
        {
            var handler = KeyDown;

            if (handler == null)
                return;

            var consoleKey = (ConsoleKey)Enum.Parse(typeof(ConsoleKey), e.ToString());
            handler.Invoke(null, consoleKey);
        }

        /// <summary>
        /// Called by JavaScript when a Key Up event fires.
        /// </summary>
        /// <param name="e"><see cref="ConsoleKey"/> number.</param>
        [JSInvokable]
        public static void JsKeyUp(int e)
        {
            var handler = KeyUp;

            if (handler == null)
                return;

            var consoleKey = (ConsoleKey)Enum.Parse(typeof(ConsoleKey), e.ToString());
            handler.Invoke(null, consoleKey);
        }

    }
}
