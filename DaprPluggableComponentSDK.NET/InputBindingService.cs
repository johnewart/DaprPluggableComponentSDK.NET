using Dapr.Proto.Components.V1;
using DaprPluggableComponentSDK.NET.Components;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace DaprPluggableComponentSDK.NET;

public class InputBindingService : InputBinding.InputBindingBase
{

    private readonly ILogger<InputBindingService> _logger;
    private readonly IInputBinding _backend;

    public InputBindingService(ILogger<InputBindingService> logger, IInputBinding backend)
    {
        this._logger = logger;
        this._backend = backend;
    }

    public override Task<Empty> Init(MetadataRequest request, ServerCallContext context)
    {
        _logger.LogDebug("Initializing input binding {0}", _backend.Name());
        this._backend.Init(new Dictionary<string, string>());
        return Task.FromResult(new Empty());
    }

    public override async Task Read(Empty request, IServerStreamWriter<ReadResponse> responseStream, ServerCallContext context)
    {
        while (true)
        {
            _logger.LogDebug("Going to read from input binding {0}", _backend.Name());
            ReadResponse resp = new ReadResponse();

            var result = _backend.Read();
            resp.Data = ByteString.CopyFrom(result.data);
            resp.Contenttype = result.contentType;
            await responseStream.WriteAsync(resp);
            _logger.LogDebug("Read operation sleeping ...");
            Thread.Sleep(10_000);
        }
    }
}
