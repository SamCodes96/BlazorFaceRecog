using BlazorFaceRecog.Server.Dtos;
using MongoDB.Driver;

namespace BlazorFaceRecog.Server.Repositories;

public class FaceRepository(IMongoDatabase database, IConfiguration configuration)
    : RepositoryBase<EmbeddedFaceDto>(database, "Faces")
{
    public void Add(string name, float[] embedding)
    {
        Collection.InsertOne(new EmbeddedFaceDto(name, embedding));
    }

    public string GetNearestFace(float[] embedding)
    {
        var searchOptions = new VectorSearchOptions<EmbeddedFaceDto>()
        {
            IndexName = configuration.GetSection("MongoDB")["SearchIndexName"],
            NumberOfCandidates = (int)Count
        };

        var result = Collection.Aggregate()
            .VectorSearch(f => f.Embedding, embedding, 1, searchOptions)
            .Project(Builders<EmbeddedFaceDto>.Projection
                .Include(f => f.Name))
            .FirstOrDefault();

        return result?[nameof(EmbeddedFaceDto.Name)]?.AsString ?? "Unknown Face";
    }
}