using System.IO;
using Windows.Storage.Streams;

/// <summary>
/// Helper methods for playing sound in WinUI.
/// </summary>
internal static class SoundPlayerHelper
{
    /// <summary>
    /// Converts a standard <see cref="Stream"/> into an <see cref="IRandomAccessStream"/> that can be played by WinUI.
    /// </summary>
    /// <param name="inputStream">IO Stream to convert.</param>
    /// <returns>Initialized WinUI Stream.</returns>
    public static IRandomAccessStream ToRandomAccessStream(this Stream inputStream)
    {
        var randomAccessStream = new InMemoryRandomAccessStream();
        using var outputStream = randomAccessStream.GetOutputStreamAt(0);

        var writer = new DataWriter(outputStream);
        var buffer = new byte[inputStream.Length];

        inputStream.Read(buffer, 0, buffer.Length);
        writer.WriteBytes(buffer);
        writer.StoreAsync().GetResults();
        randomAccessStream.Seek(0);

        return randomAccessStream;
    }
}