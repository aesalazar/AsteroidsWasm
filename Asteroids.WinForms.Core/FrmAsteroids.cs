using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using Asteroids.Standard;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;

namespace Asteroids.WinForms.Core
{
    public partial class FrmAsteroids : Form
    {
        private Classes.GraphicPictureBox _frame1;

        private readonly IGameController _controller;
        private readonly IDictionary<ActionSound, SoundPlayer> _soundPlayers;
        private SoundPlayer _soundPlaying;

        public FrmAsteroids()
        {
            InitializeComponent();

            _controller = new GameController();
            _controller.SoundPlayed += OnSoundPlayed;

            _soundPlayers = Standard
                .Sounds.ActionSounds.SoundDictionary
                .ToDictionary(
                    kvp => kvp.Key
                    , kvp => new SoundPlayer(kvp.Value)
                );

            foreach (var player in _soundPlayers)
                player.Value.Load();
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

        private void frmAsteroids_Closed(object sender, EventArgs e)
        {
            _controller.Dispose();
        }

        private void frmAsteroids_Resize(object sender, EventArgs e)
        {
            var rec = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            _controller.ResizeGame(rec);
        }

        private async void frmAsteroids_Activated(object sender, EventArgs e)
        {
            Activated -= frmAsteroids_Activated;
            var rec = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            await _controller.Initialize(_frame1, rec);
        }

        private void frmAsteroids_KeyDown(object sender, KeyEventArgs e)
        {
            PlayKey key;
            switch (e.KeyData)
            {
                case Keys.Escape:
                    // Escape during a title screen exits the game
                    if (_controller.GameStatus == GameMode.Title)
                    {
                        Application.Exit();
                        return;
                    }

                    key = PlayKey.Escape;
                    break;

                case Keys.Left:
                    key = PlayKey.Left;
                    break;

                case Keys.Right:
                    key = PlayKey.Right;
                    break;

                case Keys.Up:
                    key = PlayKey.Up;
                    break;

                case Keys.Down:
                    key = PlayKey.Down;
                    break;

                case Keys.Space:
                    key = PlayKey.Space;
                    break;

                case Keys.P:
                    key = PlayKey.P;
                    break;

                default:
                    return;
            }

            _controller.KeyDown(key);
        }

        private void frmAsteroids_KeyUp(object sender, KeyEventArgs e)
        {
            PlayKey key;
            switch (e.KeyData)
            {
                case Keys.Escape:
                    key = PlayKey.Escape;
                    break;

                case Keys.Left:
                    key = PlayKey.Left;
                    break;

                case Keys.Right:
                    key = PlayKey.Right;
                    break;

                case Keys.Up:
                    key = PlayKey.Up;
                    break;

                case Keys.Down:
                    key = PlayKey.Down;
                    break;

                case Keys.Space:
                    key = PlayKey.Space;
                    break;

                case Keys.P:
                    key = PlayKey.P;
                    break;

                default:
                    return;
            }

            _controller.KeyUp(key);
        }

    }
}
