using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Asteroids.Standard;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using LibVLCSharp.Shared;

namespace Asteroids.AvaloniaUi
{
    public partial class MainWindow : Window, IDisposable
    {
        private readonly IGameController _controller;

        private readonly LibVLC _libVlc;
        private readonly MediaPlayer _mediaPlayer;

        private readonly IDictionary<ActionSound, Media> _soundPlayers;
        private bool _isSoundPlaying;

        public MainWindow()
        {
            InitializeComponent();
            _controller = new GameController();

            _libVlc = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVlc);
            _mediaPlayer.EndReached += MediaPlayer_EndReached;
            _soundPlayers = _controller.ActionSounds.ToDictionary(
                kvp => kvp.Key,
                kvp => new Media(_libVlc, new StreamMediaInput(kvp.Value)));

            _controller.SoundPlayed += Controller_SoundPlayed;
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            var rec = new Rectangle(0, 0, (int)MainContainer.Bounds.Width, (int)MainContainer.Bounds.Height);
            _ = _controller.Initialize(MainContainer, rec);
        }

        private void MediaPlayer_EndReached(object? _, EventArgs __)
        {
            _isSoundPlaying = false;
        }

        private void Controller_SoundPlayed(object? _, ActionSound sound)
        {
            if (_isSoundPlaying)
                return;

            _isSoundPlaying = true;
            var media = _soundPlayers[sound];
            _mediaPlayer.Media = media;

            Task.Factory.StartNew(() =>
            {
                _mediaPlayer.Play();
            });
        }

        private void Window_SizeChanged(object? _, SizeChangedEventArgs e)
        {
            _controller.ResizeGame(new Rectangle(0, 0, (int)e.NewSize.Width, (int)e.NewSize.Height));
        }

        private void Window_KeyDown(object? _, KeyEventArgs e)
        {
            PlayKey key;
            switch (e.Key)
            {
                case Key.Escape:
                    // Escape during a title screen exits the game
                    if (_controller.GameStatus == GameMode.Title)
                    {
                        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                            lifetime.Shutdown();
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

        private void Window_KeyUp(object? _, KeyEventArgs e)
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
            _controller.SoundPlayed -= Controller_SoundPlayed;
            _controller.Dispose();

            foreach (var player in _soundPlayers)
                player.Value.Dispose();

            _mediaPlayer.EndReached -= MediaPlayer_EndReached;
            _mediaPlayer.Dispose();
            _libVlc.Dispose();
        }
    }
}