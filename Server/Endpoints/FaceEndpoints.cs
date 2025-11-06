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

        group.MapGet("/", Get);
        group.MapPost("/Train", Train);
        group.MapPost("/Detect", Detect);

        return group;
    }

    private static IResult Get(
        [FromServices] IFaceService faceService)
    {
        var saved = faceService.GetSavedFaces();
        return Results.Ok(saved);
    }

    private static IResult Train(
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

    private static IResult Detect(
        [FromBody] DetectFaceModel detectFaceModel,
        [FromServices] IFaceService faceService,
        [FromServices] IFaceCache faceCache,
        [FromServices] IImageLogic imageLogic)
    {
        var faces = faceService.DetectFacesInImage(detectFaceModel.ImageData);

        switch (faces.Count())
        {
            case > 1: return Results.BadRequest("Multiple faces detected");
            case < 1: return Results.BadRequest("No faces detected");
        }

        var croppedFace = imageLogic.CropFaceInImage(detectFaceModel.ImageData, faces.First());

        faceCache.SetFace(detectFaceModel.Id, croppedFace);

        var thumbnail = imageLogic.GetThumbnail(croppedFace);

        return Results.Content(thumbnail);
    }
}
