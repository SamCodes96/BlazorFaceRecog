namespace BlazorFaceRecog.Server.Configuration;

public class MongoDBSettings
{
    public required string ConnectionString { get; init; }
    public required string DatabaseName { get; init; }
    public required string CollectionName { get; init; }
    public required string SearchIndexName { get; init; }
}
