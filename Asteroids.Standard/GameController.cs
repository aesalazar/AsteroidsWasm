using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Helpers;
using Asteroids.Standard.Interfaces;
using Asteroids.Standard.Managers;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard
{
    /// <summary>
    /// Asteroids game engine that will calculate the lines and polygons and react to control key events.
    /// </summary>

    public class GameController : IDisposable, IGameController
    {
        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="GameController"/>.
        /// </summary>
        public GameController()
        {
            GameStatus = GameMode.Prep;
            _lastDrawn = false;
            Sounds.ActionSounds.SoundTriggered += PlaySound;

            _screenCanvas = new ScreenCanvas(new Rectangle());
            _textManager = new TextManager(_screenCanvas);
            _scoreManager = new ScoreManager(_textManager);
            _currentTitle = new TitleScreen(_textManager, _screenCanvas);
        }

        public async Task Initialize(IGraphicContainer container, Rectangle frameRectangle)
        {
            _container = container;

            GameStatus = GameMode.Title;
            ResizeGame(frameRectangle);

            await _container.Initialize(ColorHelper.DrawColorMap);

            _currentTitle.InitTitleScreen();

            SetFlipTimer();
        }

        #endregion

        #region Fields

        private const double TimerInterval = 1000 / ScreenCanvas.FramesPerSecond;

        private IGraphicContainer _container;
        private Rectangle _frameRectangle;

        private bool _lastDrawn;

        private readonly TextManager _textManager;
        private readonly ScoreManager _scoreManager;
        private readonly ScreenCanvas _screenCanvas;

        private TitleScreen _currentTitle;
        private Game _game;

        private bool _leftPressed;
        private bool _rightPressed;
        private bool _upPressed;
        private bool _hyperspaceLastPressed;
        private bool _shootingLastPressed;
        private bool _pauseLastPressed;

        private Timer _timerFlip;

        #endregion

        #region Properties

        /// <summary>
        /// Current state of the game.
        /// </summary>
        public GameMode GameStatus { get; private set; }

        /// <summary>
        /// Collection (read-only) of <see cref="ActionSounds"/> used by the game engine and associated WAV <see cref="Stream"/>s.
        /// </summary>
        public IDictionary<ActionSound, Stream> ActionSounds => Sounds.ActionSounds.SoundDictionary;

        #endregion

        #region Events

        /// <summary>
        /// Fires when the game calculation results in a sound stored in <see cref="ActionSounds"/> to be played by UI.
        /// </summary>
        public event EventHandler<ActionSound> SoundPlayed;

        #endregion

        #region Methods (public)

        /// <summary>
        /// Shutdown the game.
        /// </summary>
        public void Dispose()
        {
            // Ensure game exits when close is hit
            GameStatus = GameMode.Exit;
            _timerFlip?.Dispose();
        }

        /// <summary>
        /// Update the size of the screen.
        /// </summary>
        /// <param name="frameRectangle">New size.</param>
        public void ResizeGame(Rectangle frameRectangle)
        {
            _frameRectangle = frameRectangle;
            _screenCanvas.Size = _frameRectangle;
        }

        /// <summary>
        /// Apply a key-down event to the game.
        /// </summary>
        /// <param name="key"><see cref="PlayKey"/> to apply.</param>
        public void KeyDown(PlayKey key)
        {
            if (GameStatus == GameMode.Prep)
                return;

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
                    _scoreManager.CancelGame();
                    _currentTitle = new TitleScreen(_textManager, _screenCanvas);
                    GameStatus = GameMode.Title;
                }
            }
            else // Not Escape
            {
                // If we are in tht Title Screen, Start a game
                if (GameStatus == GameMode.Title)
                {
                    _scoreManager.ResetGame();
                    _game = new Game(_scoreManager, _textManager, _screenCanvas);
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

        /// <summary>
        /// Apply a key-up event to the game.
        /// </summary>
        /// <param name="key"><see cref="PlayKey"/> to apply.</param>
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

        private async Task Repaint()
        {
            // Only allow the canvas to be drawn once if there is an invalidate, it's ok, the other canvas will soon be drawn
            if (_lastDrawn)
                return;

            _lastDrawn = true;
            await _screenCanvas.Draw(_container);
        }

        private void TitleScreen()
        {
            _scoreManager.Draw();
            _currentTitle.DrawScreen();
        }

        private bool PlayGame()
        {
            if (_leftPressed)
                _game.Left();

            if (_rightPressed)
                _game.Right();

            _game.Thrust(_upPressed);
            _game.DrawScreen();

            // If the game is over, display the title screen
            if (_game.IsDone())
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
                        _currentTitle = new TitleScreen(_textManager, _screenCanvas);
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
