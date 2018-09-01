using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Input;
using Asteroids.Standard;

namespace Asteroids.Wpf
{
    public partial class MainWindow : Window, IDisposable
    {
        public MainWindow()
        {
            InitializeComponent();

            _controller = new GameController(MainContainer, PlaySound);
        }

        private GameController _controller;

        private void PlaySound(Stream stream)
        {
            using (var player = new SoundPlayer(stream))
                player.Play();
        }

        private void Window_Activated(object sender, System.EventArgs e)
        {
            if (_controller.GameStatus != Standard.Enums.Modes.Prep)
                return;

            var rec = new Rectangle(0, 0, (int)MainContainer.ActualWidth, (int)MainContainer.ActualHeight);
            _controller.Initialize(rec);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _controller.ResizeGame(new Rectangle(0, 0, (int)e.NewSize.Width, (int)e.NewSize.Height));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Standard.Enums.Keys key;
            switch (e.Key)
            {
                case Key.Escape:
                    // Escape during a title screen exits the game
                    if (_controller.GameStatus == Standard.Enums.Modes.Title)
                    {
                        Application.Current.Shutdown();
                        return;
                    }

                    key = Standard.Enums.Keys.Escape;
                    break;

                case Key.Left:
                    key = Standard.Enums.Keys.Left;
                    break;

                case Key.Right:
                    key = Standard.Enums.Keys.Right;
                    break;

                case Key.Up:
                    key = Standard.Enums.Keys.Up;
                    break;

                case Key.Down:
                    key = Standard.Enums.Keys.Down;
                    break;

                case Key.Space:
                    key = Standard.Enums.Keys.Space;
                    break;

                case Key.P:
                    key = Standard.Enums.Keys.P;
                    break;

                default:
                    return;
            }

            _controller.KeyDown(key);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            Standard.Enums.Keys key;
            switch (e.Key)
            {
                case Key.Escape:
                    key = Standard.Enums.Keys.Escape;
                    break;

                case Key.Left:
                    key = Standard.Enums.Keys.Left;
                    break;

                case Key.Right:
                    key = Standard.Enums.Keys.Right;
                    break;

                case Key.Up:
                    key = Standard.Enums.Keys.Up;
                    break;

                case Key.Down:
                    key = Standard.Enums.Keys.Down;
                    break;

                case Key.Space:
                    key = Standard.Enums.Keys.Space;
                    break;

                case Key.P:
                    key = Standard.Enums.Keys.P;
                    break;

                default:
                    return;
            }

            _controller.KeyUp(key);
        }

        public void Dispose()
        {
            _controller.Dispose();
        }
    }
}
