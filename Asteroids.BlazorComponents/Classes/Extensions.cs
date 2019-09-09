using System;
using System.IO;
using System.Text;

namespace Asteroids.BlazorComponents.Classes
{
    public static class Extensions
    {
        /// <summary>
        /// Converts the passed <see cref="Stream"/> to a base64 string using <see cref="Encoding.UTF8"/> encoding.
        /// </summary>
        /// <remarks>Encoding.UTF8.GetBytes can be used to convert back.</remarks>
        public static string ToBase64(this Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.Position = 0;
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            return Convert.ToBase64String(bytes);
        }
    }
}
