using BlazorFaceRecog.Server.Configuration;
using BlazorFaceRecog.Server.Models;
using MongoDB.Driver;

namespace BlazorFaceRecog.Server.Repositories;

public class MongoFaceRepository(IMongoDatabase database, MongoDBSettings mongoSettings)
    : MongoRepositoryBase<EmbeddedFace>(database, mongoSettings.CollectionName!), IFaceRepository
{
    public void Add(Guid id, string name, byte[] image, float[] embedding)
    {
        Collection.InsertOne(new EmbeddedFace(id, name, image, embedding));
    }

    public void Update(Guid id, string name)
    {
        var filter = Builders<EmbeddedFace>.Filter.Eq(f => f.Id, id);
        var update = Builders<EmbeddedFace>.Update.Set(f => f.Name, name);

        Collection.UpdateOne(filter, update);
    }

    public void Delete(Guid id)
    {
        var filter = Builders<EmbeddedFace>.Filter.Eq(f => f.Id, id);

        Collection.DeleteOne(filter);
    }

    public DetectedFace GetNearest(float[] embedding)
    {
        var searchOptions = new VectorSearchOptions<EmbeddedFace>()
        {
            IndexName = mongoSettings.SearchIndexName,
            NumberOfCandidates = (int)GetCount()
        };

        return Collection.Aggregate()
            .VectorSearch(f => f.Embedding, embedding, 1, searchOptions)
            .Project(Builders<EmbeddedFace>.Projection
                .Include(f => f.Name)
                .Meta(nameof(DetectedFace.Score), "vectorSearchScore"))
            .As<DetectedFace>()
            .FirstOrDefault();
    }

    public long GetCount() => Collection.CountDocuments(NoFilter);

    public IEnumerable<EmbeddedFace> GetAll() => Collection.Find(NoFilter).ToEnumerable();
}