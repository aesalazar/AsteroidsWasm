using System.Collections.Generic;
using Asteroids.Standard.Components;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Managers;

namespace Asteroids.Standard.Screen
{
    /// <summary>
    /// Splash screen drawn when not playing the game.
    /// </summary>
    internal sealed class TitleScreen
    {
        private const string Instructions = "PRESS SPACE TO PLAY";
        private const int InstructionSize = 200;
        private const int InstructionOffset = InstructionSize * 5;

        private const int TitleSize = 200;
        private const int TitleOffset1 = ScreenCanvas.CanvasHeight - TitleSize * 4;
        private const int TitleOffset2 = ScreenCanvas.CanvasHeight - TitleSize * 2;
        private const string Copyright1 = "CREATED BY HOWARD UMAN";
        private const string Copyright2 = "PORTED BY ERNIE SALAZAR";

        private string _title;
        private int _letterSize;
        private int _increment;

        private readonly TextManager _textManager;
        private readonly ScreenCanvas _canvas;
        private readonly CacheManager _cache;

        /// <summary>
        /// Creates a new instance of <see cref="TitleScreen"/>.
        /// </summary>
        /// <param name="textManager"><see cref="TextManager"/> to write text too.</param>
        /// <param name="canvas"><see cref="ScreenCanvas"/> to draw on.</param>
        public TitleScreen(TextManager textManager, ScreenCanvas canvas)
        {
            _textManager = textManager;
            _canvas = canvas;
            _title = string.Empty;

            InitTitleScreen();

            _cache = new CacheManager(
                new ScoreManager(new TextManager(_canvas))
                , null
                , new AsteroidBelt(15, Asteroid.AsteroidSize.Small)
                , new List<Bullet>()
            );
        }

        /// <summary>
        /// Resets screen with default values.
        /// </summary>
        public void InitTitleScreen()
        {
            _letterSize = 40;
            _increment = (int)(1000 / ScreenCanvas.FramesPerSecond);
            _title = "GAME OVER";
        }

        /// <summary>
        /// Draws the next screen frame.
        /// </summary>
        public void DrawScreen()
        {
            //Draw instructions
            _textManager.DrawText(
                Instructions
                , TextManager.Justify.Center
                , InstructionOffset
                , InstructionSize, InstructionSize
            );

            // Flip back and forth between "Game Over" and "Asteroids"
            if ((_letterSize > 1000) || (_letterSize < 40))
            {
                _increment = -_increment;
                if (_letterSize < 40)
                {
                    _title = _title == "GAME OVER" 
                        ? "ASTEROIDS"
                        : "GAME OVER";
                }
            }
            _letterSize += _increment;
            _textManager.DrawText(
                _title
                , TextManager.Justify.Center
                , ScreenCanvas.CanvasHeight / 2 - _letterSize
                , _letterSize
                , _letterSize * 2
            );

            // Draw copyright notice
            _textManager.DrawText(
                Copyright1
                , TextManager.Justify.Center
                , TitleOffset1
                , TitleSize
                , TitleSize
            );

            _textManager.DrawText(
                Copyright2
                , TextManager.Justify.Center
                , TitleOffset2
                , TitleSize
                , TitleSize
            );

            // Draw the asteroid belt
            _cache.Repopulate();

            foreach (var asteroid in _cache.GetAsteroids())
            {
                asteroid.ScreenObject.Move();
                _canvas.LoadPolygon(asteroid.PolygonPoints, DrawColor.White);
            }
        }
    }
}
