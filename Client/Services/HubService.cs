using BlazorFaceRecog.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorFaceRecog.Client.Services;

public interface IHubService
{
    event Func<AnalyzedImage?, Task>? OnResponseReceived;
    event Func<Exception?, Task>? OnDisconnect;

    Task StartConnectionAsync();
    Task EndConnectionAsync();
    Task RecogniseFacesAsync(byte[] imageData);
}

public class HubService : IHubService
{
    private readonly HubConnection _hubConnection;

    public event Func<AnalyzedImage?, Task>? OnResponseReceived;

    public event Func<Exception?, Task>? OnDisconnect
    {
        add => _hubConnection?.Closed += value;
        remove => _hubConnection?.Closed -= value;
    }

    public HubService(NavigationManager navigationManager)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.BaseUri + "Faces/Recognise")
            .Build();

        _hubConnection.On<AnalyzedImage?>("ImageAnalyzed", HandleResponse);
    }

    public async Task StartConnectionAsync()
    {
        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            await _hubConnection.StartAsync();
        }
    }

    public async Task EndConnectionAsync()
    {
        if (_hubConnection.State != HubConnectionState.Disconnected)
        {
            await _hubConnection.StopAsync();
        }
    }

    public async Task RecogniseFacesAsync(byte[] imageData)
    {
        await _hubConnection.SendAsync("RecogniseInImage", imageData);
    }

    private void HandleResponse(AnalyzedImage? response) => OnResponseReceived?.Invoke(response);
}