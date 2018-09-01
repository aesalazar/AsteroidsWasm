using System;
using System.Windows.Forms;

namespace Asteroids.WinForms
{
    static class Program
    {
        private static frmAsteroids _mainForm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _mainForm = new frmAsteroids();
            Application.Run(_mainForm);

            Application.ApplicationExit += OnApplicationExit;
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            _mainForm.Dispose();
        }
    }
}
