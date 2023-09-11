const soundAudios = [];

function b64toBlob(b64Data, contentType, sliceSize) {
    contentType = contentType || "";
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

window.jsInteropSounds = {

    registerDotNetInterop: function(sounds) {
        try {
            sounds.forEach((str64, idx) => {
                const blob = b64toBlob(str64, "audio/wav");
                const blobUrl = URL.createObjectURL(blob);
                soundAudios[idx] = new window.Audio(blobUrl);
            });

            return true;

        } catch (e) {
            console.error(e);
            return false;
        }
    },

    play: function (id) {
        try {
            const audio = soundAudios[id];
            audio.currentTime = 0;
            audio.play();

            return true;

        } catch (e) {
            return false;
        } 
    }
};
