using Asteroids.BlazorComponents.Classes;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Sounds;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        /// Collection of loaded sounds.
        /// </summary>
        private readonly IDictionary<string, int> _soundDict = new Dictionary<string, int>();

        /// <summary>
        /// JavaScript method container name.
        /// </summary>
        private const string JsAsteroidsSound = nameof(JsAsteroidsSound);

        /// <summary>
        /// JavaScript localStorage method container name.
        /// </summary>
        private const string JsAsteroidsLocalStorage = nameof(JsAsteroidsLocalStorage);

        /// <summary>
        /// JavaScript method to call when sounds are to be loaded.
        /// </summary>
        private const string loadSounds = nameof(loadSounds);

        /// <summary>
        /// JavaScript method to call to write a sound blob string to localStorage.
        /// </summary>
        private const string writeStorage = nameof(writeStorage);

        /// <summary>
        /// JavaScript method to call when a sound is to be played.
        /// </summary>
        private const string play = nameof(play);

        /// <summary>
        /// Store sound IDs to <see cref="_soundDict"/> and call JavaScript to load sounds to Audio objects.
        /// </summary>
        /// <param name="fileNames">Collection of wav file names.</param>
        private async Task LoadSounds(IEnumerable<string> fileNames)
        {
            var sounds = fileNames
                .Select(name =>
                {
                    var snd = new
                    {
                        id = _soundDict.Count,
                        path = name
                    };
                    _soundDict.Add(name, snd.id);

                    return snd;
                })
                .ToList();

            await _jsRuntime.InvokeAsync<string>(
                $"{JsAsteroidsSound}.{loadSounds}"
                , sounds
            );
        }

        #region Public Methods

        /// <summary>
        /// Loads sound <see cref="System.IO.Stream"/>s stored in <see cref="ActionSounds.SoundDictionary"/>
        /// to HTML localStorage.
        /// </summary>
        public async Task Initialize()
        {
            //First load the stream to storage
            foreach (var kvp in ActionSounds.SoundDictionary)
            {
                var str = kvp.Value.ToBase64();

                await _jsRuntime.InvokeAsync<object>(
                    $"{JsAsteroidsLocalStorage}.{writeStorage}"
                    , kvp.Key.ToString().ToLower()
                    , str
                );
            }

            //Load the sounds in JavaScript
            var sounds = Enum
                .GetNames(typeof(ActionSound))
                .Select(s => s.ToLowerInvariant());

            await LoadSounds(sounds);
        }

        /// <summary>
        /// Call JavaScript to play a sound.
        /// </summary>
        /// <param name="name">Sound to play.</param>
        public async Task Play(string name)
        {
            await _jsRuntime.InvokeAsync<string>(
                $"{JsAsteroidsSound}.{play}"
                , _soundDict[name]
            );
        }

        #endregion
    }
}
