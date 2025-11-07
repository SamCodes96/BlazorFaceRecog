using Microsoft.Extensions.DependencyInjection;

namespace BlazorFaceRecog.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendServices(this IServiceCollection services, string baseUrl)
    {
        services.AddScoped<IApiService, ApiService>();
        services.AddScoped<IHubService, HubService>();

        services.AddTransient<IHubConnectionFactory>(_ => new HubConnectionFactory(baseUrl));

        return services;
    }
}
