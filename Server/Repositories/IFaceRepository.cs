using BlazorFaceRecog.Server.Models;

namespace BlazorFaceRecog.Server.Repositories;

public interface IFaceRepository
{
    public void Add(Guid id, string name, byte[] image, float[] embedding);

    public void Update(Guid id, string name);

    public void Delete(Guid id);

    public DetectedFace GetNearest(float[] embedding);

    public long GetCount();

    public IEnumerable<EmbeddedFace> GetAll();
}