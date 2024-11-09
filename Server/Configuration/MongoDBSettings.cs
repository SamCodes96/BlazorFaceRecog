namespace BlazorFaceRecog.Server.Configuration;

public class MongoDBSettings
{
    public string? ConnectionString { get; init; }
    public string? DatabaseName { get; init; }
    public string? CollectionName { get; init; }
    public string? SearchIndexName { get; init; }
}
