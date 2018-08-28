using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Helpers;

namespace Asteroids.Standard.Base
{
    public abstract class CommonOps
    {
        public const double FPS = 60;
        protected const int iMaxX = 10000;
        protected const int iMaxY = 7500;

        protected static Random rndGen = new Random();

        protected const string SOUND_DIR = "Sounds";
        protected static IDictionary<ActionSounds, Stream> Sounds;

        static CommonOps()
        {
            var assemName = $"{nameof(Asteroids)}.{nameof(Standard)}";
            var assembly = AppDomain
                .CurrentDomain
                .GetAssemblies().First(a => a.GetName().Name == assemName);

            Sounds = new Dictionary<ActionSounds, Stream>
            {
                { ActionSounds.Fire, assembly.GetManifestResourceStream($"{assemName}.{SOUND_DIR}.fire.wav") },
                { ActionSounds.Life, assembly.GetManifestResourceStream($"{assemName}.{SOUND_DIR}.life.wav") },
                { ActionSounds.Thrust, assembly.GetManifestResourceStream($"{assemName}.{SOUND_DIR}.thrust.wav") },
                { ActionSounds.Explode1, assembly.GetManifestResourceStream($"{assemName}.{SOUND_DIR}.explode1.wav") },
                { ActionSounds.Explode2, assembly.GetManifestResourceStream($"{assemName}.{SOUND_DIR}.explode2.wav") },
                { ActionSounds.Explode3, assembly.GetManifestResourceStream($"{assemName}.{SOUND_DIR}.explode3.wav") },
            };
        }

        public static event EventHandler<Stream> SoundTriggered;

        protected static void PlaySound(object sender, ActionSounds sound)
        {
            var s = Sounds[sound];
            s.Position = 0;
            SoundTriggered?.Invoke(sender, s);
        }
    }
}
