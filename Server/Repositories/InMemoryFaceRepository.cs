using BlazorFaceRecog.Server.Models;
using UMapx.Core;

namespace BlazorFaceRecog.Server.Repositories;

public class InMemoryFaceRepository : IFaceRepository
{
    private readonly List<EmbeddedFace> _faces = [];

    public void Add(Guid id, string name, byte[] image, float[] embedding)
    {
        _faces.Add(new EmbeddedFace(id, name, image, embedding));
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

    public DetectedFace GetNearestFace(float[] embedding)
    {
        var Max = float.MinValue;
        string nearest = string.Empty;

        foreach (var face in _faces)
        {
            var d = face.Embedding.Cosine(embedding);

            if (d > Max)
            {
                nearest = face.Name;
                Max = d;
            }
        }

        var score = (1 + Max) / 2;
        return new DetectedFace(nearest, score);
    }

    public long GetCount() => _faces.Count;

    public IEnumerable<EmbeddedFace> GetAllItems() => _faces;
}