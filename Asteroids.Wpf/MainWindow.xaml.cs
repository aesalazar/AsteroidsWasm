using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Asteroids.Standard;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;

namespace Asteroids.Wpf
{
    public partial class MainWindow : IDisposable
    {
        private readonly IGameController _controller;
        private readonly IDictionary<ActionSound, SoundPlayer> _soundPlayers;
        private SoundPlayer _soundPlaying;

        public MainWindow()
        {
            InitializeComponent();

            _controller = new GameController();

            _soundPlayers = Standard
                .Sounds.ActionSounds.SoundDictionary
                .ToDictionary(
                    kvp => kvp.Key
                    , kvp => new SoundPlayer(kvp.Value)
                );

            foreach (var player in _soundPlayers)
                player.Value.Load();

            _controller.SoundPlayed += OnSoundPlayed;
        }

        private void OnSoundPlayed(object sender, ActionSound sound)
        {
            if (_soundPlaying != null)
                return;

            _soundPlaying = _soundPlayers[sound];

            Task.Factory.StartNew(() =>
            {
                _soundPlaying.Stream.Position = 0;
                _soundPlaying.PlaySync();
                _soundPlaying = null;
            });
        }

        private async void Window_Rendered(object sender, EventArgs e)
        {
            ContentRendered -= Window_Rendered;
            var rec = new Rectangle(0, 0, (int)MainContainer.ActualWidth, (int)MainContainer.ActualHeight);
            await _controller.Initialize(MainContainer, rec);
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
            _controller.SoundPlayed -= OnSoundPlayed;

            foreach (var player in _soundPlayers)
                player.Value.Dispose();

            _controller.Dispose();
            MainContainer.Dispose();
        }
    }
}
