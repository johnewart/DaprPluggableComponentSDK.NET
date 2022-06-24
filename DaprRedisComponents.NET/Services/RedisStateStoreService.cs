using DaprPluggableComponentSDK.NET;

namespace DaprRedisComponents.NET.Services;

public class RedisStateStoreService : StateStoreService
{
    private readonly ILogger<RedisStateStoreService> _logger;
    public RedisStateStoreService(ILogger<RedisStateStoreService> logger) : base(logger, new RedisStateStore())
    {
        _logger = logger;
    }
}
