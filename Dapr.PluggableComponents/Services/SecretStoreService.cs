using Dapr.Client.Autogen.Grpc.v1;
using Dapr.PluggableComponents.Components;
using Dapr.PluggableComponents.Data;
using Dapr.Proto.Components.V1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Dapr.PluggableComponents.Services;

public class SecretStoreService : SecretStore.SecretStoreBase
{
        private readonly ILogger<SecretStoreService> _logger;
        private readonly ISecretStore _backend; 
        
        public SecretStoreService(ILogger<SecretStoreService> logger, ISecretStore backend)
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
            _logger.LogInformation("Initializing secret store backend");
            _backend.Init(props);
            return Task.FromResult(new Empty());
        }

        public override Task<GetSecretResponse> GetSecret(GetSecretRequest request, ServerCallContext context)
        {
            var metadata = Utils.ConvertMetadata(metadata: request.Metadata);
            Dictionary<String, String> result = _backend.GetSecret(request.Name, metadata);
            var response = new GetSecretResponse();
            result.CopyToMapField(response.Data);
            return Task.FromResult(response);
        }

        public override Task<BulkGetSecretResponse> BulkGetSecret(BulkGetSecretRequest request, ServerCallContext context)
        {
            var response = new BulkGetSecretResponse();
            foreach (var name in request.Metadata.Keys)
            {
                Dictionary<String, String> secrets = _backend.GetSecret(name, new Dictionary<string, string>());
                var result = new GetSecretResponse();
                secrets.CopyToMapField(result.Data);
                response.Data[name] = result; 
                
            }

            return Task.FromResult(response);
        }

        public override Task<Empty> Ping(Empty request, ServerCallContext context)
        {
            _backend.Ping();
            return Task.FromResult(new Empty());
        }
}
