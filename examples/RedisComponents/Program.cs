using Dapr.PluggableComponents.AspNetCore;
using DaprRedisComponents.Components.StateStore;

namespace DaprRedisComponents
{

    class InMemoryPluggableComponent
    {
        static void Main()
        {
            var service = new PluggableComponentService();
            service.WithStateStore<RedisStateStoreService>();
            service.Run();
        }
    }
}