using BlazorFaceRecog.Server.Helpers;
using BlazorFaceRecog.Server.Services;
using BlazorFaceRecog.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFaceRecog.Server.Controllers
{
    [ApiController]
    [Route("Faces")]
    public class FaceController(FaceService faceService, CacheService cacheService) : ControllerBase
    {
        [HttpGet]
        [Route(nameof(Saved))]
        public IActionResult Saved()
        {
            var saved = faceService.GetSaved();

            return Ok(saved);
        }

        [HttpPost]
        [Route(nameof(Train))]
        public IActionResult Train([FromBody] IEnumerable<TrainFaceModel> faceModels)
        {
            var existingIds = faceService.GetSaved().Select(f => f.Id);

            foreach (var newItem in faceModels.ExceptBy(existingIds.ToList(), f => f.Id))
            {
                if (cacheService.GetFace(newItem.Id) is byte[] cachedFace)
                    faceService.TrainFromImage(newItem, cachedFace);
            }

            foreach(var updatedItem in faceModels.IntersectBy(existingIds.ToList(), f => f.Id))
                faceService.UpdateName(updatedItem.Id, updatedItem.Name);

            foreach (var deletedItem in existingIds.ToList().Except(faceModels.Select(f => f.Id)))
                faceService.DeleteFace(deletedItem);

            return Ok();
        }

        [HttpPost]
        [Route(nameof(Detect))]
        public IActionResult Detect([FromBody] DetectFaceModel detectFaceModel)
        {
            var faces = faceService.DetectInImage(detectFaceModel.ImageData);

            switch (faces.Count())
            {
                case > 1: return BadRequest("Multiple faces detected");
                case < 1: return BadRequest("No faces detected");
            }

            var croppedFace = ImageHelpers.CropFaceInImage(detectFaceModel.ImageData, faces.First());

            cacheService.SetFace(detectFaceModel.Id, croppedFace);

            var thumbnail = ImageHelpers.GetThumbnail(croppedFace);

            return Content(thumbnail);
        }
    }
}