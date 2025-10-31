using System.Drawing;
using System.Drawing.Imaging;

namespace BlazorFaceRecog.Server.Logic;

public interface IImageLogic
{
    byte[] CropFaceInImage(byte[] imageData, Rectangle faceArea);
    Bitmap GetBitmapImage(byte[] imageData);
    string GetThumbnail(byte[] image);
}

public class ImageLogic : IImageLogic
{
    public byte[] CropFaceInImage(byte[] imageData, Rectangle faceArea)
    {
        using var bmImage = GetBitmapImage(imageData);

        using var croppedImage = new Bitmap(bmImage.Width, bmImage.Height);
        using var graphics = Graphics.FromImage(croppedImage);
        graphics.DrawImage(
            bmImage,
            new Rectangle(0, 0, croppedImage.Width, croppedImage.Height),
            faceArea,
            GraphicsUnit.Pixel);

        using var ms = new MemoryStream();
        croppedImage.Save(ms, ImageFormat.Jpeg);

        return ms.ToArray();
    }

    public Bitmap GetBitmapImage(byte[] imageData)
    {
        using var ms = new MemoryStream(imageData);
        return new Bitmap(ms);
    }

    public string GetThumbnail(byte[] image) => Convert.ToBase64String(image);
}