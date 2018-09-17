using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Asteroids.Standard.Interfaces
{
    /// <summary>
    /// Main graphic container to draw <see cref="IGraphicLine"/>s and <see cref="IGraphicPolygon"/>s.
    /// </summary>
    public interface IGraphicContainer
    {
        /// <summary>
        /// Initialize the container before painting starts.
        /// </summary>
        /// <param name="rectangle">Dimensions to setup with.</param>
        Task Initialize(Rectangle rectangle);

        /// <summary>
        /// Paint or repaint the canvas with the collections of lines and polygons (unfilled).
        /// </summary>
        /// <param name="lines">Collection of <see cref="IGraphicLine"/>.</param>
        /// <param name="polygons">Collection of <see cref="IGraphicPolygon"/>.</param>
        Task Draw(IEnumerable<IGraphicLine> lines, IEnumerable<IGraphicPolygon> polygons);
    }
}
