using MathGame.Services.Interface.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace MathGame.Services.Services;
public class CacheService : ICacheService
{
    private IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _cacheOptions = new DistributedCacheEntryOptions
    {
        SlidingExpiration = TimeSpan.FromMinutes(60)
    };

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public void Insert<T>(string key, T value, DistributedCacheEntryOptions options = null)
    {
        if(options == null)
        {
            options = _cacheOptions;
        }

        var serializedData = SerializeValue(value);
        _cache.Set(key, serializedData, options);
    }

    public T Get<T>(string key)
    {
        return DeserializeValue<T>(_cache.Get(key));
    }

    private byte[] SerializeValue<T>(T value)
    {
        if(value == null)
        {
            return null;
        }
        
        return JsonSerializer.SerializeToUtf8Bytes(value);
    }

    private T DeserializeValue<T>(byte[] value)
    {
        if(value == null)
        {
            return default;
        }
        
        return JsonSerializer.Deserialize<T>(value);
    }
}
