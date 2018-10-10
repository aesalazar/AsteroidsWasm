document.onkeydown = function (evt) {
    evt = evt || window.event;
    DotNet.invokeMethodAsync('Asteroids.BlazorComponents', 'JsKeyDown', evt.keyCode);
};

document.onkeyup = function (evt) {
    evt = evt || window.event;
    DotNet.invokeMethodAsync('Asteroids.BlazorComponents', 'JsKeyUp', evt.keyCode);
};