using Dapr.PluggableComponents.Services;

namespace DaprInMemoryComponents.Components.StateStore;

public class InMemoryStateStoreService : StateStoreService
{
    private readonly ILogger<InMemoryStateStoreService> _logger;
    public InMemoryStateStoreService(ILogger<InMemoryStateStoreService> logger) : base(logger, new InMemoryStateStore())
    {
        _logger = logger;
    }
}
