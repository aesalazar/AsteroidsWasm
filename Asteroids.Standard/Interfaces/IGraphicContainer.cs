using System.Collections.Generic;
using System.Drawing;

namespace Asteroids.Standard.Interfaces
{
    public interface IGraphicContainer
    {
        void Initialize(GameController controller, Rectangle rectangle);
        void SetDimensions(Rectangle rectangle);
        void Activate();

        void DrawLine(string colorHex, Point point1, Point point2);
        void DrawPolygon(string colorHex, IEnumerable<Point> points);
    }
}
