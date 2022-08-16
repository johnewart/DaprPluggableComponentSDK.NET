using Dapr.Client.Autogen.Grpc.v1;
using Dapr.PluggableComponents.Components;
using Dapr.PluggableComponents.Data;
using Dapr.Proto.Components.V1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Dapr.PluggableComponents.Services;

public class ConfigurationStoreService : ConfigurationService.ConfigurationServiceBase
{
        private readonly ILogger<ConfigurationStoreService> _logger;
        private readonly IConfigurationStore _backend; 
        
        public ConfigurationStoreService(ILogger<ConfigurationStoreService> logger, IConfigurationStore backend)
        {
            this._logger = logger;
            this._backend = backend; 
        }

        public override Task<Empty> Init(MetadataRequest request, ServerCallContext context)
        {
            var props = new Dictionary<string, string>();
            foreach (var k in request.Properties.Keys)
            {
                props[k] = request.Properties[k];
            }
            _logger.LogInformation("Initializing state store backend");
            _backend.Init(props);
            return Task.FromResult(new Empty());
        }

        public override Task<GetConfigurationResponse> Get(GetConfigurationRequest request, ServerCallContext context)
        {
            var metadata = request.Metadata.ToDictionary();
            var keys = request.Keys.ToList();
            var result = _backend.Get(keys, metadata);
            var response = new GetConfigurationResponse();
            foreach (var item in result.Items)
            {
                response.Items.Add(new Item
                {
                    Key = item.Key,
                    Value = item.Data
                });
            }

            return Task.FromResult(response);
        }

        public override async Task Subscribe(ConfigurationSubscribeRequest request, IServerStreamWriter<ConfigurationUpdateEvent> responseStream, ServerCallContext context)
        {
            var metadata = request.Metadata.ToDictionary();
            var keys = request.Keys.ToList();

            while(true) {
                var msgQueue = _backend.Subscribe(keys, metadata);
                _logger.LogDebug("There are {0} messages waiting in the configuration subscription queue", msgQueue.Count);
                while (msgQueue.Count > 0) {
                    var msg = msgQueue.Dequeue(); 
                    _logger.LogDebug("Delivering a message to client for configuration subscription {0}...", msg.SubscriptionId);
                    
                    var cMsg = new ConfigurationUpdateEvent();
                    cMsg.Id = msg.SubscriptionId;
                    foreach (var item in msg.Items)
                    {
                        var cItem = new Item();
                        cItem.Key = item.Key;
                        cItem.Value = item.Data;
                        cMsg.Items.Add(cItem);
                    }
                    
                    await responseStream.WriteAsync(cMsg);
                }
                
                _logger.LogDebug("Configuration subscription is sleeping...");
                Thread.Sleep(10_000);
            }
        }

        public override Task<Empty> Unsubscribe(ConfigurationUnsubscribeRequest request, ServerCallContext context)
        {
            _backend.Unsubscribe(request.Id);
            return Task.FromResult(new Empty());
        }
}
