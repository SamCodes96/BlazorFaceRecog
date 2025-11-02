using Microsoft.Extensions.Caching.Memory;

namespace BlazorFaceRecog.Server.Services;

public interface IFaceCache
{
    byte[]? GetFace(Guid id);
    void SetFace(Guid id, byte[]? thumbnail);
}

public class FaceCache(IMemoryCache cache) : IFaceCache
{
    public byte[]? GetFace(Guid id) => cache.Get<byte[]?>(id);

    public void SetFace(Guid id, byte[]? thumbnail)
    {
        cache.TryGetValue(id, out var _);

        cache.Set(id, thumbnail);
    }
}