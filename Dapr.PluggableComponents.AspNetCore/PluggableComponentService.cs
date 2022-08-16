using Dapr.PluggableComponents.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Dapr.PluggableComponents.AspNetCore {
    public class PluggableComponentService
    {
        private readonly WebApplication _app;

        public PluggableComponentService(String? socketPath = null, WebApplicationOptions? options = null)
        {
            var udsPath = socketPath ??
                          Environment.GetEnvironmentVariable(Constants.DaprSocketPathEnvironmentVariable) ??
                          "daprcomponent.sock";
    
            Console.WriteLine("Starting Dapr pluggable component");
            Console.WriteLine(format: @"Using UNIX socket located at {0}", udsPath);
            
            if (File.Exists(udsPath))
            {
                Console.WriteLine("Removing existing socket");
                File.Delete(udsPath);
            }
            
            var builder = WebApplication.CreateBuilder(options: options ?? new WebApplicationOptions());
    
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenUnixSocket(udsPath);
            });
    
            builder.Services.AddGrpc();
            _app = builder.Build();
        }
    
        public PluggableComponentService WithStateStore<TService>() where TService : StateStoreService
        {
            _app.MapGrpcService<TService>();
            return this;
        }
        
        public PluggableComponentService WithPubSub<TService>() where TService : PubSubService
        {
            _app.MapGrpcService<TService>();
            return this;
        }
        
        public PluggableComponentService WithInputBinding<TService>() where TService : InputBindingService
        {
            _app.MapGrpcService<TService>();
            return this;
        }
        
        public PluggableComponentService WithOutputBinding<TService>() where TService : OutputBindingService
        {
            _app.MapGrpcService<TService>();
            return this;
        }
        
        public PluggableComponentService WithHttpMiddleware<TService>() where TService : HttpMiddlewareService
        {
            _app.MapGrpcService<TService>();
            return this;
        }
    
        public void Run(string? url = null)
        {
            _app.Run(url);
        }

        
    }
}