using System.Linq;
using BlazorFaceRecog.Server.Helpers;
using BlazorFaceRecog.Server.Services;
using BlazorFaceRecog.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFaceRecog.Server.Controllers
{
    [ApiController]
    public class FaceController(FaceService faceService, CacheService cacheService) : ControllerBase
    {
        [HttpGet]
        [Route(nameof(GetSavedFaces))]
        public IActionResult GetSavedFaces()
        {
            var saved = faceService.GetSavedFaces();

            return Ok(saved);
        }

        [HttpPost]
        [Route(nameof(TrainFaces))]
        public IActionResult TrainFaces([FromBody] TrainFaceModel[] faceModels)
        {
            var existing = faceService.GetSavedFaces().ToArray();

            var existingIds = existing.Select(x => x.Id).ToArray();
            var newIds = faceModels.Select(x => x.Id).ToArray();

            foreach (var newItem in faceModels.Where(fm => !Array.Exists(existingIds, e => e == fm.Id)))
            {
                var cachedFace = cacheService.GetFace(newItem.Id);
                if (cachedFace == null)
                    break;

                faceService.TrainFromImage(newItem, cachedFace);
            }

            foreach (var deletedItem in existingIds.Where(e => !Array.Exists(newIds, fm => fm == e)))
                faceService.DeleteFace(deletedItem);
         
            foreach(var updatedItem in faceModels.Where(e => Array.Exists(existing, fm => fm.Id == e.Id && fm.Name != e.Name)))
                faceService.UpdateName(updatedItem.Id, updatedItem.Name);

            return Ok();
        }

        [HttpPost]
        [Route(nameof(DetectFace))]
        public IActionResult DetectFace([FromBody] DetectFaceModel detectFaceModel)
        {
            var faces = faceService.DetectInImage(detectFaceModel.ImageData);

            switch (faces.Length)
            {
                case > 1: return BadRequest("Multiple faces detected");
                case < 1: return BadRequest("No faces detected");
            }

            var croppedFace = faceService.CropFaceInImage(detectFaceModel.ImageData, faces[0]);

            cacheService.SetFace(detectFaceModel.Id, croppedFace);

            var thumbnail = ImageHelpers.GetThumbnail(croppedFace);

            return Content(thumbnail);
        }
    }
}
