using BlazorFaceRecog.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorFaceRecog.Client.Services;

public class FaceHubService
{
    private readonly HubConnection _hubConnection;

    public event Action<AnalyzedImage?>? OnResponseReceived;

    public FaceHubService(NavigationManager navigationManager)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.BaseUri + "FaceHub")
            .Build();

        _hubConnection.On<AnalyzedImage?>("ImageAnalyzed", HandleResponse);
    }

    public async Task StartConnection()
    {
        await _hubConnection.StartAsync();
    }

    public async Task RecogniseFaces(ImageData imageData)
    {
        await _hubConnection.SendAsync("RecogniseInImage", imageData);
    }

    private void HandleResponse(AnalyzedImage? response)
    {
        OnResponseReceived?.Invoke(response);
    }
}