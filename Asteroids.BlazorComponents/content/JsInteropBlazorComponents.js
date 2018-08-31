let canvas;
let context;

window.JsInteropBlazorComponents = {

    initialize: (canvasId) => {
        canvas = document.getElementById(canvasId);
        console.warn("canvas:", canvas);

        if (canvas === null)
            return false;

        context = canvas.getContext("2d");
        console.warn("context:", context);

        if (context === null)
            return false;

        return true;
    },

    clear: () => {
        context.clearRect(
            0,
            0,
            canvas.clientWidth,
            canvas.clientHeight
        );

        //Make sure lines are cleared
        context.beginPath();
        window.requestAnimationFrame(() => { });

        return true;
    },

    drawLine: (colorHex, point1, point2) => {
        context.strokeStyle = colorHex;

        context.beginPath();
        context.lineTo(point1.x, point1.y);
        context.lineTo(point2.x, point2.y);
        context.stroke();

        return true;
    },

    drawPolygon: (colorHex, points) => {
        context.strokeStyle = colorHex;

        context.beginPath();
        points.forEach(pt => context.lineTo(pt.x, pt.y));

        var first = points[0];
        context.lineTo(first.x, first.y);

        context.stroke();

        return true;
    }
};