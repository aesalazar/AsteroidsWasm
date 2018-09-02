const soundAudios = [];

window.JsAsteroidsSound = {

    loadSounds: function (sounds) {
        console.warn(sounds);

        sounds.forEach(sound => {
            const audio = new Audio(sound["path"]);
            soundAudios[sound["id"]] = audio;
        });

        return true;
    },

    play: function (id) {
        const audio = soundAudios[id];
        audio.currentTime = 0;
        audio.play();

        return true;
    },
};
