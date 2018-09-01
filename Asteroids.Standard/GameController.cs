using System;
using System.Drawing;
using System.IO;
using System.Timers;
using Asteroids.Standard.Base;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard
{
    public class GameController : IDisposable
    {
        #region Constructor

        public GameController(IGraphicContainer container)
        {
            GameStatus = Modes.Prep;
            bLastDrawn = false;
            _container = container;
        }

        public GameController(IGraphicContainer container, Action<Stream> playSound)
            :this(container)
        {
            _playSound = playSound;
            CommonOps.SoundTriggered += PlaySound;
        }

        public void Initialize(Rectangle frameRectangle)
        {
            _frameRectangle = frameRectangle;
            _container.Initialize(this, frameRectangle);

            screenCanvas = new ScreenCanvas();
            score = new Score();
            GameStatus = Modes.Title;
            currTitle = new TitleScreen();
            currTitle.InitTitleScreen();

            SetFlipTimer();
        }

        #endregion

        #region Fields

        private const double timerInterval = 1000 / CommonOps.FPS;

        private IGraphicContainer _container;
        private Rectangle _frameRectangle;

        private Action<Stream> _playSound;

        private bool bLastDrawn;

        private TitleScreen currTitle;
        private Game game;
        protected Score score;
        private ScreenCanvas screenCanvas;

        private bool bLeftPressed;
        private bool bRightPressed;
        private bool bUpPressed;
        private bool bHyperspaceLastPressed;
        private bool bShootingLastPressed;
        private bool bPauseLastPressed;

        private Timer _timerFlip;

        #endregion

        #region Properties

        public Modes GameStatus { get; private set; }

        #endregion

        #region Methods (public)

        public void Dispose()
        {
            // Ensure game exits when close is hit
            GameStatus = Modes.Exit;
            _timerFlip?.Dispose();
        }

        public void ResizeGame(Rectangle frameRectangle)
        {
            _frameRectangle = frameRectangle;
            _container.SetDimensions(_frameRectangle);
        }

        public void Repaint(IGraphicContainer container)
        {
            // Only allow the canvas to be drawn once if there is an invalidate, it's ok, the other canvas will soon be drawn
            if (bLastDrawn)
                return;

            bLastDrawn = true;
            screenCanvas.Draw(container);
        }

        public void KeyDown(Keys key)
        {
            // Check escape key
            if (key == Keys.Escape) // Escape
            {
                // Escape during a title screen exits the game
                if (GameStatus == Modes.Title)
                {
                    GameStatus = Modes.Exit;
                }

                // Escape in game goes back to Title Screen
                else if (GameStatus == Modes.Game)
                {
                    score.CancelGame();
                    currTitle = new TitleScreen();
                    GameStatus = Modes.Title;
                }
            }
            else // Not Escape
            {
                // If we are in tht Title Screen, Start a game
                if (GameStatus == Modes.Title)
                {
                    score.ResetGame();
                    game = new Game();
                    GameStatus = Modes.Game;
                    bLeftPressed = false;
                    bRightPressed = false;
                    bUpPressed = false;
                    bHyperspaceLastPressed = false;
                    bShootingLastPressed = false;
                    bPauseLastPressed = false;
                }

                // Rotate Left
                else if (key == Keys.Left)
                {
                    bLeftPressed = true;
                }

                // Rotate Right
                else if (key == Keys.Right)
                {
                    bRightPressed = true;
                }

                // Thrust
                else if (key == Keys.Up)
                {
                    bUpPressed = true;
                }

                // Hyperspace (can't be held down)
                else if (!bHyperspaceLastPressed && key == Keys.Down)
                {
                    bHyperspaceLastPressed = true;
                    game.Hyperspace();
                }

                // Shooting (can't be held down)
                else if (!bShootingLastPressed && key == Keys.Space)
                {
                    bShootingLastPressed = true;
                    game.Shoot();
                }

                // Pause can't be held down)
                else if (!bPauseLastPressed && key == Keys.P)
                {
                    bPauseLastPressed = true;
                    game.Pause();
                }

            }
        }

        public void KeyUp(Keys key)
        {
            // Rotate Left
            if (key == Keys.Left)
                bLeftPressed = false;

            // Rotate Right
            else if (key == Keys.Right)
                bRightPressed = false;

            // Thrust
            else if (key == Keys.Up)
                bUpPressed = false;

            // Hyperspace - require key up before key down
            else if (key == Keys.Down)
                bHyperspaceLastPressed = false;

            // Shooting - require key up before key down
            else if (key == Keys.Space)
                bShootingLastPressed = false;

            // Pause - require key up before key down
            else if (key == Keys.P)
                bPauseLastPressed = false;
        }

        #endregion

        #region Methods (private)

        private bool TitleScreen()
        {
            score.Draw(screenCanvas, _frameRectangle.Width, _frameRectangle.Height);
            currTitle.DrawScreen(screenCanvas, _frameRectangle.Width, _frameRectangle.Height);

            return GameStatus == Modes.Title;
        }

        private bool PlayGame()
        {
            if (bLeftPressed)
                game.Left();

            if (bRightPressed)
                game.Right();

            game.Thrust(bUpPressed);
            game.DrawScreen(screenCanvas, _frameRectangle.Width, _frameRectangle.Height, ref score);

            // If the game is over, display the title screen
            if (game.Done())
                GameStatus = Modes.Title;

            return GameStatus == Modes.Game;
        }

        private void FlipDisplay(object source, ElapsedEventArgs e)
        {
            // Draw the next screen
            screenCanvas.Clear();

            switch (GameStatus)
            {
                case Modes.Title:
                    TitleScreen();
                    break;
                case Modes.Game:
                    if (!PlayGame())
                    {
                        currTitle = new TitleScreen();
                        currTitle.InitTitleScreen();
                    }
                    break;
            }

            // Flip the screen to show the updated image
            bLastDrawn = false;
            _container.Activate();
        }


        private void SetFlipTimer()
        {
            // Screen Flip Timer
            _timerFlip = new Timer(timerInterval);
            _timerFlip.Elapsed += new ElapsedEventHandler(FlipDisplay);
            _timerFlip.AutoReset = true;
            _timerFlip.Enabled = true;
        }

        private void PlaySound(object sender, Stream soundStream)
        {
            _playSound(soundStream);
        }

        #endregion

    }
}
