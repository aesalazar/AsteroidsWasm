using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Asteroids.Standard.Enums;

namespace Asteroids.Standard.Sounds
{
    public static class ActionSounds
    {
        private static readonly string SOUND_DIR = "Sounds";

        static ActionSounds()
        {
            var assemName = $"{nameof(Asteroids)}.{nameof(Standard)}";
            var assembly = AppDomain
                .CurrentDomain
                .GetAssemblies().First(a => a.GetName().Name == assemName);

            SoundDictionary = new Dictionary<ActionSound, Stream>
            {
                { ActionSound.Fire, assembly.GetManifestResourceStream($"{assemName}.{SOUND_DIR}.fire.wav") },
                { ActionSound.Life, assembly.GetManifestResourceStream($"{assemName}.{SOUND_DIR}.life.wav") },
                { ActionSound.Thrust, assembly.GetManifestResourceStream($"{assemName}.{SOUND_DIR}.thrust.wav") },
                { ActionSound.Explode1, assembly.GetManifestResourceStream($"{assemName}.{SOUND_DIR}.explode1.wav") },
                { ActionSound.Explode2, assembly.GetManifestResourceStream($"{assemName}.{SOUND_DIR}.explode2.wav") },
                { ActionSound.Explode3, assembly.GetManifestResourceStream($"{assemName}.{SOUND_DIR}.explode3.wav") },
                { ActionSound.Saucer, assembly.GetManifestResourceStream($"{assemName}.{SOUND_DIR}.lsaucer.wav") },
            };
        }


        public static IDictionary<ActionSound, Stream> SoundDictionary { get; }

        public static event EventHandler<ActionSound> SoundTriggered;

        public static void PlaySound(object sender, ActionSound sound)
        {
            SoundTriggered?.Invoke(sender, sound);
        }
    }
}
