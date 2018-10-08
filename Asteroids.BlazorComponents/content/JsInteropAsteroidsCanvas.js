window.onresize = () => {
    //Send dotnet an update
    DotNet.invokeMethodAsync(
        'Asteroids.BlazorComponents'
        , 'UpdateCanvasSize'
        , window.innerWidth
        , window.innerHeight
    );
};

window.JsInteropAsteroidsCanvas = {

    initialize: () => {

        //let dotnet know it is ready
        DotNet.invokeMethodAsync(
            'Asteroids.BlazorComponents'
            , 'CanvasInitialized'
            , window.innerWidth
            , window.innerHeight
        );

        return true;
    },
};