using System.Drawing;
using BlazorFaceRecog.Server.Models;
using BlazorFaceRecog.Server.Services;
using BlazorFaceRecog.Shared;
using Microsoft.AspNetCore.SignalR;

namespace BlazorFaceRecog.Server.Endpoints;

public class FaceHub(
    IFaceService faceService,
    IConfiguration configuration) : Hub
{
    private readonly int _threshold = configuration.GetValue<int>("Threshold");

    public async Task RecogniseInImage(byte[] data)
    {
        var faces = faceService.DetectFacesInImage(data);

        if (faces?.FirstOrDefault() is not Rectangle detectedFace || detectedFace == default)
        {
            await Clients.Caller.SendAsync("ImageAnalyzed", null);
            return;
        }

        if (!faceService.FaceIsTrained)
        {
            await DetectedUnknownFace(detectedFace);
            return;
        }

        var recognisedFace = faceService.RecogniseFacesInImage(data, detectedFace);

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