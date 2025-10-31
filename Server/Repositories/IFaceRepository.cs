using BlazorFaceRecog.Server.Models;

namespace BlazorFaceRecog.Server.Repositories;

public interface IFaceRepository
{
    public void AddFace(Guid id, string name, byte[] image, float[] embedding);

    public void UpdateFace(Guid id, string name);

    public void DeleteFace(Guid id);

    public DetectedFace GetNearestFace(float[] embedding);

    public long GetFacesCount();

    public IEnumerable<EmbeddedFace> GetAllFaces();
}