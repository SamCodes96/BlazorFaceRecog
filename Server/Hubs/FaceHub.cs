using System.Drawing;
using BlazorFaceRecog.Server.Services;
using BlazorFaceRecog.Shared;
using Microsoft.AspNetCore.SignalR;

namespace BlazorFaceRecog.Server.Hubs;

public class FaceHub(FaceService faceRecognitionService, IConfiguration configuration) : Hub
{
    private readonly int _threshold = configuration.GetValue<int?>("Threshold") ?? int.MaxValue;

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
            await DetectedUnknownFace(faces[0]);
            return;
        }

        var detectedFace = faceRecognitionService.RecogniseInImage(data, faces[0]);

        if (_threshold > detectedFace.Score)
        {
            await DetectedUnknownFace(faces[0]);
            return;
        }

        await Clients.Caller.SendAsync("ImageAnalyzed", new AnalyzedImage(detectedFace.Name, detectedFace.Score, faces[0]));
    }

    private Task DetectedUnknownFace(Rectangle face) => Clients.Caller.SendAsync("ImageAnalyzed", new AnalyzedImage("Unknown Face", 0, face));
}