namespace DaprRedisComponents.NET;

using DaprPluggableComponentSDK.NET.Components;
using StackExchange.Redis;

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
        var result = db.StringGet(requestKey);
        if (!result.HasValue)
        {
            return Task.FromResult(new Nullable<StoreObject>());
        }
        else
        {
            StoreObject? so = new StoreObject
            {
                data = result,
                etag = 1,
            };
            return Task.FromResult(so);
        }
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
        db.StringSet(requestKey, storeObject.data);
        return Task.FromResult(true);
    }
}
