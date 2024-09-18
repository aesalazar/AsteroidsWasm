using System;
using SkiaSharp;

namespace Asteroids.WinUi3.Helpers;

/// <summary>
/// Helpers for working with Skia Colors.
/// </summary>
internal class ColorHelper
{
    /// <summary>
    /// Converts an Color Code to a Skia Paint object.
    /// </summary>
    /// <param name="colorHex">HTML color HEX code.</param>
    /// <returns>Hydrated <see cref="SKPaint"/> object.</returns>
    public static SKPaint ColorHexToPaint(string colorHex)
    {
        var hex = colorHex.Replace("#", string.Empty);
        var length = hex.Length;

        var bytes = new byte[length / 2];

        for (var i = 0; i < length; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

        return new SKPaint
        {
            Color = new SKColor(bytes[0], bytes[1], bytes[2]),
            IsStroke = true,
        };
    }

}
