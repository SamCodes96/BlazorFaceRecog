using BlazorFaceRecog.Server.Services;
using BlazorFaceRecog.Shared;
using Microsoft.AspNetCore.SignalR;

namespace BlazorFaceRecog.Server.Hubs;

public class FaceHub(FaceService faceRecognitionService) : Hub
{
    public async Task RecogniseInImage(byte[] data)
    {
        var faces = faceRecognitionService.DetectInImage(data);

        if (faces?.Length is null or 0)
        {
            await Clients.Caller.SendAsync("ImageAnalyzed", null);
            return;
        }

        if (!faceRecognitionService.FaceIsTrained)
        {
            await Clients.Caller.SendAsync("ImageAnalyzed", new AnalyzedImage("Unknown Face", faces[0]));
            return;
        }

        var name = faceRecognitionService.RecogniseInImage(data, faces[0]);

        await Clients.Caller.SendAsync("ImageAnalyzed", new AnalyzedImage(name, faces[0]));
    }
}