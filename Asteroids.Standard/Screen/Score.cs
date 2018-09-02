using System;
using Asteroids.Standard.Base;
using Asteroids.Standard.Enums;
using static Asteroids.Standard.Sounds.ActionSounds;

namespace Asteroids.Standard.Screen
{
    /// <summary>
    /// Maintains the score information for the game
    /// </summary>
    public class Score : CommonOps
    {
        protected int iScore;
        protected int iShips;
        protected int iHiScore;
        protected int iFreeShip;
        const int iFreeShipIncrement = 10000;

        public Score()
        {
            iShips = 0;
            iScore = 0;
            iHiScore = 0;
            iFreeShip = iFreeShipIncrement;
        }

        public void GetNewShip()
        {
            iShips -= 1;
        }

        public bool HasReserveShips()
        {
            // current ship doesn't count
            return (iShips > 1);
        }

        public void ResetGame()
        {
            iShips = 3;
            iScore = 0;
            iFreeShip = iFreeShipIncrement;
        }

        public void CancelGame()
        {
            iShips = 0;
        }

        public void AddScore(int iAddScore)
        {
            iScore += iAddScore;
            if (iScore >= iFreeShip)
            {
                iShips += 1;
                iFreeShip += iFreeShipIncrement;
                PlaySound(this, ActionSound.Life);

            }
            if (iScore >= 1000000)
                iScore = iScore % 1000000;
            if (iScore > iHiScore)
                iHiScore = iScore;
        }

        public void Draw(ScreenCanvas screenCanvas, int iPictX, int iPictY)
        {
            const int iWriteTop = 100;
            const int iLetterWidth = 200;
            const int iLetterHeight = iLetterWidth * 2;
            String strScore;

            // Draw Score + Ships left justified
            strScore = iScore.ToString("000000") + " ";
            if (iShips > 10)
            {
                strScore += "^x" + (iShips - 1);
            }
            else
            {
                for (int i = 0; i < iShips - 1; i++)
                    strScore += "^";
            }
            TextDraw.DrawText(screenCanvas, strScore, TextDraw.Justify.LEFT,
               iWriteTop, iLetterWidth, iLetterHeight, iPictX, iPictY);

            // Draw HiScore Centered
            strScore = iHiScore.ToString("000000");
            TextDraw.DrawText(screenCanvas, strScore, TextDraw.Justify.CENTER,
               iWriteTop, iLetterWidth, iLetterHeight, iPictX, iPictY);
        }
    }
}
