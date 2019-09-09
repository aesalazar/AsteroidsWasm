using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;

namespace Asteroids.BlazorComponents.Components
{
    /// <summary>
    /// Provides raw graphical content for an HTML SVG element.
    /// </summary>
    public class SvgContentContainer : ComponentBase
    {
        private const string StylePolygon = "stroke=width:1px; fill:transparent; ";

        private IDictionary<DrawColor, string> _colors;

        private const string AttributeLine = "line";
        private const string AttributeLineX1 = "x1";
        private const string AttributeLineY1 = "y1";
        private const string AttributeLineX2 = "x2";
        private const string AttributeLineY2 = "y2";

        private const string AttributePolygon = "polygon";
        private const string AttributePoints = "points";

        private const string AttributeStyle = "style";
        private const string AttributeStroke = "stroke";

        private IEnumerable<IGraphicLine> _lastLines = new List<IGraphicLine>();
        private IEnumerable<IGraphicPolygon> _lastPolygons = new List<IGraphicPolygon>();

        /// <summary>
        /// Initialize the SVG in anticipation of drawing.
        /// </summary>
        /// <param name="drawColorMap">Collection (read-only) of <see cref="DrawColor"/> used by the game engine and associated HEX-based (HTML) color strings.</param>
        public void Initialize(IDictionary<DrawColor, string> drawColorMap)
        {
            _colors = new ReadOnlyDictionary<DrawColor, string>(drawColorMap);
        }

        /// <summary>
        /// Repaint the SVG content with the collections of lines and polygons (unfilled).
        /// </summary>
        /// <param name="lines">Collection of <see cref="IGraphicLine"/>.</param>
        /// <param name="polygons">Collection of <see cref="IGraphicPolygon"/>.</param>
        public void Draw(IEnumerable<IGraphicLine> lines, IEnumerable<IGraphicPolygon> polygons)
        {
            //Full replace to avoid locks
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
            //Grab the refs in case multi-threading becomes an option
            var lines = _lastLines;
            var polygons = _lastPolygons;

            var count = 0;

            //Add the lines
            foreach (var line in lines)
            {
                builder.OpenElement(count++, AttributeLine);
                builder.AddAttribute(count++, AttributeLineX1, line.Point1.X);
                builder.AddAttribute(count++, AttributeLineY1, line.Point1.Y);
                builder.AddAttribute(count++, AttributeLineX2, line.Point2.X);
                builder.AddAttribute(count++, AttributeLineY2, line.Point2.Y);
                builder.AddAttribute(count++, AttributeStyle, $"{AttributeStroke}:{_colors[line.Color]}");
                builder.CloseElement();
            }

            //add the polygons
            foreach (var polygon in polygons)
            {
                var points = polygon
                    .Points
                    .Aggregate(
                        new StringBuilder()
                        , (sb, p) => sb.Append($"{p.X},{p.Y} ")
                    );

                builder.OpenElement(count++, AttributePolygon);
                builder.AddAttribute(count++, AttributePoints, points.ToString());
                builder.AddAttribute(count++, AttributeStyle, $"{AttributeStroke}:{_colors[polygon.Color]}; {StylePolygon}");
                builder.CloseElement();
            }

            base.BuildRenderTree(builder);

        }
    }
}
