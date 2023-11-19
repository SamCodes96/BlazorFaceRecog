using Microsoft.AspNetCore.Mvc;

namespace BlazorFaceRecog.Server.Controllers;

public class ImageController : ControllerBase
{
    // TODO: This can be replaced in the frontend when blazor wasm finally supports multithreading...
    // In the mean time can maybe look at compressing image first?
    [HttpPost]
    [Route("ConvertImage")]
    public async Task<string> ConvertImageToBase64()
    {
        await using var ms = new MemoryStream();
        await Request.Body.CopyToAsync(ms);

        return Convert.ToBase64String(ms.ToArray());
    }
}