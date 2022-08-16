using Dapr.PluggableComponents.Services;
using StackExchange.Redis;

namespace DaprRedisComponents.Components.StateStore;

public class RedisStateStoreService : StateStoreService
{
    private readonly ILogger<RedisStateStoreService> _logger;
    public RedisStateStoreService(ILogger<RedisStateStoreService> logger, ConnectionMultiplexer redis) : base(logger, new RedisStateStore(redis))
    {
        _logger = logger;
    }
}
