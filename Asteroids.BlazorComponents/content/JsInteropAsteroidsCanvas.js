let canvas;
let context;
let lastColor;

//pre-render canvas
const preCanvas = document.createElement("canvas");
const preContext = preCanvas.getContext("2d");

window.JsInteropAsteroidsCanvas = {

    initialize: (canvasElement) => {
        canvas = canvasElement;
        if (canvas === null)
            return false;

        context = canvas.getContext("2d");
        if (context === null)
            return false;

        preCanvas.width = canvas.clientWidth;
        preCanvas.height = canvas.clientHeight;

        return true;
    },

    clear: () => {
        preContext.clearRect(
            0,
            0,
            canvas.clientWidth,
            canvas.clientHeight
        );

        preContext.beginPath();

        return true;
    },

    paint: () => {

        //commit the queued vectors and paint
        preContext.stroke();

        context.clearRect(
            0,
            0,
            canvas.clientWidth,
            canvas.clientHeight
        );

        context.drawImage(preCanvas, 0, 0);
    },

    drawLine: (colorHex, point1, point2) => {

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

        return true;
    },

    drawPolygon: (colorHex, points) => {

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

        return true;
    }
};