using System;
using System.Diagnostics;
using Asteroids.Standard;
using Asteroids.Standard.Enums;
using Xamarin.Forms;

namespace Asteroids.Xamarin
{
    public partial class MainPage : ContentPage, IDisposable
    {
        public MainPage()
        {
            InitializeComponent();

            _controller = new GameController(MainContainer1);
        }

        private GameController _controller;

        private void ContentPage_SizeChanged(object sender, EventArgs e)
        {
            _controller.ResizeGame(GetRectangle());
        }

        private void ContentPage_LayoutChanged(object sender, EventArgs e)
        {
            if (_controller.GameStatus != Standard.Enums.GameMode.Prep)
                return;

            _controller.Initialize(GetRectangle());
        }

        private System.Drawing.Rectangle GetRectangle()
        {
            var rec = new System.Drawing.Rectangle(
                0
                , 0
                , (int)MainContentPage.Width * 2
                , (int)MainContentPage.Height * 2
            );

            Debug.WriteLine(rec);
            return rec;
        }

        public void Dispose()
        {
            _controller.Dispose();
        }
    }
}
