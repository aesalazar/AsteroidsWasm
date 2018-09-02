using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Asteroids.BlazorComponents.JsInterop
{
    public class InteropSounds
    {
        //JS method container name
        private const string JsAsteroidsSound = nameof(JsAsteroidsSound);

        //Commands
        private const string loadSounds = nameof(loadSounds);
        private const string play = nameof(play);

        //loaded sounds
        private IDictionary<string, int> soundDict = new Dictionary<string, int>();

        /// <summary>
        /// Call JavaScript to load sounds to Audio objects.
        /// </summary>
        /// <param name="fileNames">Collection of wav file names.</param>
        public Task<string> LoadSounds(IEnumerable<string> fileNames)
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

            return JSRuntime.Current.InvokeAsync<string>(
                $"{JsAsteroidsSound}.{loadSounds}"
                , sounds
            );
        }

        /// <summary>
        /// Call JavaScript to play a sound.
        /// </summary>
        /// <param name="name">Sound to play.</param>
        /// <returns></returns>
        public Task<string> Play(string name)
        {
            return JSRuntime.Current.InvokeAsync<string>(
                $"{JsAsteroidsSound}.{play}"
                , soundDict[name]
            );
        }
    }
}
