using DaprPluggableComponentSDK.NET;
using DaprInMemoryComponents.NET;

namespace DaprInMemoryStateStore.NET.Services;

public class InMemoryInputBindingService : InputBindingService
{
    private readonly ILogger<InMemoryInputBindingService> _logger;
    public InMemoryInputBindingService(ILogger<InMemoryInputBindingService> logger) : base(logger, new InMemoryBinding())
    {
        _logger = logger;
    }
}

public class InMemoryOutputBindingService : OutputBindingService 
{
    private readonly ILogger<InMemoryOutputBindingService> _logger;
    public InMemoryOutputBindingService(ILogger<InMemoryOutputBindingService> logger) : base(logger, new InMemoryBinding())
    {
        _logger = logger;
    }
}
