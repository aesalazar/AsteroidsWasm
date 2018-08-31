using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Asteroids.BlazorComponents.JsInterop
{
    public class InteropCanvas
    {
        #region Properties

        //JS method container name
        private const string JsInteropBlazorComponents = nameof(JsInteropBlazorComponents);

        //Commands
        private const string initialize = nameof(initialize);
        private const string clear = nameof(clear);
        private const string drawLine = nameof(drawLine);
        private const string drawPolygon = nameof(drawPolygon);

        public string CanvasId { get; private set; }

        #endregion

        #region Methods

        public Task<string> Initialize(string canvasId)
        {
            if (string.IsNullOrEmpty(canvasId))
                throw new ArgumentNullException($"Parameter '{nameof(canvasId)}' must have a value");

            if (!string.IsNullOrEmpty(CanvasId))
                throw new TypeInitializationException(nameof(InteropCanvas), new Exception($"Instance has already been initialized with canvas id {canvasId}"));

            CanvasId = canvasId;

            return JSRuntime.Current.InvokeAsync<string>(
                $"{JsInteropBlazorComponents}.{initialize}"
                , CanvasId
            );
        }

        public void Clear(Action callback)
        {
            JSRuntime.Current.InvokeAsync<string>(
                $"{JsInteropBlazorComponents}.{clear}"
            );

            callback();
        }

        public Task<string> DrawLine(string colorHex, Point point1, Point point2)
        {
            return JSRuntime.Current.InvokeAsync<string>(
                $"{JsInteropBlazorComponents}.{drawLine}"
                , colorHex
                , point1
                , point2
            );
        }
        public Task<string> DrawPolygon(string colorHex, IEnumerable<Point> points)
        {
            return JSRuntime.Current.InvokeAsync<string>(
                $"{JsInteropBlazorComponents}.{drawPolygon}"
                , colorHex
                , points
            );
        }

        #endregion

    }
}
