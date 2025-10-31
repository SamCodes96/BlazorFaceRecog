using BlazorFaceRecog.Server.Configuration;
using BlazorFaceRecog.Server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BlazorFaceRecog.Server.Repositories;

public class MongoFaceRepository(
    IMongoDatabase database,
    IOptions<MongoDBSettings> mongoSettings)
    : IFaceRepository
{
    private readonly IMongoCollection<EmbeddedFace> _collection
        = database.GetCollection<EmbeddedFace>(mongoSettings.Value.CollectionName);

    public void AddFace(Guid id, string name, byte[] image, float[] embedding)
    {
        _collection.InsertOne(new EmbeddedFace(id, name, image, embedding));
    }

    public void UpdateFace(Guid id, string name)
    {
        var filter = Builders<EmbeddedFace>.Filter.Eq(f => f.Id, id);
        var update = Builders<EmbeddedFace>.Update.Set(f => f.Name, name);

        _collection.UpdateOne(filter, update);
    }

    public void DeleteFace(Guid id)
    {
        var filter = Builders<EmbeddedFace>.Filter.Eq(f => f.Id, id);

        _collection.DeleteOne(filter);
    }

    public DetectedFace GetNearestFace(float[] embedding)
    {
        var searchOptions = new VectorSearchOptions<EmbeddedFace>()
        {
            IndexName = mongoSettings.Value.SearchIndexName,
            NumberOfCandidates = (int)GetFacesCount()
        };

        return _collection.Aggregate()
            .VectorSearch(f => f.Embedding, embedding, 1, searchOptions)
            .Project(Builders<EmbeddedFace>.Projection
                .Include(f => f.Name)
                .Meta(nameof(DetectedFace.Score), "vectorSearchScore"))
            .As<DetectedFace>()
            .FirstOrDefault();
    }

    public long GetFacesCount() => _collection.CountDocuments(NoFilter);

    public IEnumerable<EmbeddedFace> GetAllFaces() => _collection.Find(NoFilter).ToEnumerable();

    private static FilterDefinition<EmbeddedFace> NoFilter => Builders<EmbeddedFace>.Filter.Empty;
}