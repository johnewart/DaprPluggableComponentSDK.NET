using DaprPluggableComponentSDK.NET;

namespace DaprInMemoryStateStore.NET.Services;

public class InMemoryPubSubService : PubSubService
{
    private readonly ILogger<InMemoryPubSubService> _logger;
    public InMemoryPubSubService(ILogger<InMemoryPubSubService> logger) : base(logger, new InMemoryPubSubComponent())
    {
        _logger = logger;
    }
}
