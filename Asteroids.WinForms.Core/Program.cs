using System;
using System.Windows.Forms;

namespace Asteroids.WinForms.Core
{
    internal static class Program
    {
        private static FrmAsteroids _mainForm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _mainForm = new FrmAsteroids();
            Application.Run(_mainForm);

            Application.ApplicationExit += OnApplicationExit;
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            _mainForm.Dispose();
        }
    }
}
