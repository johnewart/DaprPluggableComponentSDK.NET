namespace DaprRedisComponents.NET;

using DaprPluggableComponentSDK.NET.Components;
using StackExchange.Redis;
using Microsoft.Extensions.Logging; 

public class RedisStateStore : IStateStore
{
    private ConnectionMultiplexer _redis;

    public RedisStateStore(ConnectionMultiplexer redis)
    {
        _redis = redis; 
    }
    
    public void Init(Dictionary<string, string> props)
    {
    }

    public Task<StoreObject?> Get(string requestKey)
    {
        IDatabase db = _redis.GetDatabase();
        return db.StringGetAsync(requestKey).ContinueWith( it => {
            if (it.Result.IsNull) { 
                return new Nullable<StoreObject>();
            } else {
                return new StoreObject {
                   data = it.Result,
                   etag = 1,
                };
            }
        });
    }

    public List<string> Features()
    {
        return new List<string>();
    }

    public Task Delete(string requestKey, int etag)
    {
        IDatabase db = _redis.GetDatabase();
        return db.KeyDeleteAsync(requestKey);
    }

    public Task Set(string requestKey, StoreObject storeObject)
    {
        IDatabase db = _redis.GetDatabase();
        return db.StringSetAsync(requestKey, storeObject.data);
    }
}
