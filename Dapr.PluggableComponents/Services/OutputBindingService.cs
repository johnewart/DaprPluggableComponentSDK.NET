using Dapr.Client.Autogen.Grpc.v1;
using Dapr.PluggableComponents.Components;
using Dapr.Proto.Components.V1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using InvokeRequest = Dapr.Proto.Components.V1.InvokeRequest;
using InvokeResponse = Dapr.Proto.Components.V1.InvokeResponse;

namespace Dapr.PluggableComponents.Services;

public class OutputBindingService : OutputBinding.OutputBindingBase {

    private readonly ILogger<OutputBindingService> _logger; 
    private readonly IOutputBinding _backend; 

    public OutputBindingService(ILogger<OutputBindingService> logger, IOutputBinding backend) {
        this._logger = logger; 
        this._backend = backend;
    }

    public override Task<Empty> Init(MetadataRequest request, ServerCallContext context) {
        this._backend.Init(Utils.ConvertMetadata(request.Properties));
        return Task.FromResult(new Empty()); 
    }

    public override Task<InvokeResponse> Invoke(InvokeRequest request, ServerCallContext context) {
        _logger.LogDebug("Going to invoke output binding {0}", _backend.Name());
        
        byte[] data = request.Data.ToArray();
        string operation = request.Operation; 
        var metadata = Utils.ConvertMetadata(request.Metadata);

        var result = _backend.Invoke(operation, data, metadata);
        
        InvokeResponse resp = new InvokeResponse(); 
        resp.Data = ByteString.CopyFrom(result.data);
        resp.Contenttype = result.contentType; 
        Utils.MergeDictionaryIntoMetadata(result.metadata, resp.Metadata);

        return Task.FromResult(resp);
    }
}
