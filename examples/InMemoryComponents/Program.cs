using Dapr.PluggableComponents.AspNetCore;
using DaprInMemoryComponents.Components.HttpMiddleware;
using DaprInMemoryComponents.Components.InputBinding;
using DaprInMemoryComponents.Components.PubSub;
using DaprInMemoryComponents.Components.StateStore;

namespace DaprInMemoryComponents
{

    class InMemoryPluggableComponent
    {
        static void Main()
        {
            var service = new PluggableComponentService();
            service.WithStateStore<InMemoryStateStoreService>();
            service.WithHttpMiddleware<InMemoryHttpMiddlewareService>();
            service.WithInputBinding<InMemoryInputBindingService>();
            service.WithOutputBinding<InMemoryOutputBindingService>();
            service.WithPubSub<InMemoryPubSubService>();

            service.Run();
        }
    }

}