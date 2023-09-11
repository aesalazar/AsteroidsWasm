using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Asteroids.BlazorComponents.Classes;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Sounds;
using Microsoft.JSInterop;

namespace Asteroids.BlazorComponents.JsInterop
{
    /// <summary>
    /// Proxy to manage sounds stored in JavaScript.
    /// </summary>
    public sealed class InteropSounds
    {
        #region Properties

        /// <summary>
        /// JavaScript runtime bridge.
        /// </summary>
        private IJSRuntime? _jsRuntime;

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads sound <see cref="System.IO.Stream"/>s stored in <see cref="ActionSounds.SoundDictionary"/>
        /// to HTML localStorage.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime to hook into.</param>
        /// <param name="actionSoundMap">Collection of <see cref="ActionSound"/> <see cref="Stream"/>s to cache.</param>
        /// <returns>Indication if the sounds were loaded successfully.</returns>
        public async Task<bool> Initialize(
            IJSRuntime jsRuntime
            , IDictionary<ActionSound, Stream> actionSoundMap)
        {
            _jsRuntime = jsRuntime;

            //Load the sounds in JavaScript indexed by enum value
            var sounds = actionSoundMap
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value.ToBase64())
                .ToList();

            //Index in the collection will be the map
            return await _jsRuntime.InvokeAsync<bool>(
                $"{InteropConstants.JsInteropSoundsClassName}.{InteropConstants.JsInteropRegistrationMethodName}"
                , sounds
            );
        }

        /// <summary>
        /// Call JavaScript to play a sound.
        /// </summary>
        /// <param name="sound">Sound to play.</param>
        /// <returns>Indication if the sound was played successfully.</returns>
        public async Task<bool> Play(ActionSound sound)
        {
            if (_jsRuntime == null)
                throw new TypeInitializationException(GetType().Name, new NullReferenceException(nameof(_jsRuntime)));

            //Returns null so use object type
            return await _jsRuntime.InvokeAsync<bool>(
                $"{InteropConstants.JsInteropSoundsClassName}.{InteropConstants.JsInteropSoundsPlayMethodName}"
                , sound
            );
        }

        #endregion
    }
}
