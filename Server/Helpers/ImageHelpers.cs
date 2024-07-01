using System.Drawing;

namespace BlazorFaceRecog.Server.Helpers;

public static class ImageHelpers
{
    public static string GetThumbnail(byte[] image) => Convert.ToBase64String(image);

    public static Bitmap GetBitmapImage(byte[] imageData)
    {
        using var ms = new MemoryStream(imageData);
        return new Bitmap(ms);
    }
}
