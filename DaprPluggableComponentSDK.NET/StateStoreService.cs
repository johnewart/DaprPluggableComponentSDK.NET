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
            var props = new Dictionary<string, string>();
            foreach (var k in request.Properties.Keys)
            {
                props[k] = request.Properties[k];
            }
            _logger.LogInformation("Initializing state store backend");
            _backend.Init(props);
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
            _backend.Delete(request.Key, int.Parse(request.Etag.Value));
            return base.Delete(request, context);
        }

        public override Task<GetResponse> Get(GetRequest request, ServerCallContext context)
        {
            _logger.LogDebug("Getting data in store for key {}", request.Key);

            var resp = new GetResponse();
           
            _backend.Get(request.Key).ContinueWith( it => {
                if (it.Result.HasValue)
                {
                    var obj = it.Result;
                    resp.Data = ByteString.CopyFrom(obj.Value.data);
                    resp.Etag = new Etag { Value = obj.Value.etag.ToString() };
                }
                else
                {
                    resp.Data = ByteString.Empty;
                    resp.Etag = null;
                }
            });

            foreach (var k in request.Metadata.Keys)
            {
                resp.Metadata[k] = request.Metadata[k];
            }
            
            return Task.FromResult(resp);
        }

        public override Task<Empty> Set(SetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Setting data in store for key {0}", request.Key);
            
            var obj = new StoreObject { data = request.Value.ToByteArray(), etag = -1 };

            _backend.Set(request.Key, obj);
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
                _backend.Delete(item.Key, int.Parse(item.Etag.Value));
            }

            return Task.FromResult(new Empty());
        }

        public override Task<BulkGetResponse> BulkGet(BulkGetRequest request, ServerCallContext context)
        {
            _logger.LogDebug("Bulk fetching data in store for {} keys", request.Items.Count);

            var response = new BulkGetResponse();
            foreach (var item in request.Items)
            {
                _backend.Get(item.Key).ContinueWith( it => {
                    if (it.Result.HasValue)
                    {
                        var obj = it.Result;
                        response.Items.Add(new BulkStateItem
                        {
                            Data = ByteString.CopyFrom(obj.Value.data),
                            Etag = new Etag { Value = obj.Value.etag.ToString() },
                            Key = item.Key,
                            Error = "none"
                        });
                    }
                    else
                    {
                        response.Items.Add(new BulkStateItem
                        {
                            Data = ByteString.Empty,
                            Etag = new Etag(),
                            Key = item.Key, 
                            Error = "KeyDoesNotExist"
                        });
                    }
                });
            }

            return Task.FromResult(response);
        }

        public override Task<Empty> BulkSet(BulkSetRequest request, ServerCallContext context)
        {
            _logger.LogDebug("Bulk storing data in store for {} keys", request.Items.Count);

            foreach (var item in request.Items)
            {
                Set(item, context);
            }

            return Task.FromResult(new Empty());
        }
}
