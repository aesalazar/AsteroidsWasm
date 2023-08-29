using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids.BlazorComponents.JsInterop
{
    /// <summary>
    /// General consts used by the components.
    /// </summary>
    public static class InteropConstants
    {
        /// <summary>
        /// Name of the Method invoked by javascript to register interop with .net.
        /// </summary>
        public const string JsInteropRegistrationMethodName = "registerDotNetInterop";

        /// <summary>
        /// Name of the class set on the 'window' object in javascript for Key Press interop.
        /// </summary>
        public const string JsInteropKeyPressClassName = "js" + nameof(InteropKeyPress);

        /// <summary>
        /// Name of the class set on the 'window' object in javascript for Window interop.
        /// </summary>
        public const string JsInteropWindowClassName = "js" + nameof(InteropWindow);

        /// <summary>
        /// Name of the class set on the 'window' object in javascript for Sound interop.
        /// </summary>
        public const string JsInteropSoundsClassName = "js" + nameof(InteropSounds);

        /// <summary>
        /// Name of the Method invoked in javascript to play a registered sound.
        /// </summary>
        public const string JsInteropSoundsPlayMethodName = "play";

    }
}
