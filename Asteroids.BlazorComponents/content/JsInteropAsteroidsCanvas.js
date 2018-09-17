let visCanvas;
let visContext;
let lastColor;

//pre-render canvas
const preCanvas = document.createElement("canvas");
const preContext = preCanvas.getContext("2d");

window.onresize = () => {
    const w = window.innerWidth;
    const h = window.innerHeight;

    visCanvas.width = w;
    visCanvas.height = h;

    preCanvas.width = w;
    preCanvas.height = h;

    preContext.stroke();
    preContext.beginPath();
    preContext.strokeStyle = lastColor;

    //Send dotnet an update
    DotNet.invokeMethodAsync(
        'Asteroids.BlazorComponents'
        , 'UpdateCanvasSize'
        , window.innerWidth
        , window.innerHeight
    );
};

window.JsInteropAsteroidsCanvas = {

    initialize: (canvasElement) => {
        visCanvas = canvasElement;
        if (visCanvas === null)
            return false;

        visContext = visCanvas.getContext("2d");
        if (visContext === null)
            return false;

        //Set the dimensions
        const w = window.innerWidth;
        const h = window.innerHeight;

        visCanvas.width = w;
        visCanvas.height = h;

        preCanvas.width = w;
        preCanvas.height = h;

        //let dotnet know it is ready
        DotNet.invokeMethodAsync(
            'Asteroids.BlazorComponents'
            , 'CanvasInitialized'
            , w
            , h
        );

        return true;
    },

    clear: () => {
        preContext.clearRect(
            0,
            0,
            visCanvas.clientWidth,
            visCanvas.clientHeight
        );

        preContext.beginPath();

        return true;
    },

    paint: () => {

        //commit the queued vectors and paint
        preContext.stroke();

        visContext.clearRect(
            0,
            0,
            visCanvas.clientWidth,
            visCanvas.clientHeight
        );

        visContext.drawImage(preCanvas, 0, 0);
    },

    drawLines: (lines) => {

        lines.forEach(line => {
            const colorHex = line.colorHex;
            const point1 = line.point1;
            const point2 = line.point2;

            //If start of a new line color
            if (lastColor !== colorHex) {
                preContext.stroke();
                preContext.beginPath();
                preContext.strokeStyle = colorHex;
                lastColor = colorHex;
            }

            //Connect the points
            preContext.moveTo(point1.x, point1.y);
            preContext.lineTo(point2.x, point2.y);
        });

        return true;
    },

    drawPolygons: (polygons) => {

        polygons.forEach(poly => {
            const colorHex = poly.colorHex;
            const points = poly.points;

            //If start of a new line color
            if (lastColor !== colorHex) {
                preContext.stroke();
                preContext.beginPath();
                preContext.strokeStyle = colorHex;
                lastColor = colorHex;
            }

            //Connect the points
            const first = points[0];
            preContext.moveTo(first.x, first.y);

            points.forEach(pt => preContext.lineTo(pt.x, pt.y));
            preContext.closePath();
        });

        return true;
    }
};