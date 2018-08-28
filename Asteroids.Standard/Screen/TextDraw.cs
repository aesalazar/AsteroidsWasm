using System.Drawing;
using Asteroids.Standard.Base;

namespace Asteroids.Standard.Screen
{
    /// <summary>
    /// Summary description for TextDraw.
    /// </summary>
    public class TextDraw : CommonOps
    {
        protected ScreenCanvas screenCanvas;
        public TextDraw()
        {
        }

        public enum Justify { LEFT, CENTER, RIGHT };

        public static void DrawText(ScreenCanvas screenCanvas, string strText,
                                         Justify justification, int iTopLoc,
                                         int iLetterWidth, int iLetterHeight,
                                         int iPictX, int iPictY)
        {
            int iPrintStart;

            switch (justification)
            {
                case Justify.LEFT:
                    iPrintStart = 100;
                    break;
                case Justify.CENTER:
                    iPrintStart = (int)((iMaxX - strText.Length * iLetterWidth) / 2.0);
                    break;
                case Justify.RIGHT:
                    iPrintStart = iMaxX - 100 - strText.Length * iLetterWidth;
                    break;
                default:
                    return;
            }

            for (int i = 0; i < strText.Length; i++)
                TextDraw.DrawLetter(screenCanvas, strText[i],
                   (int)((iPrintStart + i * iLetterWidth) / (double)iMaxX * iPictX),
                   (int)(iTopLoc / (double)iMaxY * iPictY),
                   (int)(iLetterWidth / (double)iMaxX * iPictX),
                   (int)(iLetterHeight / (double)iMaxY * iPictY));
        }

        static private void DrawLetter(ScreenCanvas screenCanvas, char chDraw,
                                      int letterLeft, int letterTop, int letterWidth, int letterHeight)
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
                    screenCanvas.AddLine(new Point(halfRight, newTop), new Point(rightSide, bottomSide));
                    screenCanvas.AddLineTo(new Point(pointInRight, pointInUp));
                    screenCanvas.AddLineTo(new Point(pointInLeft, pointInUp));
                    screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    screenCanvas.AddLineTo(new Point(halfRight, newTop));
                    break;
                case 'O':
                case '0':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    screenCanvas.AddLineTo(new Point(newLeft, newTop));
                    break;
                case '1':
                case 'I':
                    screenCanvas.AddLine(new Point(halfRight, newTop), new Point(halfRight, bottomSide));
                    break;
                case '2':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    screenCanvas.AddLineTo(new Point(newLeft, halfDown));
                    screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case '3':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    screenCanvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    break;
                case '4':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, halfDown));
                    screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    screenCanvas.AddLine(new Point(rightSide, newTop), new Point(rightSide, bottomSide));
                    break;
                case '5':
                case 'S':
                    screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    screenCanvas.AddLineTo(new Point(newLeft, halfDown));
                    screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    break;
                case '6':
                    screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    screenCanvas.AddLineTo(new Point(newLeft, halfDown));
                    break;
                case '7':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case '8':
                case 'B':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    screenCanvas.AddLineTo(new Point(newLeft, newTop));
                    screenCanvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    break;
                case '9':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, halfDown));
                    screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    break;
                case 'x':
                    screenCanvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, bottomSide));
                    screenCanvas.AddLine(new Point(rightSide, halfDown), new Point(newLeft, bottomSide));
                    break;
                case 'A':
                    screenCanvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, halfDown));
                    screenCanvas.AddLineTo(new Point(halfRight, newTop));
                    screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    screenCanvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    break;
                // case 'B' handled by '8'
                case 'C':
                    screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case 'D':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(halfRight, newTop));
                    screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    screenCanvas.AddLineTo(new Point(halfRight, bottomSide));
                    screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    screenCanvas.AddLineTo(new Point(newLeft, newTop));
                    break;
                case 'E':
                    screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    screenCanvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    break;
                case 'F':
                    screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    screenCanvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    break;
                case 'G':
                    screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, newTop));
                    screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    screenCanvas.AddLineTo(new Point(halfRight, halfDown));
                    break;
                case 'H':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, bottomSide));
                    screenCanvas.AddLine(new Point(rightSide, newTop), new Point(rightSide, bottomSide));
                    screenCanvas.AddLine(new Point(newLeft, halfDown), new Point(rightSide, halfDown));
                    // case 'I' handled by '1'
                    break;
                case 'J':
                    screenCanvas.AddLine(new Point(rightSide, newTop), new Point(rightSide, bottomSide));
                    screenCanvas.AddLineTo(new Point(halfRight, bottomSide));
                    screenCanvas.AddLineTo(new Point(newLeft, halfDown));
                    break;
                case 'K':
                    screenCanvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, newTop));
                    screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, halfDown));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case 'L':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, bottomSide));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case 'M':
                    screenCanvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, newTop));
                    screenCanvas.AddLineTo(new Point(halfRight, halfDown));
                    screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                case 'N':
                    screenCanvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, newTop));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    break;
                // case 'O' handled by '0'
                case 'P':
                    screenCanvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, newTop));
                    screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    screenCanvas.AddLineTo(new Point(newLeft, halfDown));
                    break;
                case 'Q':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    screenCanvas.AddLineTo(new Point(newLeft, newTop));
                    screenCanvas.AddLine(new Point(halfRight, halfDown), new Point(rightSide, bottomSide));
                    break;
                case 'R':
                    screenCanvas.AddLine(new Point(newLeft, bottomSide), new Point(newLeft, newTop));
                    screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    screenCanvas.AddLineTo(new Point(rightSide, halfDown));
                    screenCanvas.AddLineTo(new Point(newLeft, halfDown));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
                // case 'S' handled by '5'
                case 'T':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    screenCanvas.AddLine(new Point(halfRight, newTop), new Point(halfRight, bottomSide));
                    break;
                case 'U':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, bottomSide));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    break;
                case 'V':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(halfRight, bottomSide));
                    screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    break;
                case 'W':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(newLeft, bottomSide));
                    screenCanvas.AddLineTo(new Point(halfRight, halfDown));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    break;
                case 'X':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, bottomSide));
                    screenCanvas.AddLine(new Point(rightSide, newTop), new Point(newLeft, bottomSide));
                    break;
                case 'Y':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(halfRight, halfDown));
                    screenCanvas.AddLineTo(new Point(rightSide, newTop));
                    screenCanvas.AddLine(new Point(halfRight, halfDown), new Point(halfRight, bottomSide));
                    break;
                case 'Z':
                    screenCanvas.AddLine(new Point(newLeft, newTop), new Point(rightSide, newTop));
                    screenCanvas.AddLineTo(new Point(newLeft, bottomSide));
                    screenCanvas.AddLineTo(new Point(rightSide, bottomSide));
                    break;
            }
        }
    }
}
