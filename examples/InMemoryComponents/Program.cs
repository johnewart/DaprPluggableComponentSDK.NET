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
            var builder = PluggableComponentServiceBuilder.CreateBuilder();
            var singletonInMemoryStateStore = new InMemoryStateStore();
            builder
                .UseStateStore(() => singletonInMemoryStateStore)
                .UseHttpMiddleware(() => new InMemoryHttpMiddleware())
                .UseInputBinding(() => new InMemoryBinding())
                .UseOutputputBinding(() => new InMemoryBinding())
                .UsePubSub(() => new InMemoryPubSubComponent())
                .Run();
        }
    }

}