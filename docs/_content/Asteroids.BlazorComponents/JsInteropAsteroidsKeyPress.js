window.jsInteropKeyPress = {
    registerDotNetInterop: function (dotNetReference) {
        document.onkeydown = function (evt) {
            evt = evt || window.event;
            dotNetReference.invokeMethodAsync('JsKeyDown', evt.keyCode);
            //Prevent all but F5 and F12
            if (evt.keyCode !== 116 && evt.keyCode !== 123)
                evt.preventDefault();
        };

        document.onkeyup = function (evt) {
            evt = evt || window.event;
            dotNetReference.invokeMethodAsync('JsKeyUp', evt.keyCode);

            //Prevent all but F5 and F12
            if (evt.keyCode !== 116 && evt.keyCode !== 123)
                evt.preventDefault();
        };        
    }
}