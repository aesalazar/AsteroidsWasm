using System.ComponentModel;

namespace Asteroids.WinForms
{
    public sealed partial class FrmAsteroids
    {
        #region Setup

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container _components = null;

        protected override void Dispose(bool disposing)
        {
            _controller.SoundPlayed -= OnSoundPlayed;

            if (disposing)
                _components?.Dispose();


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
            this._frame1 = new Classes.GraphicPictureBox();
            ((ISupportInitialize)(this._frame1)).BeginInit();
            this.SuspendLayout();
            // 
            // _frame1
            // 
            this._frame1.BackColor = System.Drawing.SystemColors.WindowText;
            this._frame1.Dock = System.Windows.Forms.DockStyle.Fill;
            this._frame1.Location = new System.Drawing.Point(0, 0);
            this._frame1.Name = "_frame1";
            this._frame1.Size = new System.Drawing.Size(634, 461);
            this._frame1.TabIndex = 2;
            this._frame1.TabStop = false;
            // 
            // FrmAsteroids
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(12, 28);
            this.ClientSize = new System.Drawing.Size(634, 461);
            this.Controls.Add(this._frame1);
            this.Name = "FrmAsteroids";
            this.Text = "Asteroids";
            this.Activated += new System.EventHandler(this.frmAsteroids_Activated);
            this.Closed += new System.EventHandler(this.frmAsteroids_Closed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmAsteroids_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmAsteroids_KeyUp);
            this.Resize += new System.EventHandler(this.frmAsteroids_Resize);
            ((System.ComponentModel.ISupportInitialize)(this._frame1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

    }
}