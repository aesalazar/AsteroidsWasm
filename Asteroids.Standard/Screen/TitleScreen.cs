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
        private const String strCopyright = "BY HOWARD UMAN";
        private String strTitle;
        private int iLetterSize;
        private int iIncrement;
        private AsteroidBelt asteroids;

        public TitleScreen()
        {
            InitTitleScreen();
            asteroids = new AsteroidBelt(15, Asteroid.ASTEROID_SIZE.SMALL);
        }

        public void InitTitleScreen()
        {
            iLetterSize = 40;
            iIncrement = (int)(1000 / FPS);
            strTitle = "GAME OVER";
        }

        public void DrawScreen(ScreenCanvas screenCanvas, int iPictX, int iPictY)
        {
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
            TextDraw.DrawText(screenCanvas, strTitle, TextDraw.Justify.CENTER,
                              iMaxY / 2 - iLetterSize, iLetterSize, iLetterSize * 2, iPictX, iPictY);

            // Draw copyright notice
            const int iTitleWidth = 200;
            const int iTitleHeight = iTitleWidth * 2;
            TextDraw.DrawText(screenCanvas, strCopyright, TextDraw.Justify.CENTER,
                              iMaxY - iTitleWidth * 3, iTitleWidth, iTitleHeight, iPictX, iPictY);

            // Draw the asteroid belt
            asteroids.Move();
            asteroids.Draw(screenCanvas, iPictX, iPictY);
        }
    }
}
