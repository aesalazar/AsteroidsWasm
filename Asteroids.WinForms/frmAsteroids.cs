using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using Asteroids.Standard;
using Asteroids.Standard.Enums;

namespace Asteroids
{
    public class frmAsteroids : Form, IDisposable
    {
        private WinForms.Classes.GraphicPictureBox frame1;

        private readonly GameController _controller;
        private readonly IDictionary<ActionSound, SoundPlayer> _soundPlayers;
        private SoundPlayer _soundPlaying;

        public frmAsteroids()
        {
            InitializeComponent();

            _controller = new GameController(frame1, PlaySound);

            _soundPlayers = Standard
                .Sounds.ActionSounds.SoundDictionary
                .ToDictionary(
                    kvp => kvp.Key
                    , kvp => new SoundPlayer(kvp.Value)
                );

            foreach (var player in _soundPlayers)
                player.Value.Load();
        }

        private void PlaySound(ActionSound sound)
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

        private async void frmAsteroids_Resize(object sender, EventArgs e)
        {
            var rec = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            await _controller.ResizeGame(rec);
        }

        private async void frmAsteroids_Activated(object sender, EventArgs e)
        {
            Activated -= new EventHandler(frmAsteroids_Activated);
            var rec = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            await _controller.Initialize(rec);
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

        #region Setup

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();


            foreach (var player in _soundPlayers)
                player.Value.Dispose();

            _controller.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.frame1 = new Asteroids.WinForms.Classes.GraphicPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.frame1)).BeginInit();
            this.SuspendLayout();
            // 
            // frame1
            // 
            this.frame1.BackColor = System.Drawing.SystemColors.WindowText;
            this.frame1.Location = new System.Drawing.Point(189, 7);
            this.frame1.Name = "frame1";
            this.frame1.Size = new System.Drawing.Size(100, 50);
            this.frame1.TabIndex = 2;
            this.frame1.TabStop = false;
            // 
            // frmAsteroids
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(634, 461);
            this.Controls.Add(this.frame1);
            this.Name = "frmAsteroids";
            this.Text = "Asteroids";
            this.Activated += new System.EventHandler(this.frmAsteroids_Activated);
            this.Closed += new System.EventHandler(this.frmAsteroids_Closed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmAsteroids_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmAsteroids_KeyUp);
            this.Resize += new System.EventHandler(this.frmAsteroids_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.frame1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
