using BlazorFaceRecog.Server.Services;
using BlazorFaceRecog.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFaceRecog.Server.Controllers
{
    [ApiController]
    public class FaceController(FaceService faceService, CacheService cacheService) : ControllerBase
    {
        [HttpPost]
        [Route(nameof(TrainFaces))]
        public IActionResult TrainFaces([FromBody] TrainFaceModel[] faceModels)
        {
            foreach (var face in faceModels)
            {
                var cachedFace = cacheService.GetFace(face.Id);
                if (cachedFace == null)
                    break;

                faceService.TrainFromImage(face, cachedFace);
            }

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

            cacheService.AddFace(detectFaceModel.Id, croppedFace);

            return Content(Convert.ToBase64String(croppedFace));
        }
    }
}
