using System.ComponentModel.DataAnnotations;

namespace BlazorFaceRecog.Server.Configuration;

public class MongoDBSettings
{
    [Required]
    public required string ConnectionString { get; init; }
    [Required]
    public required string DatabaseName { get; init; }
    [Required]
    public required string CollectionName { get; init; }
    [Required]
    public required string SearchIndexName { get; init; }
}
