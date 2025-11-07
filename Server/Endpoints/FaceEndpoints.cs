using System.Net;
using System.Net.Mime;
using BlazorFaceRecog.Server.Logic;
using BlazorFaceRecog.Server.Services;
using BlazorFaceRecog.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFaceRecog.Server.Endpoints;

public static class FaceEndpoints
{
    public static RouteGroupBuilder MapFaceEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/Faces");

        group.MapGet("/", GetSavedFaces)
            .WithName(nameof(GetSavedFaces))
            .Produces<IEnumerable<SavedFaceModel>>();

        group.MapPost("/Train", TrainFaces)
            .WithName(nameof(TrainFaces))
            .Produces((int)HttpStatusCode.Created);

        group.MapPost("/Detect", DetectFaces)
            .WithName(nameof(DetectFaces))
            .Produces<string>((int)HttpStatusCode.BadRequest, MediaTypeNames.Text.Plain)
            .Produces<string>((int)HttpStatusCode.OK, MediaTypeNames.Text.Plain);

        group.MapHub<FaceHub>("/Recognise");

        group.DisableAntiforgery();

        return group;
    }

    private static IResult GetSavedFaces(
        [FromServices] IFaceService faceService)
    {
        var saved = faceService.GetSavedFaces();
        return Results.Ok(saved);
    }

    private static IResult TrainFaces(
        [FromBody] IEnumerable<TrainFaceModel> faceModels,
        [FromServices] IFaceService faceService,
        [FromServices] IFaceCache faceCache)
    {
        var existingIds = faceService.GetSavedFaces().Select(f => f.Id).ToList();

        var updatedFaceIds = new List<Guid>();

        foreach (var face in faceModels)
        {
            if (!existingIds.Contains(face.Id))
            {
                if (faceCache.GetFace(face.Id) is byte[] cachedFace)
                    faceService.TrainFacesFromImage(face, cachedFace);
            }
            else
            {
                updatedFaceIds.Add(face.Id);
                faceService.UpdateFaceName(face.Id, face.Name);
            }
        }

        foreach (var deletedItem in existingIds.Except(updatedFaceIds))
            faceService.DeleteFace(deletedItem);

        return Results.Created();
    }

    private static IResult DetectFaces(
        IFormFile file,
        [FromForm] Guid id,
        [FromServices] IFaceService faceService,
        [FromServices] IFaceCache faceCache,
        [FromServices] IImageLogic imageLogic)
    {
        byte[] image;
        using (var ms = new MemoryStream())
        {
            file.CopyTo(ms);
            image = ms.ToArray();
        }

        var faces = faceService.DetectFacesInImage(image);

        switch (faces.Count())
        {
            case > 1: return Results.BadRequest("Multiple faces detected");
            case < 1: return Results.BadRequest("No faces detected");
        }

        var croppedFace = imageLogic.CropFaceInImage(image, faces.First());

        faceCache.SetFace(id, croppedFace);

        var thumbnail = imageLogic.GetThumbnail(croppedFace);

        return Results.Content(thumbnail);
    }
}
