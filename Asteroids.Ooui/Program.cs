using System.Threading.Tasks;
using Asteroids.Ooui.Classes;
using Asteroids.Standard;
using Asteroids.Standard.Interfaces;
using Ooui;

namespace Asteroids.Ooui
{
    internal class Program
    {
        private static GraphicsContainer _container;
        private static IGameController _gameController;

        private static void Main(string[] args)
        {
            // Create the UI            
            _container = new GraphicsContainer
            {
                Style = {BackgroundColor = new Color(0, 0, 0, 255)}
            };

            _gameController = new GameController(_container);

            var d = new Div();
            d.AppendChild(_container);

            UI.Port = 8082;
            UI.Publish("/", d);

            Task.Factory.StartNew(async () => 
                await _gameController.Initialize(new System.Drawing.Rectangle(0, 0, 650, 500))
            );

        }
    }
}
