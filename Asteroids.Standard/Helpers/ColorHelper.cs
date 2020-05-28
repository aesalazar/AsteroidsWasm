using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Asteroids.Standard.Enums;

namespace Asteroids.Standard.Helpers
{
    /// <summary>
    /// Drawing colors used by the game engine.
    /// </summary>
    internal static class ColorHelper
    {
        /// <summary>
        /// Collection of <see cref="DrawColor"/> HEX string values used by the game engine.
        /// </summary>
        public static IDictionary<DrawColor, string> DrawColorMap { get; } = new ReadOnlyDictionary<DrawColor, string>(
            new Dictionary<DrawColor, string>
            {
                [DrawColor.White] = Color.White.ToHexString(),
                [DrawColor.Red] = Color.Red.ToHexString(),
                [DrawColor.Yellow] = Color.Yellow.ToHexString(),
                [DrawColor.Orange] = Color.Orange.ToHexString(),

            }
        );

        /// <summary>
        /// Collection of <see cref="DrawColor"/> keys in <see cref="DrawColorMap"/>.
        /// </summary>
        public static IList<DrawColor> DrawColorList { get; } = DrawColorMap.Keys.OrderBy(k => k).ToList();

        /// <summary>
        /// Converts a <see cref="Color"/> to an html-formatted text string (e.g. #RRGGBB).
        /// </summary>
        public static string ToHexString(this Color c)
        {
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }
    }
}
