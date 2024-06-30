using Microsoft.Extensions.Caching.Memory;

namespace BlazorFaceRecog.Server.Services
{
    public class CacheService(IMemoryCache cache)
    {
        public void AddFace(Guid id, byte[] data) => cache.Set(id, data);
        public byte[]? GetFace(Guid id) => cache.Get<byte[]>(id);
    }
}
