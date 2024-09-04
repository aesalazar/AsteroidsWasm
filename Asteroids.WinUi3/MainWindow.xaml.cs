using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Asteroids.Standard;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;
using Asteroids.WinUi3.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.System;

namespace Asteroids.WinUi3
{
    public sealed partial class MainWindow : Window
    {
        private readonly IGameController _controller;
        private readonly IDictionary<ActionSound, MediaPlayer> _soundPlayers;

        public MainWindow()
        {
            InitializeComponent();
            _controller = new GameController();
            _controller.SoundPlayed += Controller_OnSoundPlayed;
            _soundPlayers = _controller
                .ActionSounds
                .ToDictionary(
                    kvp => kvp.Key
                    , kvp => new MediaPlayer()
                    {
                        Source = MediaSource.CreateFromStream(
                            kvp.Value.ToRandomAccessStream(),
                            "audio/wav"
                        )
                    }
                );
        }

        private void Controller_OnSoundPlayed(object sender, ActionSound e)
        {
            _soundPlayers[e].Play();
        }

        private async void GraphicsContainer_Loaded(object sender, RoutedEventArgs e)
        {
            var canvas = (GraphicsContainer)sender;
            var dpi = canvas.Dpi;
            var rec = new Rectangle(0, 0, (int)(canvas.ActualWidth * dpi), (int)(canvas.ActualHeight * dpi));
            await _controller.Initialize(canvas, rec);
        }

        private void GraphicsContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var canvas = (GraphicsContainer)sender;
            var dpi = canvas.Dpi;
            var rec = new Rectangle(0, 0, (int)(e.NewSize.Width * dpi), (int)(e.NewSize.Height * dpi));
            _controller.ResizeGame(rec);
        }

        private void GraphicsContainer_KeyUp(object _, KeyRoutedEventArgs e)
        {
            PlayKey key;
            switch (e.Key)
            {
                case VirtualKey.Escape:
                    key = PlayKey.Escape;
                    break;

                case VirtualKey.Left:
                    key = PlayKey.Left;
                    break;

                case VirtualKey.Right:
                    key = PlayKey.Right;
                    break;

                case VirtualKey.Up:
                    key = PlayKey.Up;
                    break;

                case VirtualKey.Down:
                    key = PlayKey.Down;
                    break;

                case VirtualKey.Space:
                    key = PlayKey.Space;
                    break;

                case VirtualKey.P:
                    key = PlayKey.P;
                    break;

                default:
                    return;
            }

            _controller.KeyUp(key);
        }

        private void GraphicsContainer_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            PlayKey key;
            switch (e.Key)
            {
                case VirtualKey.Escape:
                    // Escape during a title screen exits the game
                    if (_controller.GameStatus == GameMode.Title)
                    {
                        _controller.Dispose();
                        Application.Current.Exit();
                        return;
                    }

                    key = PlayKey.Escape;
                    break;

                case VirtualKey.Left:
                    key = PlayKey.Left;
                    break;

                case VirtualKey.Right:
                    key = PlayKey.Right;
                    break;

                case VirtualKey.Up:
                    key = PlayKey.Up;
                    break;

                case VirtualKey.Down:
                    key = PlayKey.Down;
                    break;

                case VirtualKey.Space:
                    key = PlayKey.Space;
                    break;

                case VirtualKey.P:
                    key = PlayKey.P;
                    break;

                default:
                    return;
            }

            _controller.KeyDown(key);
        }
    }
}
