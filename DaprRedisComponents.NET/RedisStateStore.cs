namespace DaprRedisComponents.NET;

using DaprPluggableComponentSDK.NET.Components;
using StackExchange.Redis;

public class RedisStateStore : IStateStore
{
    private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");

    public RedisStateStore()
    {
    }
    
    public void Init(Dictionary<string, string> props)
    {
    }

    public Task<StoreObject?> Get(string requestKey)
    {
        IDatabase redisDb = redis.GetDatabase();
        return redisDb.StringGetAsync(requestKey).ContinueWith( it => {
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
        IDatabase redisDb = redis.GetDatabase();
        return redisDb.KeyDeleteAsync(requestKey);
    }

    public Task Set(string requestKey, StoreObject storeObject)
    {
        IDatabase redisDb = redis.GetDatabase();
        return redisDb.StringSetAsync(requestKey, storeObject.data);
    }
}
