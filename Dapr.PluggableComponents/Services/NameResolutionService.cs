using Dapr.Client.Autogen.Grpc.v1;
using Dapr.PluggableComponents.Components;
using Dapr.Proto.Components.V1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Dapr.PluggableComponents.Services;

public class NameResolutionService : NameResolution.NameResolutionBase
{
        private readonly ILogger<NameResolutionService> _logger;
        private readonly INameResolver _backend; 
        
        public NameResolutionService(ILogger<NameResolutionService> logger, INameResolver backend)
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

        public override Task<ResolveResponse> ResolveID(ResolveRequest request, ServerCallContext context)
        {
            var result = _backend.Lookup(request.Id);
            var response = new ResolveResponse
            {
                Answer = result,
            };

            return Task.FromResult(response); 
        }
}
