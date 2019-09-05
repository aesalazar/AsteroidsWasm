using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Asteroids.Standard.Enums;

namespace Asteroids.Standard.Sounds
{
    /// <summary>
    /// Collection of <see cref="ActionSound"/> <see cref="Stream"/>s.
    /// </summary>
    public class ActionSounds
    {
        private const string SoundDir = "Sounds";

        static ActionSounds()
        {
            var asmName = $"{nameof(Asteroids)}.{nameof(Standard)}";
            var dirName = $"{asmName}.{SoundDir}";
            var assembly = AppDomain
                .CurrentDomain
                .GetAssemblies().First(a => a.GetName().Name == asmName);

            SoundDictionary = new ReadOnlyDictionary<ActionSound, Stream>(new Dictionary<ActionSound, Stream>
            {
                {ActionSound.Fire, assembly.GetManifestResourceStream($"{dirName}.fire.wav")},
                {ActionSound.Life, assembly.GetManifestResourceStream($"{dirName}.life.wav")},
                {ActionSound.Thrust, assembly.GetManifestResourceStream($"{dirName}.thrust.wav")},
                {ActionSound.Explode1, assembly.GetManifestResourceStream($"{dirName}.explode1.wav")},
                {ActionSound.Explode2, assembly.GetManifestResourceStream($"{dirName}.explode2.wav")},
                {ActionSound.Explode3, assembly.GetManifestResourceStream($"{dirName}.explode3.wav")},
                {ActionSound.Saucer, assembly.GetManifestResourceStream($"{dirName}.lsaucer.wav")},
            });
        }

        /// <summary>
        /// Collection of <see cref="ActionSound"/> WAV file <see cref="Stream"/>s.
        /// </summary>
        public static IDictionary<ActionSound, Stream> SoundDictionary { get; }

        /// <summary>
        /// Fires when a call is made within the game engine to play a sound.
        /// </summary>
        public static event EventHandler<ActionSound> SoundTriggered;

        /// <summary>
        /// Invokes <see cref="SoundTriggered"/> to play an <see cref="ActionSound"/>.
        /// </summary>
        /// <param name="sender">Calling object.</param>
        /// <param name="sound">Sound to play.</param>
        public static void PlaySound(object sender, ActionSound sound)
        {
            SoundTriggered?.Invoke(sender, sound);
        }
    }
}
