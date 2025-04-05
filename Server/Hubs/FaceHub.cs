using System.Drawing;
using BlazorFaceRecog.Server.Models;
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

        if (recognisedFace?.Score is not float score || score < _threshold)
        {
            await DetectedUnknownFace(detectedFace);
            return;
        }

        await DetectedKnownFace(detectedFace, recognisedFace);
    }

    private async Task DetectedKnownFace(Rectangle detectedFace, DetectedFace recognisedFace)
        => await Clients.Caller.SendAsync("ImageAnalyzed", new AnalyzedImage(recognisedFace.Name, recognisedFace.Score, detectedFace));

    private async Task DetectedUnknownFace(Rectangle detectedFace)
        => await Clients.Caller.SendAsync("ImageAnalyzed", new AnalyzedImage("Unknown Face", 0, detectedFace));
}