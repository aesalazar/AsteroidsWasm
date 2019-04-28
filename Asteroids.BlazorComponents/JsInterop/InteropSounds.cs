using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        /// <summary>
        /// Collection of loaded sounds.
        /// </summary>
        private IDictionary<string, int> soundDict = new Dictionary<string, int>();

        /// <summary>
        /// Call JavaScript to load sounds to Audio objects.
        /// </summary>
        /// <param name="fileNames">Collection of wav file names.</param>
        public async Task<string> LoadSounds(IEnumerable<string> fileNames)
        {
            var sounds = fileNames
                .Select(name =>
                {
                    var snd = new
                    {
                        id = soundDict.Count,
                        path = name
                    };
                    soundDict.Add(name, snd.id);

                    return snd;
                })
                .ToList();

            return await _jsRuntime.InvokeAsync<string>(
                $"{JsAsteroidsSound}.{loadSounds}"
                , sounds
            );
        }

        /// <summary>
        /// Call JavaScript to play a sound.
        /// </summary>
        /// <param name="name">Sound to play.</param>
        public async Task<string> Play(string name)
        {
            return await _jsRuntime.InvokeAsync<string>(
                $"{JsAsteroidsSound}.{play}"
                , soundDict[name]
            );
        }
    }
}
