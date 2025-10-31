using System.Drawing;
using BlazorFaceRecog.Server.Logic;
using BlazorFaceRecog.Server.Models;
using BlazorFaceRecog.Server.Repositories;
using BlazorFaceRecog.Shared;

namespace BlazorFaceRecog.Server.Services;

public interface IFaceService
{
    bool FaceIsTrained { get; }
    void DeleteFace(Guid Id);
    IEnumerable<Rectangle> DetectFacesInImage(byte[] imageData);
    IEnumerable<SavedFaceModel> GetSavedFaces();
    DetectedFace RecogniseFacesInImage(byte[] imageData, Rectangle? faceArea = null);
    void TrainFacesFromImage(TrainFaceModel data, byte[] imageData, Rectangle? faceArea = null);
    void UpdateFaceName(Guid id, string name);
}

public class FaceService(
    IFaceRepository faceRepository,
    IFaceLogic faceLogic,
    IImageLogic imageLogic) : IFaceService
{
    public bool FaceIsTrained
    {
        get
        {
            if (!field)
            {
                field = faceRepository.GetFacesCount() > 0;
            }

            return field;
        }
    }

    public void DeleteFace(Guid Id)
    {
        faceRepository.DeleteFace(Id);
    }

    public IEnumerable<Rectangle> DetectFacesInImage(byte[] imageData)
    {
        using var bmImage = imageLogic.GetBitmapImage(imageData);

        var result = faceLogic.DetectFaces(bmImage);

        return result.Select(f => f.Box);
    }

    public IEnumerable<SavedFaceModel> GetSavedFaces()
    {
        var faces = faceRepository.GetAllFaces();

        return faces.Select(f => new SavedFaceModel(f.Id, f.Name, imageLogic.GetThumbnail(f.Image)));
    }

    public DetectedFace RecogniseFacesInImage(byte[] imageData, Rectangle? faceArea = null)
    {
        using var bmImage = imageLogic.GetBitmapImage(imageData);
        faceArea ??= new Rectangle(0, 0, bmImage.Width, bmImage.Height);

        var embedding = faceLogic.GetEmbeddingForFace(bmImage, faceArea.Value);
        return faceRepository.GetNearestFace(embedding);
    }

    public void TrainFacesFromImage(TrainFaceModel data, byte[] imageData, Rectangle? faceArea = null)
    {
        using var bmImage = imageLogic.GetBitmapImage(imageData);
        faceArea ??= new Rectangle(0, 0, bmImage.Width, bmImage.Height);

        var embedding = faceLogic.GetEmbeddingForFace(bmImage, faceArea.Value);
        faceRepository.AddFace(data.Id, data.Name, imageData, embedding);
    }

    public void UpdateFaceName(Guid id, string name)
    {
        faceRepository.UpdateFace(id, name);
    }
}
