window.onresize = () => {
    //Send dotnet an update
    DotNet.invokeMethodAsync(
        'Asteroids.BlazorComponents'
        , 'UpdateWindowSize'
        , window.innerWidth
        , window.innerHeight
    );
};

window.JsInteropAsteroidsWindow = {

    initialize: () => {

        //let dotnet know it is ready
        DotNet.invokeMethodAsync(
            'Asteroids.BlazorComponents'
            , 'WindowInitialized'
            , window.innerWidth
            , window.innerHeight
        );

        return true;
    },
};