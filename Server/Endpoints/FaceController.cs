using BlazorFaceRecog.Server.Logic;
using BlazorFaceRecog.Server.Services;
using BlazorFaceRecog.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFaceRecog.Server.Endpoints;

[ApiController]
[Route("Faces")]
public class FaceController(
    FaceCache faceCache,
    IFaceService faceService,
    IImageLogic imageLogic) : ControllerBase
{
    [HttpGet]
    public IActionResult Saved()
    {
        var saved = faceService.GetSavedFaces();

        return Ok(saved);
    }

    [HttpPost, Route(nameof(Train))]
    public IActionResult Train([FromBody] IEnumerable<TrainFaceModel> faceModels)
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

        return Ok();
    }

    [HttpPost, Route(nameof(Detect))]
    public IActionResult Detect([FromBody] DetectFaceModel detectFaceModel)
    {
        var faces = faceService.DetectFacesInImage(detectFaceModel.ImageData);

        switch (faces.Count())
        {
            case > 1: return BadRequest("Multiple faces detected");
            case < 1: return BadRequest("No faces detected");
        }

        var croppedFace = imageLogic.CropFaceInImage(detectFaceModel.ImageData, faces.First());

        faceCache.SetFace(detectFaceModel.Id, croppedFace);

        var thumbnail = imageLogic.GetThumbnail(croppedFace);

        return Content(thumbnail);
    }
}