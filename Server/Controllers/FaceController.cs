using BlazorFaceRecog.Server.Services;
using BlazorFaceRecog.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFaceRecog.Server.Controllers
{
    [ApiController]
    public class FaceController(FaceService faceService) : ControllerBase
    {
        [HttpPost]
        [Route(nameof(TrainFace))]
        public IActionResult TrainFace([FromBody]TrainFaceData trainFaceData)
        {
            var faces = faceService.DetectInImage(trainFaceData.ImageData);

            if (faces.Length != 1) return BadRequest();

            faceService.TrainFromImage(trainFaceData, faces[0]);

            return Ok();
        }

        [HttpPost]
        [Route(nameof(GetFace))]
        public IActionResult GetFace([FromBody]byte[] imageData)
        {
            var faces = faceService.DetectInImage(imageData);

            if (faces.Length != 1) return BadRequest();

            var croppedFace = faceService.CropFaceInImage(imageData, faces[0]);

            return Content(Convert.ToBase64String(croppedFace));
        }
    }
}
