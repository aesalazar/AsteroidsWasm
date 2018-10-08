using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asteroids.Standard.Interfaces;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.RenderTree;

namespace Asteroids.BlazorComponents.Components
{
    /// <summary>
    /// Provides raw graphical contain for an HTML SVG element.
    /// </summary>
    public class SvgContentContainer : BlazorComponent
    {
        private const string StylePolygon = "stroke=width:1px; fill:transparent; ";

        private IEnumerable<IGraphicLine> _lastLines = new List<IGraphicLine>();
        private IEnumerable<IGraphicPolygon> _lastPolygons = new List<IGraphicPolygon>();

        //TODO: Find better way to store the element and avoid static
        /// <summary>
        /// Store the element reference
        /// </summary>
        protected override void OnAfterRender()
        {
            if (GraphicsContainerComponent.MainSvgContainer is null)
                GraphicsContainerComponent.MainSvgContainer = this;
        }

        /// <summary>
        /// Repaint the SVG content with the collections of lines and polygons (unfilled).
        /// </summary>
        /// <param name="lines">Collection of <see cref="IGraphicLine"/>.</param>
        /// <param name="polygons">Collection of <see cref="IGraphicPolygon"/>.</param>
        public void Draw(IEnumerable<IGraphicLine> lines, IEnumerable<IGraphicPolygon> polygons)
        {
            _lastLines = lines;
            _lastPolygons = polygons;
            StateHasChanged();
        }

        /// <summary>
        /// Renders the lines and polygons to the supplied <see cref="RenderTreeBuilder"/>.
        /// </summary>
        /// <param name="builder">A <see cref="RenderTreeBuilder"/> that will receive the render output.</param>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var count = 0;

            foreach (var line in _lastLines)
            {
                builder.OpenElement(count++, nameof(line));
                builder.AddAttribute(count++, "x1", line.Point1.X);
                builder.AddAttribute(count++, "y1", line.Point1.Y);
                builder.AddAttribute(count++, "x2", line.Point2.X);
                builder.AddAttribute(count++, "y2", line.Point2.Y);
                builder.AddAttribute(count++, "style", $"stroke:{line.ColorHex}");
                builder.CloseElement();
            }

            foreach (var polygon in _lastPolygons)
            {
                var points = polygon
                    .Points
                    .Aggregate(
                        new StringBuilder()
                        , (sb, p) => sb.Append($"{p.X},{p.Y} ")
                    );

                builder.OpenElement(count++, nameof(polygon));
                builder.AddAttribute(count++, nameof(points), points.ToString());
                builder.AddAttribute(count++, "style", $"stroke:{polygon.ColorHex}; {StylePolygon}");
                builder.CloseElement();
            }


            base.BuildRenderTree(builder);

        }
    }
}
