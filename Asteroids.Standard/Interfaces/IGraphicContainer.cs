using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Asteroids.Standard.Interfaces
{
    public interface IGraphicContainer
    {
        Task Initialize(GameController controller, Rectangle rectangle);
        Task SetDimensions(Rectangle rectangle);
        Task Activate();

        Task DrawLine(string colorHex, Point point1, Point point2);
        Task DrawPolygon(string colorHex, IEnumerable<Point> points);
    }
}
