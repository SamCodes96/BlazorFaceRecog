using Microsoft.Extensions.Caching.Memory;

namespace BlazorFaceRecog.Server.Services
{
    public class CacheService(IMemoryCache cache)
    {
        public byte[]? GetFace(Guid id) => cache.Get<byte[]?>(id);

        public void SetFace(Guid id, byte[]? thumbnail)
        {
            cache.TryGetValue(id, out var _);

            cache.Set(id, thumbnail);
        }
    }
}