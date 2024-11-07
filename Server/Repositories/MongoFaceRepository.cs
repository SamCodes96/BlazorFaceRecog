using BlazorFaceRecog.Server.Models;
using MongoDB.Driver;

namespace BlazorFaceRecog.Server.Repositories;

public class MongoFaceRepository(IMongoDatabase database, IConfiguration configuration)
    : MongoRepositoryBase<EmbeddedFace>(database, "Faces"), IFaceRepository
{
    public void Add(Guid id, string name, byte[] image, float[] embedding)
    {
        Collection.InsertOne(new EmbeddedFace(id, name, image, embedding));
    }

    public void Update(Guid id, string name)
    {
        var filter = Builders<EmbeddedFace>.Filter.Eq(x => x.Id, id);
        var update = Builders<EmbeddedFace>.Update.Set(x => x.Name, name);

        Collection.UpdateOne(filter, update);
    }

    public void Delete(Guid id)
    {
        var filter = Builders<EmbeddedFace>.Filter.Eq(x => x.Id, id);

        Collection.DeleteOne(filter);
    }

    public DetectedFace GetNearestFace(float[] embedding)
    {
        var searchOptions = new VectorSearchOptions<EmbeddedFace>()
        {
            IndexName = configuration.GetSection("MongoDB")["SearchIndexName"],
            NumberOfCandidates = (int)GetCount()
        };

        return Collection.Aggregate()
            .VectorSearch(f => f.Embedding, embedding, 1, searchOptions)
            .Project(Builders<EmbeddedFace>.Projection
                .Include(x => x.Name)
                .Meta(nameof(DetectedFace.Score), "vectorSearchScore"))
            .As<DetectedFace>()
            .FirstOrDefault();
    }

    public long GetCount() => Collection.CountDocuments(NoFilter);

    public IEnumerable<EmbeddedFace> GetAllItems() => Collection.Find(NoFilter).ToEnumerable();
}