using System.Windows;

namespace Asteroids.Wpf
{
    public partial class App : Application
    {
        private MainWindow mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            mainWindow = new MainWindow();
            mainWindow.Show();

        }
        protected override void OnExit(ExitEventArgs e)
        {
            mainWindow.Dispose();
            base.OnExit(e);
        }
    }
}
