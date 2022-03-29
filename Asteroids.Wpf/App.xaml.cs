using System.Windows;

namespace Asteroids.Wpf
{
    public sealed partial class App : Application
    {
        private MainWindow _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _mainWindow = new MainWindow();
            _mainWindow.Show();

        }
        protected override void OnExit(ExitEventArgs e)
        {
            _mainWindow.Dispose();
            base.OnExit(e);
        }
    }
}
