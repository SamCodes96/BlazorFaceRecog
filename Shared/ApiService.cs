namespace BlazorFaceRecog.Shared;

public partial record SavedFaceModel(Guid Id, string Name, string Thumbnail);

public partial record TrainFaceModel(Guid Id, string Name);

public interface IApiService
{
    Task<string> DetectFacesAsync(FileParameter file, Guid id);
    Task<ICollection<SavedFaceModel>> GetSavedFacesAsync();
    Task TrainFacesAsync(IEnumerable<TrainFaceModel> models);
}

public class ApiService(HttpClient httpClient) : IApiService
{
    private readonly ApiServiceClient _client = new(httpClient);

    public Task<string> DetectFacesAsync(FileParameter file, Guid id) => _client.DetectFacesAsync(file, id);

    public Task<ICollection<SavedFaceModel>> GetSavedFacesAsync() => _client.GetSavedFacesAsync();

    public Task TrainFacesAsync(IEnumerable<TrainFaceModel> models) => _client.TrainFacesAsync(models);
}