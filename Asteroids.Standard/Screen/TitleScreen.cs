using System;
using Asteroids.Standard.Base;
using Asteroids.Standard.Components;

namespace Asteroids.Standard.Screen
{
    /// <summary>
    /// Summary description for CTitleScreen.
    /// </summary>
    public class TitleScreen : CommonOps
    {
        private const string instructions = "PRESS SPACE TO PLAY";
        private const int instructionSize = 200;
        private const int instructionOffset = instructionSize * 5;

        private const int titleSize = 200;
        private const int titleOffset1 = iMaxY - titleSize * 4;
        private const int titleOffset2 = iMaxY - titleSize * 2;
        private const string copyright1 = "CREATED BY HOWARD UMAN";
        private const string copyright2 = "PORTED BY ERNIE SALAZAR";

        private String strTitle;
        private int iLetterSize;
        private int iIncrement;
        private AsteroidBelt asteroids;

        private readonly TextDraw _textDraw;

        public TitleScreen(TextDraw textDraw, ScreenCanvas canvas) : base(canvas)
        {
            _textDraw = textDraw;
            InitTitleScreen();
            asteroids = new AsteroidBelt(15, canvas, Asteroid.ASTEROID_SIZE.SMALL);
        }

        public void InitTitleScreen()
        {
            iLetterSize = 40;
            iIncrement = (int)(1000 / FPS);
            strTitle = "GAME OVER";
        }

        public void DrawScreen()
        {
            //Draw instructions
            _textDraw.DrawText(
                instructions
                , TextDraw.Justify.CENTER
                , instructionOffset
                , instructionSize, instructionSize
            );

            // Flip back and forth between "Game Over" and "Asteroids"
            if ((iLetterSize > 1000) || (iLetterSize < 40))
            {
                iIncrement = -iIncrement;
                if (iLetterSize < 40)
                {
                    if (strTitle == "GAME OVER")
                        strTitle = "ASTEROIDS";
                    else
                        strTitle = "GAME OVER";
                }
            }
            iLetterSize += iIncrement;
            _textDraw.DrawText(strTitle, TextDraw.Justify.CENTER,
                              iMaxY / 2 - iLetterSize, iLetterSize, iLetterSize * 2);

            // Draw copyright notice
            _textDraw.DrawText( copyright1, TextDraw.Justify.CENTER,
                              titleOffset1, titleSize, titleSize);

            _textDraw.DrawText(copyright2, TextDraw.Justify.CENTER,
                              titleOffset2, titleSize, titleSize);

            // Draw the asteroid belt
            asteroids.Move();
            asteroids.Draw();
        }
    }
}
