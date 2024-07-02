using BlazorFaceRecog.Server.Dtos;
using UMapx.Core;

namespace BlazorFaceRecog.Server.Repositories;

public class InMemoryFaceRepository : IFaceRepository
{
    private readonly List<EmbeddedFaceDto> _faces = [];

    public void Add(Guid id, string name, byte[] image, float[] embedding)
    {
        _faces.Add(new EmbeddedFaceDto(id, name, image, embedding));
    }

    public void Update(Guid id, string name)
    {
        var face = _faces.SingleOrDefault(x => x.Id == id);

        if (face == null)
            return;

        face.Name = name;
    }

    public void Delete(Guid id)
    {
        _faces.RemoveAll(x => x.Id == id);
    }

    public string GetNearestFace(float[] embedding)
    {
        var min = float.MaxValue;
        string nearest = string.Empty;

        // do job
        foreach (var face in _faces)
        {
            var d = face.Embedding.Euclidean(embedding);

            if (d < min)
            {
                nearest = face.Name;
                min = d;
            }
        }

        // result
        return nearest;
    }

    public long GetCount() => _faces.Count;

    public IEnumerable<EmbeddedFaceDto> GetAllItems() => _faces;
}