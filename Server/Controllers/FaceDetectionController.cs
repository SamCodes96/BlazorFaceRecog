using BlazorFaceRecog.Server.Services;
using BlazorFaceRecog.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFaceRecog.Server.Controllers
{
    [ApiController]
    public class FaceDetectionController(FaceService faceService) : ControllerBase
    {
        [HttpPost]
        [Route(nameof(TrainFace))]
        public IActionResult TrainFace([FromBody]TrainFaceData trainFaceData)
        {
            var faces = faceService.DetectInImage(trainFaceData.GetImageData());

            if (faces.Length != 1) return BadRequest();

            faceService.TrainFromImage(trainFaceData, faces[0]);

            return Ok();
        }
    }
}
