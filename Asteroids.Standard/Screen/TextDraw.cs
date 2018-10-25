using System.Drawing;
using Asteroids.Standard.Base;

namespace Asteroids.Standard.Screen
{
    /// <summary>
    /// Summary description for TextDraw.
    /// </summary>
    public class TextDraw : CommonOps
    {
        public TextDraw(ScreenCanvas canvas) : base(canvas)
        {
        }

        public enum Justify { LEFT, CENTER, RIGHT };

        public void DrawText(string strText, Justify justification, int iTopLoc, int iLetterWidth, int iLetterHeight)
        {
            int iPrintStart;

            switch (justification)
            {
                case Justify.LEFT:
                    iPrintStart = 100;
                    break;
                case Justify.CENTER:
                    iPrintStart = (int)((CanvasWidth - strText.Length * iLetterWidth) / 2.0);
                    break;
                case Justify.RIGHT:
                    iPrintStart = CanvasWidth - 100 - strText.Length * iLetterWidth;
                    break;
                default:
                    return;
            }

            var x = Canvas.Size.Width;
            var y = Canvas.Size.Height;

            for (int i = 0; i < strText.Length; i++)
                DrawLetter(strText[i],
                   (int)((iPrintStart + i * iLetterWidth) / (double)CanvasWidth * x),
                   (int)(iTopLoc / (double)CanvasHeight * y),
                   (int)(iLetterWidth / (double)CanvasWidth * x),
                   (int)(iLetterHeight / (double)CanvasHeight * y));
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
                    Canvas.AddLine(new Point(halfRight, newTop), new Point(rightSide, bottomSide));
                    Canvas.AddLineTo(new Point(pointInRight, pointInUp));
                    Canvas.AddLineTo(new Point(pointInLeft, pointInUp));
                    Canvas.AddLineTo(new Point(newLeft, bottomSide));
                    Canvas.AddLineTo(new Point(halfRight, newTop));
                    break;
                case 'O':
                case '0':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    Canvas.AddLineTo(new Point(newLeft, bottomSide));
                    Canvas.AddLineTo(new Point(newLeft, newTop));
                    break;
                case '1':
                case 'I':
                    Canvas.AddLine(new Point(halfRight, newTop), new Point(halfRight, bottomSide));
                    break;
                case '2':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    Canvas.AddLineTo(new Point(rightSide, halfDown));
                    Canvas.AddLineTo(new Point(newLeft, halfDown));
                    Canvas.AddLineTo(new Point(newLeft, bottomSide));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case '3':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    Canvas.AddLineTo(new Point(newLeft, bottomSide));
                    Canvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    break;
                case '4':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, halfDown));
                    Canvas.AddLineTo(new Point(rightSide, halfDown));
                    Canvas.AddLine(new Point(rightSide, newTop), new Point(rightSide, bottomSide));
                    break;
                case '5':
                case 'S':
                    Canvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    Canvas.AddLineTo(new Point(newLeft, halfDown));
                    Canvas.AddLineTo(new Point(rightSide, halfDown));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    Canvas.AddLineTo(new Point(newLeft, bottomSide));
                    break;
                case '6':
                    Canvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    Canvas.AddLineTo(new Point(newLeft, bottomSide));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    Canvas.AddLineTo(new Point(rightSide, halfDown));
                    Canvas.AddLineTo(new Point(newLeft, halfDown));
                    break;
                case '7':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case '8':
                case 'B':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    Canvas.AddLineTo(new Point(newLeft, bottomSide));
                    Canvas.AddLineTo(new Point(newLeft, newTop));
                    Canvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    break;
                case '9':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    Canvas.AddLineTo(new Point(newLeft, bottomSide));
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, halfDown));
                    Canvas.AddLineTo(new Point(rightSide, halfDown));
                    break;
                case 'x':
                    Canvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, bottomSide));
                    Canvas.AddLine(new Point(rightSide, halfDown), new Point(newLeft, bottomSide));
                    break;
                case 'A':
                    Canvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, halfDown));
                    Canvas.AddLineTo(new Point(halfRight, newTop));
                    Canvas.AddLineTo(new Point(rightSide, halfDown));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    Canvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    break;
                // case 'B' handled by '8'
                case 'C':
                    Canvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    Canvas.AddLineTo(new Point(newLeft, bottomSide));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case 'D':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(halfRight, newTop));
                    Canvas.AddLineTo(new Point(rightSide, halfDown));
                    Canvas.AddLineTo(new Point(halfRight, bottomSide));
                    Canvas.AddLineTo(new Point(newLeft, bottomSide));
                    Canvas.AddLineTo(new Point(newLeft, newTop));
                    break;
                case 'E':
                    Canvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    Canvas.AddLineTo(new Point(newLeft, bottomSide));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    Canvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    break;
                case 'F':
                    Canvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    Canvas.AddLineTo(new Point(newLeft, bottomSide));
                    Canvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    break;
                case 'G':
                    Canvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    Canvas.AddLineTo(new Point(newLeft, bottomSide));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    Canvas.AddLineTo(new Point(rightSide, halfDown));
                    Canvas.AddLineTo(new Point(halfRight, halfDown));
                    break;
                case 'H':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, bottomSide));
                    Canvas.AddLine(new Point(rightSide, newTop), new Point(rightSide, bottomSide));
                    Canvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    // case 'I' handled by '1'
                    break;
                case 'J':
                    Canvas.AddLine(new Point(rightSide, newTop), new Point(rightSide, bottomSide));
                    Canvas.AddLineTo(new Point(halfRight, bottomSide));
                    Canvas.AddLineTo(new Point(newLeft, halfDown));
                    break;
                case 'K':
                    Canvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, newTop));
                    Canvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, halfDown));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case 'L':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, bottomSide));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case 'M':
                    Canvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, newTop));
                    Canvas.AddLineTo(new Point(halfRight, halfDown));
                    Canvas.AddLineTo(new Point(rightSide, newTop));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case 'N':
                    Canvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, newTop));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    Canvas.AddLineTo(new Point(rightSide, newTop));
                    break;
                // case 'O' handled by '0'
                case 'P':
                    Canvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, newTop));
                    Canvas.AddLineTo(new Point(rightSide, newTop));
                    Canvas.AddLineTo(new Point(rightSide, halfDown));
                    Canvas.AddLineTo(new Point(newLeft, halfDown));
                    break;
                case 'Q':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    Canvas.AddLineTo(new Point(newLeft, bottomSide));
                    Canvas.AddLineTo(new Point(newLeft, newTop));
                    Canvas.AddLine(new Point(halfRight, halfDown), new Point(rightSide, bottomSide));
                    break;
                case 'R':
                    Canvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, newTop));
                    Canvas.AddLineTo(new Point(rightSide, newTop));
                    Canvas.AddLineTo(new Point(rightSide, halfDown));
                    Canvas.AddLineTo(new Point(newLeft, halfDown));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                // case 'S' handled by '5'
                case 'T':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    Canvas.AddLine(new Point(halfRight, newTop), new Point(halfRight, bottomSide));
                    break;
                case 'U':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, bottomSide));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    Canvas.AddLineTo(new Point(rightSide, newTop));
                    break;
                case 'V':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(halfRight, bottomSide));
                    Canvas.AddLineTo(new Point(rightSide, newTop));
                    break;
                case 'W':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, bottomSide));
                    Canvas.AddLineTo(new Point(halfRight, halfDown));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    Canvas.AddLineTo(new Point(rightSide, newTop));
                    break;
                case 'X':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, bottomSide));
                    Canvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, bottomSide));
                    break;
                case 'Y':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(halfRight, halfDown));
                    Canvas.AddLineTo(new Point(rightSide, newTop));
                    Canvas.AddLine(new Point(halfRight, halfDown), new Point(halfRight, bottomSide));
                    break;
                case 'Z':
                    Canvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    Canvas.AddLineTo(new Point(newLeft, bottomSide));
                    Canvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
            }
        }
    }
}
