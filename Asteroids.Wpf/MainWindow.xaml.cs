using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Input;
using Asteroids.Standard;
using Asteroids.Standard.Enums;

namespace Asteroids.Wpf
{
    public partial class MainWindow : Window, IDisposable
    {
        public MainWindow()
        {
            InitializeComponent();

            _controller = new GameController(MainContainer, PlaySound);

            _soundPlayers = Standard
                .Sounds.ActionSounds.SoundDictionary
                .ToDictionary(
                    kvp => kvp.Key
                    , kvp => new SoundPlayer(kvp.Value)
                );

            foreach (var player in _soundPlayers)
                player.Value.Load();
        }

        private GameController _controller;
        private IDictionary<ActionSound, SoundPlayer> _soundPlayers;

        private void PlaySound(ActionSound sound)
        {
            var player = _soundPlayers[sound];
            player.Play();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (_controller.GameStatus != GameMode.Prep)
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
            PlayKey key;
            switch (e.Key)
            {
                case Key.Escape:
                    // Escape during a title screen exits the game
                    if (_controller.GameStatus == GameMode.Title)
                    {
                        Application.Current.Shutdown();
                        return;
                    }

                    key = PlayKey.Escape;
                    break;

                case Key.Left:
                    key = PlayKey.Left;
                    break;

                case Key.Right:
                    key = PlayKey.Right;
                    break;

                case Key.Up:
                    key = PlayKey.Up;
                    break;

                case Key.Down:
                    key = PlayKey.Down;
                    break;

                case Key.Space:
                    key = PlayKey.Space;
                    break;

                case Key.P:
                    key = PlayKey.P;
                    break;

                default:
                    return;
            }

            _controller.KeyDown(key);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            PlayKey key;
            switch (e.Key)
            {
                case Key.Escape:
                    key = PlayKey.Escape;
                    break;

                case Key.Left:
                    key = PlayKey.Left;
                    break;

                case Key.Right:
                    key = PlayKey.Right;
                    break;

                case Key.Up:
                    key = PlayKey.Up;
                    break;

                case Key.Down:
                    key = PlayKey.Down;
                    break;

                case Key.Space:
                    key = PlayKey.Space;
                    break;

                case Key.P:
                    key = PlayKey.P;
                    break;

                default:
                    return;
            }

            _controller.KeyUp(key);
        }

        public void Dispose()
        {
            foreach (var player in _soundPlayers)
                player.Value.Dispose();

            _controller.Dispose();
        }
    }
}
