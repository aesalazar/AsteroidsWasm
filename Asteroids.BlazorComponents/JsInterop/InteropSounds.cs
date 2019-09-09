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
    public class InteropSounds
    {
        /// <summary>
        /// Creates a new instance of <see cref="InteropSounds"/>.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime bridge.</param>
        public InteropSounds(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// JavaScript runtime bridge.
        /// </summary>
        private readonly IJSRuntime _jsRuntime;

        /// <summary>
        /// JavaScript method container name.
        /// </summary>
        private const string JsAsteroidsSound = nameof(JsAsteroidsSound);

        /// <summary>
        /// JavaScript method to call when sounds are to be loaded.
        /// </summary>
        private const string loadSounds = nameof(loadSounds);

        /// <summary>
        /// JavaScript method to call when a sound is to be played.
        /// </summary>
        private const string play = nameof(play);

        #region Public Methods

        /// <summary>
        /// Loads sound <see cref="System.IO.Stream"/>s stored in <see cref="ActionSounds.SoundDictionary"/>
        /// to HTML localStorage.
        /// </summary>
        /// <param name="actionSoundMap">Collection of <see cref="ActionSound"/> <see cref="Stream"/>s to cache.</param>
        /// <returns>Indication if the sounds were loaded successfully.</returns>
        public async Task<bool> Initialize(IDictionary<ActionSound, Stream> actionSoundMap)
        {
            //Load the sounds in JavaScript indexed by enum value
            var sounds = actionSoundMap
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value.ToBase64())
                .ToList();

            //Index in the collection will be the map
            return await _jsRuntime.InvokeAsync<bool>(
                $"{JsAsteroidsSound}.{loadSounds}"
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
            //Returns null so use object type
            return await _jsRuntime.InvokeAsync<bool>(
                $"{JsAsteroidsSound}.{play}"
                , sound
            );
        }

        #endregion
    }
}
