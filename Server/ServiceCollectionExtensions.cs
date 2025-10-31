using BlazorFaceRecog.Server.Configuration;
using BlazorFaceRecog.Server.Repositories;
using FaceONNX;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using ONNXOptions = Microsoft.ML.OnnxRuntime.SessionOptions;

namespace BlazorFaceRecog.Server;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFaceONNX(this IServiceCollection services)
    {
        services.AddTransient<IFace68LandmarksExtractor, Face68LandmarksExtractor>();
        services.AddTransient<IFaceDetector, FaceDetector>();
        services.AddTransient<IFaceClassifier, FaceEmbedder>();

        services.AddSingleton((sp) =>
        {
            var settings = sp.GetService<IOptions<AppSettings>>()!;
            if (settings.Value.UseGpu)
                return ONNXOptions.MakeSessionOptionWithCudaProvider();

            return new ONNXOptions();
        });

        return services;
    }

    public static IServiceCollection AddMongoDB(this IServiceCollection services, IConfigurationSection mongoSettings)
    {
        services
            .AddOptions<MongoDBSettings>()
            .Bind(mongoSettings)
            .ValidateDataAnnotations();

        services.AddScoped<IFaceRepository, MongoFaceRepository>();

        services.AddSingleton<IMongoClient, MongoClient>((sp) =>
        {
            var settings = sp.GetService<IOptions<MongoDBSettings>>()!;
            return new MongoClient(settings.Value.ConnectionString);
        });

        services.AddScoped((sp) =>
        {
            var databaseName = sp.GetService<IOptions<MongoDBSettings>>()!.Value.DatabaseName;
            var mongoClient = sp.GetService<IMongoClient>()!;
            return mongoClient.GetDatabase(databaseName);
        });

        // This stops MongoDB throwing an error because there isn't an _id field on the models
        ConventionRegistry.Register("IgnoreExtraElements", new ConventionPack { new IgnoreExtraElementsConvention(true) }, _ => true);

        return services;
    }
}
