using Asteroids.Standard.Enums;
using static Asteroids.Standard.Sounds.ActionSounds;

namespace Asteroids.Standard.Managers
{
    /// <summary>
    /// Maintains the score information for the game.
    /// </summary>
    internal sealed class ScoreManager
    {
        private const int FreeShipIncrement = 10000;

        private const int ScoreTop = 100;
        private const int ScoreLetterWidth = 200;
        private const int ScoreLetterHeight = ScoreLetterWidth * 2;

        private int _shipsRemaining;
        private int _highestScore;
        private int _remainderToFreeShip;

        private readonly TextManager _textDraw;

        /// <summary>
        /// Creates a new instance of <see cref="ScoreManager"/>.
        /// </summary>
        /// <param name="textDraw">Text draw to write score on.</param>
        public ScoreManager(TextManager textDraw)
        {
            _textDraw = textDraw;
            _remainderToFreeShip = FreeShipIncrement;
        }

        /// <summary>
        /// Score for the current game.
        /// </summary>
        public int CurrentScore { get; private set; }

        /// <summary>
        /// Decrease the number of ships available by 1.
        /// </summary>
        public void DecrementReserveShips()
        {
            _shipsRemaining -= 1;
        }

        /// <summary>
        /// Indicates if there are ships remaining AFTER the current one.
        /// </summary>
        /// <returns></returns>
        public bool HasReserveShips()
        {
            return _shipsRemaining > 1;
        }

        /// <summary>
        /// Sets ships and score back to defaults.
        /// </summary>
        public void ResetGame()
        {
            _shipsRemaining = 3;
            CurrentScore = 0;
            _remainderToFreeShip = FreeShipIncrement;
        }

        /// <summary>
        /// Ends the game immediately.
        /// </summary>
        public void CancelGame()
        {
            _shipsRemaining = 0;
        }

        /// <summary>
        /// Increments the current score.
        /// </summary>
        /// <param name="addScore">Amount to increase.</param>
        public void AddScore(int addScore)
        {
            if (addScore == 0)
                return;

            CurrentScore += addScore;

            if (CurrentScore >= _remainderToFreeShip)
            {
                _shipsRemaining += 1;
                _remainderToFreeShip += FreeShipIncrement;
                PlaySound(this, ActionSound.Life);

            }

            if (CurrentScore >= 1000000)
                CurrentScore = CurrentScore % 1000000;

            if (CurrentScore > _highestScore)
                _highestScore = CurrentScore;
        }

        /// <summary>
        /// Draw the current score to the <see cref="TextManager"/>.
        /// </summary>
        public void Draw()
        {
            // Draw Score + Ships left justified
            var strScore = CurrentScore.ToString("000000") + " ";

            if (_shipsRemaining > 10)
                strScore += "^x" + (_shipsRemaining - 1);
            else
                for (var i = 0; i < _shipsRemaining - 1; i++)
                    strScore += "^";

            _textDraw.DrawText(
                strScore
                , TextManager.Justify.Left
                , ScoreTop
                , ScoreLetterWidth
                , ScoreLetterHeight
            );

            // Draw HiScore Centered
            strScore = _highestScore.ToString("000000");

            _textDraw.DrawText(
                strScore
                , TextManager.Justify.Center
                , ScoreTop
                , ScoreLetterWidth
                , ScoreLetterHeight
            );
        }
    }
}
