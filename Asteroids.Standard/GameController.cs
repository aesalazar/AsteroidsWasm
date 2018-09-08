using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Timers;
using Asteroids.Standard.Base;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;
using Asteroids.Standard.Screen;
using Asteroids.Standard.Sounds;

namespace Asteroids.Standard
{
    public class GameController : IDisposable
    {
        #region Constructor

        public GameController(IGraphicContainer container)
        {
            GameStatus = GameMode.Prep;
            bLastDrawn = false;
            _container = container;
        }

        public GameController(IGraphicContainer container, Action<ActionSound> playSound)
            : this(container)
        {
            _playSound = playSound;
            ActionSounds.SoundTriggered += PlaySound;
        }

        public async Task Initialize(Rectangle frameRectangle)
        {
            _frameRectangle = frameRectangle;
            await _container.Initialize(frameRectangle);

            screenCanvas = new ScreenCanvas();
            score = new Score();
            GameStatus = GameMode.Title;
            currTitle = new TitleScreen();
            currTitle.InitTitleScreen();

            SetFlipTimer();
        }

        #endregion

        #region Fields

        private const double timerInterval = 1000 / CommonOps.FPS;

        private IGraphicContainer _container;
        private Rectangle _frameRectangle;

        private Action<ActionSound> _playSound;

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

        public GameMode GameStatus { get; private set; }

        #endregion

        #region Methods (public)

        public void Dispose()
        {
            // Ensure game exits when close is hit
            GameStatus = GameMode.Exit;
            _timerFlip?.Dispose();
        }

        public async Task ResizeGame(Rectangle frameRectangle)
        {
            _frameRectangle = frameRectangle;
            await _container.SetDimensions(_frameRectangle);
        }

        private async Task Repaint()
        {
            // Only allow the canvas to be drawn once if there is an invalidate, it's ok, the other canvas will soon be drawn
            if (bLastDrawn)
                return;

            bLastDrawn = true;
            await screenCanvas.Draw(_container);
        }

        public void KeyDown(PlayKey key)
        {
            // Check escape key
            if (key == PlayKey.Escape) // Escape
            {
                // Escape during a title screen exits the game
                if (GameStatus == GameMode.Title)
                {
                    GameStatus = GameMode.Exit;
                }

                // Escape in game goes back to Title Screen
                else if (GameStatus == GameMode.Game)
                {
                    score.CancelGame();
                    currTitle = new TitleScreen();
                    GameStatus = GameMode.Title;
                }
            }
            else // Not Escape
            {
                // If we are in tht Title Screen, Start a game
                if (GameStatus == GameMode.Title)
                {
                    score.ResetGame();
                    game = new Game();
                    GameStatus = GameMode.Game;
                    bLeftPressed = false;
                    bRightPressed = false;
                    bUpPressed = false;
                    bHyperspaceLastPressed = false;
                    bShootingLastPressed = false;
                    bPauseLastPressed = false;
                }

                // Rotate Left
                else if (key == PlayKey.Left)
                {
                    bLeftPressed = true;
                }

                // Rotate Right
                else if (key == PlayKey.Right)
                {
                    bRightPressed = true;
                }

                // Thrust
                else if (key == PlayKey.Up)
                {
                    bUpPressed = true;
                }

                // Hyperspace (can't be held down)
                else if (!bHyperspaceLastPressed && key == PlayKey.Down)
                {
                    bHyperspaceLastPressed = true;
                    game.Hyperspace();
                }

                // Shooting (can't be held down)
                else if (!bShootingLastPressed && key == PlayKey.Space)
                {
                    bShootingLastPressed = true;
                    game.Shoot();
                }

                // Pause can't be held down)
                else if (!bPauseLastPressed && key == PlayKey.P)
                {
                    bPauseLastPressed = true;
                    game.Pause();
                }

            }
        }

        public void KeyUp(PlayKey key)
        {
            // Rotate Left
            if (key == PlayKey.Left)
                bLeftPressed = false;

            // Rotate Right
            else if (key == PlayKey.Right)
                bRightPressed = false;

            // Thrust
            else if (key == PlayKey.Up)
                bUpPressed = false;

            // Hyperspace - require key up before key down
            else if (key == PlayKey.Down)
                bHyperspaceLastPressed = false;

            // Shooting - require key up before key down
            else if (key == PlayKey.Space)
                bShootingLastPressed = false;

            // Pause - require key up before key down
            else if (key == PlayKey.P)
                bPauseLastPressed = false;
        }

        #endregion

        #region Methods (private)

        private bool TitleScreen()
        {
            score.Draw(screenCanvas, _frameRectangle.Width, _frameRectangle.Height);
            currTitle.DrawScreen(screenCanvas, _frameRectangle.Width, _frameRectangle.Height);

            return GameStatus == GameMode.Title;
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
                GameStatus = GameMode.Title;

            return GameStatus == GameMode.Game;
        }

        private async Task FlipDisplay(object source, ElapsedEventArgs e)
        {
            // Draw the next screen
            screenCanvas.Clear();

            switch (GameStatus)
            {
                case GameMode.Title:
                    TitleScreen();
                    break;
                case GameMode.Game:
                    if (!PlayGame())
                    {
                        currTitle = new TitleScreen();
                        currTitle.InitTitleScreen();
                    }
                    break;
            }

            // Flip the screen to show the updated image
            bLastDrawn = false;

            try
            {
                await Repaint();
            }
            catch (Exception)
            {
                //ignore
            }
        }


        private void SetFlipTimer()
        {
            // Screen Flip Timer
            _timerFlip = new Timer(timerInterval);
            _timerFlip.Elapsed += new ElapsedEventHandler(async (s, e) => await FlipDisplay(s, e));
            _timerFlip.AutoReset = true;
            _timerFlip.Enabled = true;
        }

        private void PlaySound(object sender, ActionSound sound)
        {
            _playSound(sound);
        }

        #endregion

    }
}
