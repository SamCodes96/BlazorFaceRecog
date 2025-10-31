using BlazorFaceRecog.Server.Models;
using UMapx.Distance;

namespace BlazorFaceRecog.Server.Repositories;

public class InMemoryFaceRepository : IFaceRepository
{
    private readonly List<EmbeddedFace> _faces = [];

    private readonly Cosine _cosine = new();

    public void AddFace(Guid id, string name, byte[] image, float[] embedding)
    {
        _faces.Add(new EmbeddedFace(id, name, image, embedding));
    }

    public void UpdateFace(Guid id, string name)
    {
        var face = _faces.SingleOrDefault(f => f.Id == id);

        face?.Name = name;
    }

    public void DeleteFace(Guid id)
    {
        _faces.RemoveAll(f => f.Id == id);
    }

    public DetectedFace GetNearestFace(float[] embedding)
    {
        var Max = float.MinValue;
        string nearest = string.Empty;

        foreach (var face in _faces)
        {
            var d = _cosine.Compute(face.Embedding, embedding);

            if (d > Max)
            {
                nearest = face.Name;
                Max = d;
            }
        }

        var score = (1 + Max) / 2;
        return new DetectedFace { Name = nearest, Score = score };
    }

    public long GetFacesCount() => _faces.Count;

    public IEnumerable<EmbeddedFace> GetAllFaces() => _faces;
}