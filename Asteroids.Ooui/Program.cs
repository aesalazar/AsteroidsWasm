using System.Threading.Tasks;
using Asteroids.Ooui.Classes;
using Asteroids.Standard;
using Ooui;

namespace Asteroids.Ooui
{
    class Program
    {
        private const int radius = 10;

        private static GraphicsContainer _container;
        private static GameController _gameController;

        static void Main(string[] args)
        {
            // Create the UI            
            _container = new GraphicsContainer();
            _container.Style.BackgroundColor = new Color(0, 0, 0, 255);

            _gameController = new GameController(_container);

            Div d = new Div();
            d.AppendChild(_container);

            UI.Port = 8082;
            UI.Publish("/", d);

            Task.Factory.StartNew(async () => 
                await _gameController.Initialize(new System.Drawing.Rectangle(0, 0, 650, 500))
            );

        }
    }
}
