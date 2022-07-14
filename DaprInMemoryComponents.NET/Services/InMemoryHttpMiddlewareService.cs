using DaprPluggableComponentSDK.NET;

namespace DaprInMemoryStateStore.NET.Services;

public class InMemoryHttpMiddlewareService : HttpMiddlewareService
{
    private readonly ILogger<InMemoryHttpMiddlewareService> _logger;
    public InMemoryHttpMiddlewareService(ILogger<InMemoryHttpMiddlewareService> logger) : base(logger, new InMemoryHttpMiddleware())
    {
        _logger = logger;
    }
}
