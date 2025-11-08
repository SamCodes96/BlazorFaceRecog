using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorFaceRecog.Client.Services;

public interface IHubConnectionFactory
{
    HubConnection CreateHubConnection(string baseUrl);
}

public class HubConnectionFactory(string baseUrl) : IHubConnectionFactory
{
    public HubConnection CreateHubConnection(string relativeUrl)
    {
        var builder = new HubConnectionBuilder();
        var fullUrl = new Uri(new Uri(baseUrl), relativeUrl);

        builder.WithUrl(fullUrl);

        return builder.Build();
    }
}
