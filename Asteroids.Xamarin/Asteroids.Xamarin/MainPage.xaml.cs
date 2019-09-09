using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Asteroids.Standard;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;
using Plugin.SimpleAudioPlayer;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Asteroids.Xamarin
{
    public partial class MainPage : IDisposable
    {
        public MainPage()
        {
            InitializeComponent();

            _controller = new GameController();
            _soundPlayers = new Dictionary<ActionSound, ISimpleAudioPlayer>();
            SetupSounds();
        }

        private readonly IGameController _controller;

        private readonly IDictionary<ActionSound, ISimpleAudioPlayer> _soundPlayers;

        private void MainContentPage_SizeChanged(object sender, EventArgs e)
        {
            _controller.ResizeGame(GetRectangle());
        }

        private async void MainContentPage_LayoutChanged(object sender, EventArgs e)
        {
            MainContentPage.LayoutChanged -= MainContentPage_LayoutChanged;
            await _controller.Initialize(MainContainer1, GetRectangle());
        }

        private System.Drawing.Rectangle GetRectangle()
        {
            var density = DeviceDisplay.MainDisplayInfo.Density;
            var rec = new System.Drawing.Rectangle(
                0
                , 0
                , (int)(MainContentPage.Width * density)
                , (int)(MainContentPage.Height * density)
            );

            Debug.WriteLine(rec);
            return rec;
        }

        public void Dispose()
        {
            _controller.Dispose();
        }

        #region Keyboard Handlers

        /// <summary>
        /// Current state of the game
        /// </summary>
        public GameMode GameMode => _controller.GameStatus;

        /// <summary>
        /// Processes a game-play key down.
        /// </summary>
        /// <param name="key"></param>
        public void KeyDown(PlayKey key)
        {
            _controller.KeyDown(key);
        }

        /// <summary>
        /// Processes a game-play key up.
        /// </summary>
        /// <param name="key"></param>
        public void KeyUp(PlayKey key)
        {
            _controller.KeyUp(key);
        }

        #endregion

        #region Sound

        /// <summary>
        /// Preprocess the games sound <see cref="Stream"/>s and wire the handler.
        /// </summary>
        private void SetupSounds()
        {
            //Preprocess each sound
            foreach(var kvp in _controller.ActionSounds)
            {
                var player = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                player.Load(kvp.Value);
                _soundPlayers.Add(kvp.Key, player);
            }

            //Listen for the events
            _controller.SoundPlayed += OnSoundPlayed;
        }

        /// <summary>
        /// Playes associated <see cref="ISimpleAudioPlayer"/>.
        /// </summary>
        private void OnSoundPlayed(object sender, ActionSound e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var player = _soundPlayers[e];
                player.Play();
            }
            );
        }
            
        #endregion
    }
}
