window.jsInteropWindow = {
    registerDotNetInterop: function (dotNetReference) {
        window.onresize = function () {
            //Send dotnet an update
            dotNetReference.invokeMethodAsync(
                'JsUpdateWindowSize'
                , window.innerWidth
                , window.innerHeight
            );
        };

        //let dotnet know it is ready
        dotNetReference.invokeMethodAsync(
            'JsWindowInitialized'
            , window.innerWidth
            , window.innerHeight
        );
    }
}