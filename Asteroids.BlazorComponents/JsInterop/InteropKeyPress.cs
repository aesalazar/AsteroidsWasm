using System;
using Microsoft.JSInterop;

namespace Asteroids.BlazorComponents.JsInterop
{
    public static class InteropKeyPress
    {

        public static event EventHandler<ConsoleKey> KeyUp;
        public static event EventHandler<ConsoleKey> KeyDown;

        [JSInvokable]
        public static void JsKeyDown(int e)
        {
            var handler = KeyDown;

            if (handler == null)
                return;

            var consoleKey = (ConsoleKey)Enum.Parse(typeof(ConsoleKey), e.ToString());
            handler.Invoke(null, consoleKey);
        }

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
