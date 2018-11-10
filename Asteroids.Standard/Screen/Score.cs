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
        public int CurrentScore { get; private set; }

        protected int iShips;
        protected int iHiScore;
        protected int iFreeShip;
        const int iFreeShipIncrement = 10000;

        private readonly TextDraw _textDraw;

        public Score(TextDraw textDraw, ScreenCanvas canvas) : base(canvas)
        {
            _textDraw = textDraw;
            iShips = 0;
            CurrentScore = 0;
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
            CurrentScore = 0;
            iFreeShip = iFreeShipIncrement;
        }

        public void CancelGame()
        {
            iShips = 0;
        }

        public void AddScore(int iAddScore)
        {
            CurrentScore += iAddScore;
            if (CurrentScore >= iFreeShip)
            {
                iShips += 1;
                iFreeShip += iFreeShipIncrement;
                PlaySound(this, ActionSound.Life);

            }
            if (CurrentScore >= 1000000)
                CurrentScore = CurrentScore % 1000000;
            if (CurrentScore > iHiScore)
                iHiScore = CurrentScore;
        }

        public void Draw()
        {
            const int iWriteTop = 100;
            const int iLetterWidth = 200;
            const int iLetterHeight = iLetterWidth * 2;
            String strScore;

            // Draw Score + Ships left justified
            strScore = CurrentScore.ToString("000000") + " ";
            if (iShips > 10)
            {
                strScore += "^x" + (iShips - 1);
            }
            else
            {
                for (int i = 0; i < iShips - 1; i++)
                    strScore += "^";
            }
            _textDraw.DrawText(strScore, TextDraw.Justify.LEFT,
               iWriteTop, iLetterWidth, iLetterHeight);

            // Draw HiScore Centered
            strScore = iHiScore.ToString("000000");
            _textDraw.DrawText(strScore, TextDraw.Justify.CENTER,
               iWriteTop, iLetterWidth, iLetterHeight);
        }
    }
}
