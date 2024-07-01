using System.Drawing;
using System.Drawing.Imaging;
using BlazorFaceRecog.Server.Helpers;
using BlazorFaceRecog.Server.Logic;
using BlazorFaceRecog.Server.Repositories;
using BlazorFaceRecog.Shared;

namespace BlazorFaceRecog.Server.Services;

public class FaceService(
    FaceRepository faceRepository,
    FaceLogic faceLogic)
{
    public bool FaceIsTrained => faceRepository.GetCount() > 0;

    public IEnumerable<SavedFaceModel> GetSavedFaces()
    {
        return faceRepository.GetAllItems().Select(x => new SavedFaceModel(x.Id, x.Name, ImageHelpers.GetThumbnail(x.Image)));
    }

    public Rectangle[] DetectInImage(byte[] imageData)
    {
        using var bmImage = ImageHelpers.GetBitmapImage(imageData);

        var result = faceLogic.Detect(bmImage);

        return result.Select(x => x.Box).ToArray();
    }

    public string RecogniseInImage(byte[] imageData, Rectangle? faceArea = null)
    {
        using var bmImage = ImageHelpers.GetBitmapImage(imageData);
        faceArea ??= new Rectangle(0, 0, bmImage.Width, bmImage.Height);

        var embedding = faceLogic.GetEmbedding(bmImage, faceArea.Value);
        return faceRepository.GetNearestFace(embedding);
    }

    public void TrainFromImage(TrainFaceModel data, byte[] imageData, Rectangle? faceArea = null)
    {
        using var bmImage = ImageHelpers.GetBitmapImage(imageData);
        faceArea ??= new Rectangle(0, 0, bmImage.Width, bmImage.Height);

        var embedding = faceLogic.GetEmbedding(bmImage, faceArea.Value);
        faceRepository.Add(data.Id, data.Name, imageData, embedding);
    }

    public void UpdateName(Guid id, string name)
    {
        faceRepository.Update(id, name);
    }

    public void DeleteFace(Guid Id)
    {
        faceRepository.Delete(Id);
    }

    public byte[] CropFaceInImage(byte[] imageData, Rectangle faceArea)
    {
        using var bmImage = ImageHelpers.GetBitmapImage(imageData);

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
