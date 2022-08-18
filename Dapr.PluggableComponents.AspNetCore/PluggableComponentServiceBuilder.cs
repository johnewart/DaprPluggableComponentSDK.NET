using Dapr.PluggableComponents.Components;
using Dapr.PluggableComponents.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Dapr.PluggableComponents.AspNetCore
{
    public class PluggableComponentServiceBuilder
    {
        private List<Action<WebApplication>> appCallbacks;
        private List<Action<WebApplicationBuilder>> builderCallbacks;

        private PluggableComponentServiceBuilder()
        {
            appCallbacks = new List<Action<WebApplication>>();
            builderCallbacks = new List<Action<WebApplicationBuilder>>();
        }
        private PluggableComponentServiceBuilder(PluggableComponentServiceBuilder other) : this()
        {
            this.appCallbacks = new List<Action<WebApplication>>(other.appCallbacks);
            this.builderCallbacks = new List<Action<WebApplicationBuilder>>(other.builderCallbacks);
        }

        public static PluggableComponentServiceBuilder CreateBuilder(string? socketPath = null)
        {
            var udsPath =
                socketPath
                    ?? Environment
                        .GetEnvironmentVariable(Constants
                            .DaprSocketPathEnvironmentVariable)
                        ?? "daprcomponent.sock";

            Console.WriteLine("Starting Dapr pluggable component");
            Console
                .WriteLine(format: @"Using UNIX socket located at {0}",
                udsPath);

            if (File.Exists(udsPath))
            {
                Console.WriteLine("Removing existing socket");
                File.Delete(udsPath);
            }
            return new PluggableComponentServiceBuilder().UseSocket(udsPath).WithBuilderCallback(builder =>
            {
                builder.Services.AddGrpc();
            });
        }

        public PluggableComponentServiceBuilder UseSocket(string socketPath)
        {
            return this.WithBuilderCallback(builder =>
            {
                builder
                .WebHost
                .ConfigureKestrel(options =>
                {
                    options.ListenUnixSocket(socketPath);
                });
            });
        }

        public PluggableComponentServiceBuilder UseStateStore(Func<IStateStore> stateStoreFactory)
        {
            return this.UseServiceFactory<IStateStore, StateStoreService>(stateStoreFactory);
        }

        public PluggableComponentServiceBuilder UseLockStore(Func<ILockStore> lockStoreFactory)
        {
            return this.UseServiceFactory<ILockStore, LockService>(lockStoreFactory);
        }

        public PluggableComponentServiceBuilder UseInputBinding(Func<IInputBinding> inputBindingFactory)
        {
            return this.UseServiceFactory<IInputBinding, InputBindingService>(inputBindingFactory);
        }

        public PluggableComponentServiceBuilder UseOutputputBinding(Func<IOutputBinding> outputBindingFactory)
        {
            return this.UseServiceFactory<IOutputBinding, OutputBindingService>(outputBindingFactory);
        }
        public PluggableComponentServiceBuilder UseNameResolver(Func<INameResolver> nameResolverFactory)
        {
            return this.UseServiceFactory<INameResolver, NameResolutionService>(nameResolverFactory);
        }
        public PluggableComponentServiceBuilder UsePubSub(Func<IPubSubComponent> pubSubFactory)
        {
            return this.UseServiceFactory<IPubSubComponent, PubSubService>(pubSubFactory);
        }
        public PluggableComponentServiceBuilder UseHttpMiddleware(Func<IHttpMiddleware> httpMiddlewareFactory)
        {
            return this.UseServiceFactory<IHttpMiddleware, HttpMiddlewareService>(httpMiddlewareFactory);
        }

        public PluggableComponentServiceBuilder UseConfigurationStore(Func<IConfigurationStore> configurationStoreFactory)
        {
            return this.UseServiceFactory<IConfigurationStore, ConfigurationStoreService>(configurationStoreFactory);
        }

        public PluggableComponentServiceBuilder UseSecretStore(Func<ISecretStore> secretStoreFactory)
        {
            return this.UseServiceFactory<ISecretStore, SecretStoreService>(secretStoreFactory);
        }

        private PluggableComponentServiceBuilder UseServiceFactory<TImpl, TService>(Func<TImpl> factory) where TImpl : class where TService : class
        {
            return this.AsScopped<TImpl>(factory).WithAppCallback(WithGrpcService<TService>());
        }

        private PluggableComponentServiceBuilder AsScopped<T>(Func<T> factory) where T : class
        {
            return this.WithBuilderCallback(app =>
            {
                app.Services.AddScoped<T>((_) => factory());
            });
        }

        private PluggableComponentServiceBuilder WithBuilderCallback(Action<WebApplicationBuilder> callback)
        {
            return new PluggableComponentServiceBuilder(this)
            {
                builderCallbacks = new List<Action<WebApplicationBuilder>>(this.builderCallbacks) { callback }
            };
        }

        private PluggableComponentServiceBuilder WithAppCallback(Action<WebApplication> callback)
        {
            return new PluggableComponentServiceBuilder(this)
            {
                appCallbacks = new List<Action<WebApplication>>(this.appCallbacks) { callback }
            };
        }

        private static Action<WebApplication> WithGrpcService<T>() where T : class
        {
            return (app) =>
            {
                app.MapGrpcService<T>();
            };
        }

        public void Run(string? url = null)
        {
            var builder = WebApplication.CreateBuilder();
            builderCallbacks.ForEach(callback => callback(builder));
            var app = builder.Build();
            appCallbacks.ForEach(callback => callback(app));
            app.Run(url);
        }
    }
}
