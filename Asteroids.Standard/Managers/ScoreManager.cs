using System;
using Asteroids.Standard.Base;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Screen;
using static Asteroids.Standard.Sounds.ActionSounds;

namespace Asteroids.Standard.Managers
{
    /// <summary>
    /// Maintains the score information for the game
    /// </summary>
    public class ScoreManager : CommonOps
    {
        public int CurrentScore { get; private set; }

        protected int iShips;
        protected int iHiScore;
        protected int iFreeShip;
        private const int iFreeShipIncrement = 10000;

        private readonly TextDraw _textDraw;

        /// <summary>
        /// Creates a new instance of <see cref="ScoreManager"/>.
        /// </summary>
        /// <param name="textDraw"Text draw to write score on.</param>
        public ScoreManager(TextDraw textDraw) : base()
        {
            _textDraw = textDraw;
            iShips = 0;
            CurrentScore = 0;
            iHiScore = 0;
            iFreeShip = iFreeShipIncrement;
        }

        public void DecrementReserveShips()
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
            if (iAddScore == 0)
                return;

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
