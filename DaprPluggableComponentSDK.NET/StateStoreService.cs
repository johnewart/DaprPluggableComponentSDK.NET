using Dapr.Client.Autogen.Grpc.v1;
using Dapr.Proto.Components.V1;
using DaprPluggableComponentSDK.NET.Components;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace DaprPluggableComponentSDK.NET;

public class StateStoreService : StateStore.StateStoreBase
{
        private readonly ILogger<StateStoreService> _logger;
        private readonly IStateStore _backend; 
        
        public StateStoreService(ILogger<StateStoreService> logger, IStateStore backend)
        {
            this._logger = logger;
            this._backend = backend; 
        }

        public override Task<Empty> Init(MetadataRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Initializing state store backend");
            _backend.Init(request.Properties);
            return Task.FromResult(new Empty());
        }

        public override Task<FeaturesResponse> Features(Empty request, ServerCallContext context)
        {
            List<String> unused = _backend.Features();
            var resp = new FeaturesResponse();
            return Task.FromResult(resp);
        }

        public override Task<Empty> Delete(DeleteRequest request, ServerCallContext context)
        {
            _logger.LogDebug("Deleting data in store for key {}", request.Key);
            _backend.Delete(request.Key, request.Options, request.Etag);
            return base.Delete(request, context);
        }

        public override Task<GetResponse> Get(GetRequest request, ServerCallContext context)
        {
            _logger.LogDebug("Getting data in store for key {}", request.Key);

            var obj = _backend.Get(request.Key); //, request.Consistency, request.Metadata);
            var resp = new GetResponse
            {
                Data = ByteString.CopyFrom(obj.data),
                Etag = new Etag { Value = obj.etag.ToString() },
            };
            foreach (var k in request.Metadata.Keys)
            {
                resp.Metadata[k] = request.Metadata[k];
            }
            
            return Task.FromResult(resp);
        }

        public override Task<Empty> Set(SetRequest request, ServerCallContext context)
        {
            _logger.LogDebug("Setting data in store for key {}", request.Key);

            _backend.Set(request.Key, request.Value, request.Etag);
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Ping(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> BulkDelete(BulkDeleteRequest request, ServerCallContext context)
        {
            _logger.LogDebug("Bulk deleting data in store for {} keys", request.Items.Count);

            foreach (var item in request.Items)
            {
                _backend.Delete(item.Key, item.Options, item.Etag);
            }

            return Task.FromResult(new Empty());
        }

        public override Task<BulkGetResponse> BulkGet(BulkGetRequest request, ServerCallContext context)
        {
            _logger.LogDebug("Bulk fetching data in store for {} keys", request.Items.Count);
            var response = new BulkGetResponse();
            foreach (var item in request.Items)
            {
                var obj = _backend.Get(item.Key);

                response.Items.Add(new BulkStateItem
                {
                    Data = ByteString.CopyFrom(obj.data),
                    Etag = new Etag { Value = obj.etag.ToString() },
                    Key = item.Key,
                    Error = "none"
                });
            }

            return Task.FromResult(response);
        }

        public override Task<Empty> BulkSet(BulkSetRequest request, ServerCallContext context)
        {
            _logger.LogDebug("Bulk storing data in store for {} keys", request.Items.Count);

            foreach (var item in request.Items)
            {
                _backend.Set(item.Key, item.Value, item.Etag);
            }
            return base.BulkSet(request, context);
        }
}
