using System.Drawing;
using BlazorFaceRecog.Server.Services;
using BlazorFaceRecog.Shared;
using Microsoft.AspNetCore.SignalR;

namespace BlazorFaceRecog.Server.Hubs;

public class FaceHub(FaceService faceRecognitionService, IConfiguration configuration) : Hub
{
    private readonly int _threshold = configuration.GetValue<int?>("Threshold") ?? 0;

    public async Task RecogniseInImage(byte[] data)
    {
        var faces = faceRecognitionService.DetectInImage(data);

        if (faces?.FirstOrDefault() is not Rectangle detectedFace)
        {
            await Clients.Caller.SendAsync("ImageAnalyzed", null);
            return;
        }

        if (!faceRecognitionService.FaceIsTrained)
        {
            await DetectedUnknownFace(detectedFace);
            return;
        }

        var recognisedFace = faceRecognitionService.RecogniseInImage(data, detectedFace);

        if (_threshold > (recognisedFace?.Score ?? 0))
        {
            await DetectedUnknownFace(detectedFace);
            return;
        }

        await Clients.Caller.SendAsync("ImageAnalyzed", new AnalyzedImage(recognisedFace!.Name, recognisedFace.Score, detectedFace));
    }

    private Task DetectedUnknownFace(Rectangle face) => Clients.Caller.SendAsync("ImageAnalyzed", new AnalyzedImage("Unknown Face", 0, face));
}