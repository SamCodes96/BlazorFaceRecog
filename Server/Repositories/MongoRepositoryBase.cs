using MongoDB.Driver;

namespace BlazorFaceRecog.Server.Repositories;

public abstract class MongoRepositoryBase<T>(IMongoDatabase database, string collectionName)
{
    protected IMongoCollection<T> Collection = database.GetCollection<T>(collectionName);

    protected static FilterDefinition<T> NoFilter => Builders<T>.Filter.Empty;
}