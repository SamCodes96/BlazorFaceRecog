using System.Drawing;
using BlazorFaceRecog.Server.Helpers;
using BlazorFaceRecog.Server.Logic;
using BlazorFaceRecog.Server.Models;
using BlazorFaceRecog.Server.Repositories;
using BlazorFaceRecog.Shared;

namespace BlazorFaceRecog.Server.Services;

public class FaceService(IFaceRepository faceRepository, FaceLogic faceLogic)
{
    public bool FaceIsTrained => faceRepository.GetCount() > 0;

    public IEnumerable<SavedFaceModel> GetSaved()
    {
        var faces = faceRepository.GetAll();

        return faces.Select(f => new SavedFaceModel(f.Id, f.Name, ImageHelpers.GetThumbnail(f.Image)));
    }

    public IEnumerable<Rectangle> DetectInImage(byte[] imageData)
    {
        using var bmImage = ImageHelpers.GetBitmapImage(imageData);

        var result = faceLogic.Detect(bmImage);

        return result.Select(f => f.Box);
    }

    public DetectedFace RecogniseInImage(byte[] imageData, Rectangle? faceArea = null)
    {
        using var bmImage = ImageHelpers.GetBitmapImage(imageData);
        faceArea ??= new Rectangle(0, 0, bmImage.Width, bmImage.Height);

        var embedding = faceLogic.GetEmbedding(bmImage, faceArea.Value);
        return faceRepository.GetNearest(embedding);
    }

    public void TrainFromImage(TrainFaceModel data, byte[] imageData, Rectangle? faceArea = null)
    {
        using var bmImage = ImageHelpers.GetBitmapImage(imageData);
        faceArea ??= new Rectangle(0, 0, bmImage.Width, bmImage.Height);

        var embedding = faceLogic.GetEmbedding(bmImage, faceArea.Value);
        faceRepository.Add(data.Id, data.Name, imageData, embedding);
    }

    public void UpdateName(Guid id, string name) => faceRepository.Update(id, name);

    public void DeleteFace(Guid Id) => faceRepository.Delete(Id);
}
