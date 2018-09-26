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
    public class GameController : IDisposable, IGameController
    {
        #region Constructor

        public GameController()
        {
            GameStatus = GameMode.Prep;
            _lastDrawn = false;
            ActionSounds.SoundTriggered += PlaySound;

        }

        public async Task Initialize(IGraphicContainer container, Rectangle frameRectangle)
        {
            _container = container;
            _frameRectangle = frameRectangle;
            await _container.Initialize();

            _screenCanvas = new ScreenCanvas();
            _score = new Score();
            GameStatus = GameMode.Title;
            _currentTitle = new TitleScreen();
            _currentTitle.InitTitleScreen();

            SetFlipTimer();
        }

        #endregion

        #region Fields

        private const double TimerInterval = 1000 / CommonOps.FPS;

        private IGraphicContainer _container;
        private Rectangle _frameRectangle;

        private bool _lastDrawn;

        private TitleScreen _currentTitle;
        private Game _game;
        private Score _score;
        private ScreenCanvas _screenCanvas;

        private bool _leftPressed;
        private bool _rightPressed;
        private bool _upPressed;
        private bool _hyperspaceLastPressed;
        private bool _shootingLastPressed;
        private bool _pauseLastPressed;

        private Timer _timerFlip;

        #endregion

        #region Properties

        public GameMode GameStatus { get; private set; }

        #endregion

        #region Events

        public event EventHandler<ActionSound> SoundPlayed;

        #endregion

        #region Methods (public)

        public void Dispose()
        {
            // Ensure game exits when close is hit
            GameStatus = GameMode.Exit;
            _timerFlip?.Dispose();
        }

        public void ResizeGame(Rectangle frameRectangle)
        {
            _frameRectangle = frameRectangle;
        }

        private async Task Repaint()
        {
            // Only allow the canvas to be drawn once if there is an invalidate, it's ok, the other canvas will soon be drawn
            if (_lastDrawn)
                return;

            _lastDrawn = true;
            await _screenCanvas.Draw(_container);
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
                    _score.CancelGame();
                    _currentTitle = new TitleScreen();
                    GameStatus = GameMode.Title;
                }
            }
            else // Not Escape
            {
                // If we are in tht Title Screen, Start a game
                if (GameStatus == GameMode.Title)
                {
                    _score.ResetGame();
                    _game = new Game();
                    GameStatus = GameMode.Game;
                    _leftPressed = false;
                    _rightPressed = false;
                    _upPressed = false;
                    _hyperspaceLastPressed = false;
                    _shootingLastPressed = false;
                    _pauseLastPressed = false;
                }

                // Rotate Left
                else if (key == PlayKey.Left)
                {
                    _leftPressed = true;
                }

                // Rotate Right
                else if (key == PlayKey.Right)
                {
                    _rightPressed = true;
                }

                // Thrust
                else if (key == PlayKey.Up)
                {
                    _upPressed = true;
                }

                // Hyperspace (can't be held down)
                else if (!_hyperspaceLastPressed && key == PlayKey.Down)
                {
                    _hyperspaceLastPressed = true;
                    _game.Hyperspace();
                }

                // Shooting (can't be held down)
                else if (!_shootingLastPressed && key == PlayKey.Space)
                {
                    _shootingLastPressed = true;
                    _game.Shoot();
                }

                // Pause can't be held down)
                else if (!_pauseLastPressed && key == PlayKey.P)
                {
                    _pauseLastPressed = true;
                    _game.Pause();
                }

            }
        }

        public void KeyUp(PlayKey key)
        {
            // Rotate Left
            if (key == PlayKey.Left)
                _leftPressed = false;

            // Rotate Right
            else if (key == PlayKey.Right)
                _rightPressed = false;

            // Thrust
            else if (key == PlayKey.Up)
                _upPressed = false;

            // Hyperspace - require key up before key down
            else if (key == PlayKey.Down)
                _hyperspaceLastPressed = false;

            // Shooting - require key up before key down
            else if (key == PlayKey.Space)
                _shootingLastPressed = false;

            // Pause - require key up before key down
            else if (key == PlayKey.P)
                _pauseLastPressed = false;
        }

        #endregion

        #region Methods (private)

        private void TitleScreen()
        {
            _score.Draw(_screenCanvas, _frameRectangle.Width, _frameRectangle.Height);
            _currentTitle.DrawScreen(_screenCanvas, _frameRectangle.Width, _frameRectangle.Height);
        }

        private bool PlayGame()
        {
            if (_leftPressed)
                _game.Left();

            if (_rightPressed)
                _game.Right();

            _game.Thrust(_upPressed);
            _game.DrawScreen(_screenCanvas, _frameRectangle.Width, _frameRectangle.Height, ref _score);

            // If the game is over, display the title screen
            if (_game.Done())
                GameStatus = GameMode.Title;

            return GameStatus == GameMode.Game;
        }

        private async Task FlipDisplay()
        {
            // Draw the next screen
            _screenCanvas.Clear();

            switch (GameStatus)
            {
                case GameMode.Title:
                    TitleScreen();
                    break;
                case GameMode.Game:
                    if (!PlayGame())
                    {
                        _currentTitle = new TitleScreen();
                        _currentTitle.InitTitleScreen();
                    }
                    break;
            }

            // Flip the screen to show the updated image
            _lastDrawn = false;

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
            _timerFlip = new Timer(TimerInterval);
            _timerFlip.Elapsed += async (s, e) => await FlipDisplay();
            _timerFlip.AutoReset = true;
            _timerFlip.Enabled = true;
        }

        private void PlaySound(object sender, ActionSound sound)
        {
            SoundPlayed?.Invoke(sender, sound);
        }

        #endregion

    }
}
