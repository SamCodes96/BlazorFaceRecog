using System.Drawing;
using System.Drawing.Imaging;

namespace BlazorFaceRecog.Server.Helpers;

public static class ImageHelpers
{
    public static string GetThumbnail(byte[] image) => Convert.ToBase64String(image);

    public static Bitmap GetBitmapImage(byte[] imageData)
    {
        using var ms = new MemoryStream(imageData);
        return new Bitmap(ms);
    }

    public static byte[] CropFaceInImage(byte[] imageData, Rectangle faceArea)
    {
        using var bmImage = GetBitmapImage(imageData);

        using var croppedImage = new Bitmap(bmImage.Width, bmImage.Height);
        using Graphics g = Graphics.FromImage(croppedImage);
        g.DrawImage(
            bmImage,
            new Rectangle(0, 0, croppedImage.Width, croppedImage.Height),
            faceArea,
            GraphicsUnit.Pixel);

        using var ms = new MemoryStream();
        croppedImage.Save(ms, ImageFormat.Jpeg);

        return ms.ToArray();
    }
}