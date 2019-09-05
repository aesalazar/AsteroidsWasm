using System.Drawing;

namespace Asteroids.Standard.Helpers
{
    /// <summary>
    /// Helpers for converting HEX-based colors and strings.
    /// </summary>
    public static class ColorHexStrings
    {
        /// <summary>
        /// Converts a <see cref="Color"/> to an html-formatted text string (e.g. #RRGGBB).
        /// </summary>
        public static string ToHexString(this Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}
