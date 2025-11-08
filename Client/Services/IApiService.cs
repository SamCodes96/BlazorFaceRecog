using BlazorFaceRecog.Shared;
using Refit;

namespace BlazorFaceRecog.Client.Services;

public interface IApiService
{
    [Get("/Faces")]
    Task<IApiResponse<IEnumerable<SavedFaceModel>>> GetSavedFacesAsync();

    [Post("/Faces/Train")]
    Task<IApiResponse> TrainFacesAsync(IEnumerable<TrainFaceModel> models);

    [Multipart]
    [Post("/Faces/Detect")]
    Task<IApiResponse<string>> DetectFacesAsync(string id, [AliasAs("file")]StreamPart stream);
}
