using System.Drawing;

namespace Asteroids.Standard.Helpers
{
    public static class ColorHexStrings
    {
        public static string RedHex = Color.Red.ToHexString();
        public static string YellowHex = Color.Yellow.ToHexString();
        public static string OrangeHex = Color.Orange.ToHexString();
        public static string WhiteHex = Color.White.ToHexString();
        public static string TransparentHex = Color.White.ToHexString();

        public static string ToHexString(this Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}
