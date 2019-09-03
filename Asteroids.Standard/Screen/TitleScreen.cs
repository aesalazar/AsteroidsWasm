using System.Collections.Generic;
using Asteroids.Standard.Components;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Managers;

namespace Asteroids.Standard.Screen
{
    /// <summary>
    /// Splash screen drawn when not playing the game.
    /// </summary>
    public class TitleScreen
    {
        private const string instructions = "PRESS SPACE TO PLAY";
        private const int instructionSize = 200;
        private const int instructionOffset = instructionSize * 5;

        private const int titleSize = 200;
        private const int titleOffset1 = ScreenCanvas.CANVAS_HEIGHT - titleSize * 4;
        private const int titleOffset2 = ScreenCanvas.CANVAS_HEIGHT - titleSize * 2;
        private const string copyright1 = "CREATED BY HOWARD UMAN";
        private const string copyright2 = "PORTED BY ERNIE SALAZAR";

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

            InitTitleScreen();

            _cache = new CacheManager(
                new ScoreManager(new TextManager(_canvas))
                , null
                , new AsteroidBelt(15, Asteroid.ASTEROID_SIZE.SMALL)
                , new List<Bullet>()
            );
        }

        /// <summary>
        /// Resets screen with default values.
        /// </summary>
        public void InitTitleScreen()
        {
            _letterSize = 40;
            _increment = (int)(1000 / ScreenCanvas.FPS);
            _title = "GAME OVER";
        }

        /// <summary>
        /// Draws the next screen frame.
        /// </summary>
        public void DrawScreen()
        {
            //Draw instructions
            _textManager.DrawText(
                instructions
                , TextManager.Justify.CENTER
                , instructionOffset
                , instructionSize, instructionSize
            );

            // Flip back and forth between "Game Over" and "Asteroids"
            if ((_letterSize > 1000) || (_letterSize < 40))
            {
                _increment = -_increment;
                if (_letterSize < 40)
                {
                    if (_title == "GAME OVER")
                        _title = "ASTEROIDS";
                    else
                        _title = "GAME OVER";
                }
            }
            _letterSize += _increment;
            _textManager.DrawText(_title, TextManager.Justify.CENTER,
                              ScreenCanvas.CANVAS_HEIGHT / 2 - _letterSize, _letterSize, _letterSize * 2);

            // Draw copyright notice
            _textManager.DrawText(copyright1, TextManager.Justify.CENTER,
                              titleOffset1, titleSize, titleSize);

            _textManager.DrawText(copyright2, TextManager.Justify.CENTER,
                              titleOffset2, titleSize, titleSize);

            // Draw the asteroid belt
            _cache.Repopulate();

            foreach (var asteroid in _cache.Asteroids)
            {
                asteroid.ScreenObject.Move();
                _canvas.LoadPolygon(asteroid.PolygonPoints, DrawColor.White);
            }
        }
    }
}
