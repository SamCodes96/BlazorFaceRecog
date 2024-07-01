using MongoDB.Driver;

namespace BlazorFaceRecog.Server.Repositories;

public abstract class RepositoryBase<T>(IMongoDatabase database, string collectionName)
{
    protected IMongoCollection<T> Collection = database.GetCollection<T>(collectionName);

    protected FilterDefinition<T> NoFilter => Builders<T>.Filter.Empty;

    public long GetCount() => Collection.CountDocuments(NoFilter);

    public IEnumerable<T> GetAllItems() => Collection.Find(NoFilter).ToEnumerable();
}
