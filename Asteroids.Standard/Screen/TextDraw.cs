using System.Drawing;
using Asteroids.Standard.Base;

namespace Asteroids.Standard.Screen
{
    /// <summary>
    /// Summary description for TextDraw.
    /// </summary>
    public class TextDraw : CommonOps
    {
        private readonly ScreenCanvas _screenCanvas;

        public TextDraw(ScreenCanvas canvas) : base()
        {
            _screenCanvas = canvas;
        }

        public enum Justify { LEFT, CENTER, RIGHT };

        public void DrawText(string strText, Justify justification, int iTopLoc, int iLetterWidth, int iLetterHeight)
        {
            int iPrintStart;
            var width = ScreenCanvas.CANVAS_WIDTH;
            var height = ScreenCanvas.CANVAS_HEIGHT;

            switch (justification)
            {
                case Justify.LEFT:
                    iPrintStart = 100;
                    break;
                case Justify.CENTER:
                    iPrintStart = (int)((width - strText.Length * iLetterWidth) / 2.0);
                    break;
                case Justify.RIGHT:
                    iPrintStart = height - 100 - strText.Length * iLetterWidth;
                    break;
                default:
                    return;
            }

            var x = _screenCanvas.Size.Width;
            var y = _screenCanvas.Size.Height;

            for (int i = 0; i < strText.Length; i++)
                DrawLetter(strText[i],
                   (int)((iPrintStart + i * iLetterWidth) / (double)width * x),
                   (int)(iTopLoc / (double)height * y),
                   (int)(iLetterWidth / (double)width * x),
                   (int)(iLetterHeight / (double)height * y));
        }

        private void DrawLetter(char chDraw, int letterLeft, int letterTop, int letterWidth, int letterHeight)
        {
            int newLeft = (int)(letterLeft + letterWidth * .2);
            int newTop = (int)(letterTop + letterHeight * .1);
            int halfRight = (newLeft + letterLeft + letterWidth) / 2;
            int halfDown = (newTop + letterTop + letterHeight) / 2;
            int rightSide = letterLeft + letterWidth;
            int bottomSide = letterTop + letterHeight;

            switch (chDraw)
            {
                case '^':/* Ship */
                    int pointInUp = (int)(bottomSide - letterHeight * .2);
                    int pointInLeft = (int)(newLeft + letterWidth * .25);
                    int pointInRight = (int)(rightSide - letterWidth * .25);
                    _screenCanvas.AddLine(new Point(halfRight, newTop), new Point(rightSide, bottomSide));
                    _screenCanvas.AddLineTo(new Point(pointInRight, pointInUp));
                    _screenCanvas.AddLineTo(new Point(pointInLeft, pointInUp));
                    _screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    _screenCanvas.AddLineTo(new Point(halfRight, newTop));
                    break;
                case 'O':
                case '0':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    _screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    _screenCanvas.AddLineTo(new Point(newLeft, newTop));
                    break;
                case '1':
                case 'I':
                    _screenCanvas.AddLine(new Point(halfRight, newTop), new Point(halfRight, bottomSide));
                    break;
                case '2':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    _screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    _screenCanvas.AddLineTo(new Point(newLeft, halfDown));
                    _screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case '3':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    _screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    _screenCanvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    break;
                case '4':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, halfDown));
                    _screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    _screenCanvas.AddLine(new Point(rightSide, newTop), new Point(rightSide, bottomSide));
                    break;
                case '5':
                case 'S':
                    _screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    _screenCanvas.AddLineTo(new Point(newLeft, halfDown));
                    _screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    _screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    break;
                case '6':
                    _screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    _screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    _screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    _screenCanvas.AddLineTo(new Point(newLeft, halfDown));
                    break;
                case '7':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case '8':
                case 'B':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    _screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    _screenCanvas.AddLineTo(new Point(newLeft, newTop));
                    _screenCanvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    break;
                case '9':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    _screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, halfDown));
                    _screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    break;
                case 'x':
                    _screenCanvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, bottomSide));
                    _screenCanvas.AddLine(new Point(rightSide, halfDown), new Point(newLeft, bottomSide));
                    break;
                case 'A':
                    _screenCanvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, halfDown));
                    _screenCanvas.AddLineTo(new Point(halfRight, newTop));
                    _screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    _screenCanvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    break;
                // case 'B' handled by '8'
                case 'C':
                    _screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    _screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case 'D':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(halfRight, newTop));
                    _screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    _screenCanvas.AddLineTo(new Point(halfRight, bottomSide));
                    _screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    _screenCanvas.AddLineTo(new Point(newLeft, newTop));
                    break;
                case 'E':
                    _screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    _screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    _screenCanvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    break;
                case 'F':
                    _screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    _screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    _screenCanvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    break;
                case 'G':
                    _screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    _screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    _screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    _screenCanvas.AddLineTo(new Point(halfRight, halfDown));
                    break;
                case 'H':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, bottomSide));
                    _screenCanvas.AddLine(new Point(rightSide, newTop), new Point(rightSide, bottomSide));
                    _screenCanvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    // case 'I' handled by '1'
                    break;
                case 'J':
                    _screenCanvas.AddLine(new Point(rightSide, newTop), new Point(rightSide, bottomSide));
                    _screenCanvas.AddLineTo(new Point(halfRight, bottomSide));
                    _screenCanvas.AddLineTo(new Point(newLeft, halfDown));
                    break;
                case 'K':
                    _screenCanvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, newTop));
                    _screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, halfDown));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case 'L':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, bottomSide));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case 'M':
                    _screenCanvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, newTop));
                    _screenCanvas.AddLineTo(new Point(halfRight, halfDown));
                    _screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case 'N':
                    _screenCanvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, newTop));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    _screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    break;
                // case 'O' handled by '0'
                case 'P':
                    _screenCanvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, newTop));
                    _screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    _screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    _screenCanvas.AddLineTo(new Point(newLeft, halfDown));
                    break;
                case 'Q':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    _screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    _screenCanvas.AddLineTo(new Point(newLeft, newTop));
                    _screenCanvas.AddLine(new Point(halfRight, halfDown), new Point(rightSide, bottomSide));
                    break;
                case 'R':
                    _screenCanvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, newTop));
                    _screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    _screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    _screenCanvas.AddLineTo(new Point(newLeft, halfDown));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                // case 'S' handled by '5'
                case 'T':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    _screenCanvas.AddLine(new Point(halfRight, newTop), new Point(halfRight, bottomSide));
                    break;
                case 'U':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, bottomSide));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    _screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    break;
                case 'V':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(halfRight, bottomSide));
                    _screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    break;
                case 'W':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, bottomSide));
                    _screenCanvas.AddLineTo(new Point(halfRight, halfDown));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    _screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    break;
                case 'X':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, bottomSide));
                    _screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, bottomSide));
                    break;
                case 'Y':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(halfRight, halfDown));
                    _screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    _screenCanvas.AddLine(new Point(halfRight, halfDown), new Point(halfRight, bottomSide));
                    break;
                case 'Z':
                    _screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    _screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    _screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
            }
        }
    }
}
