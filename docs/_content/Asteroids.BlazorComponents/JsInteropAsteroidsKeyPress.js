document.onkeydown = function (evt) {
    evt = evt || window.event;
    DotNet.invokeMethodAsync('Asteroids.BlazorComponents', 'JsKeyDown', evt.keyCode);

    //Prevent all but F5 and F12
    if (evt.keyCode !== 116 && evt.keyCode !== 123)
        evt.preventDefault();
};

document.onkeyup = function (evt) {
    evt = evt || window.event;
    DotNet.invokeMethodAsync('Asteroids.BlazorComponents', 'JsKeyUp', evt.keyCode);

    //Prevent all but F5 and F12
    if (evt.keyCode !== 116 && evt.keyCode !== 123)
        evt.preventDefault();
};