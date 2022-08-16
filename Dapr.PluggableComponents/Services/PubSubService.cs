using Dapr.Client.Autogen.Grpc.v1;
using Dapr.PluggableComponents.Components;
using Dapr.Proto.Components.V1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Dapr.PluggableComponents.Services;
public class PubSubService : PubSub.PubSubBase   {

    private readonly ILogger<PubSubService> _logger; 
    private readonly IPubSubComponent _backend; 

    public PubSubService(ILogger<PubSubService> logger, IPubSubComponent backend) {
        this._logger = logger; 
        this._backend = backend;
    }

    public override Task<Empty> Init(MetadataRequest request, ServerCallContext context) {
        this._backend.Init(new Dictionary<string, string>());
        return Task.FromResult(new Empty()); 
    }

    public override Task<FeaturesResponse> Features(Empty request, ServerCallContext context)
    {
        List<String> unused = _backend.Features();
        var resp = new FeaturesResponse();
        return Task.FromResult(resp);
    }


    public override Task<Empty> Publish(PublishRequest request, ServerCallContext context) {
        _logger.LogDebug("Going to publish a message to {0}", request.Topic);
        var m = new PubSubMessage {
            Data = request.Data.ToByteArray(),
            Topic = request.Topic 
        };

        _backend.Publish(request.Topic, m);
        return Task.FromResult(new Empty());
    }

    public override async Task Subscribe(SubscribeRequest request, IServerStreamWriter<NewMessage> responseStream, ServerCallContext context) {

        while(true) {
            var msgQueue = _backend.Subscribe(request.Topic);
            _logger.LogDebug("There are {0} messages waiting in the queue for {1}", msgQueue.Count, request.Topic);
            while (msgQueue.Count > 0) {
                var msg = msgQueue.Dequeue(); 
                _logger.LogDebug("Delivering a message to client on topic {0} ...", request.Topic);
                var pMsg = new NewMessage {
                     Data = ByteString.CopyFrom(msg.Data),
                     Topic = request.Topic,
                };
                await responseStream.WriteAsync(pMsg);
            }
            _logger.LogDebug("Subscription for {0} sleeping...", request.Topic);
            Thread.Sleep(10_000);
        }
    }
}
