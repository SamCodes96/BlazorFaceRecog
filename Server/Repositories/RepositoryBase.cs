using MongoDB.Driver;

namespace BlazorFaceRecog.Server.Repositories;

public abstract class RepositoryBase<T>(IMongoDatabase database, string collectionName)
{
    protected IMongoCollection<T> Collection = database.GetCollection<T>(collectionName);

    public long Count => Collection.CountDocuments(Builders<T>.Filter.Empty);
}
