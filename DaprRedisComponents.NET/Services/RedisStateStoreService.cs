using DaprPluggableComponentSDK.NET;
using StackExchange.Redis;

namespace DaprRedisComponents.NET.Services;

public class RedisStateStoreService : StateStoreService
{
    private readonly ILogger<RedisStateStoreService> _logger;
    public RedisStateStoreService(ILogger<RedisStateStoreService> logger, ConnectionMultiplexer redis) : base(logger, new RedisStateStore(redis))
    {
        _logger = logger;
    }
}
