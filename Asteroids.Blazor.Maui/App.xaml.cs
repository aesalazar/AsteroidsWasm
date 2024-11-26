
namespace Asteroids.Blazor.Maui
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? _)
        {
            return new Window(new MainPage());
        }
    }
}