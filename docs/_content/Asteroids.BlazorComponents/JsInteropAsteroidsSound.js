const soundAudios = [];

function b64toBlob(b64Data, contentType, sliceSize) {
    contentType = contentType || '';
    sliceSize = sliceSize || 512;

    const byteCharacters = atob(b64Data);
    const byteArrays = [];

    for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
        const slice = byteCharacters.slice(offset, offset + sliceSize);

        const byteNumbers = new Array(slice.length);
        for (let i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }

        const byteArray = new Uint8Array(byteNumbers);

        byteArrays.push(byteArray);
    }

    return new Blob(byteArrays, { type: contentType });
}

window.JsAsteroidsSound = {

    loadSounds: function (sounds) {
        sounds.forEach(sound => {
            const str64 = localStorage.getItem(sound.path).slice(1, -1);;
            const blob = b64toBlob(str64, "audio/wav");
            const blobUrl = URL.createObjectURL(blob);

            const audio = new Audio(blobUrl);
            soundAudios[sound.id] = audio;
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
