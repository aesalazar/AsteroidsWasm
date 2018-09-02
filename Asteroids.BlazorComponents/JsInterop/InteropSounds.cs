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
        private const string SoundPath = "sounds/";
        private const string SoundExtension = ".wav";

        //Commands
        private const string loadSounds = nameof(loadSounds);
        private const string play = nameof(play);

        //loaded sounds
        private IDictionary<string, int> soundDict = new Dictionary<string, int>();

        public Task<string> LoadSounds(IEnumerable<string> fileNames)
        {
            foreach (string name in fileNames)
            {
                var id = soundDict.Count;
                soundDict.Add(name, id);
            }

            var sounds = fileNames
                .Select(name => new
                {
                    id = soundDict[name],
                    path = $"{SoundPath}{name}{SoundExtension}"
                }).ToList();

            return JSRuntime.Current.InvokeAsync<string>(
                $"{JsAsteroidsSound}.{loadSounds}"
                , sounds
            );
        }

        public Task<string> Play(string name)
        {
            return JSRuntime.Current.InvokeAsync<string>(
                $"{JsAsteroidsSound}.{play}"
                , soundDict[name]
            );
        }
    }
}
