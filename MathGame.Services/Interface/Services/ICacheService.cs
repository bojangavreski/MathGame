using Microsoft.Extensions.Caching.Distributed;

namespace MathGame.Services.Interface.Services;
public interface ICacheService
{
    void Insert<T>(string key, T value, DistributedCacheEntryOptions options = null);

    T Get<T>(string key);
}
