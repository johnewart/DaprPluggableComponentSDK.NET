using Dapr.Client.Autogen.Grpc.v1;
using Dapr.PluggableComponents.Components;
using Dapr.Proto.Components.V1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Dapr.PluggableComponents.Services;

public class LockService : LockStore.LockStoreBase
{
        private readonly ILogger<LockService> _logger;
        private readonly ILockStore _backend; 
        
        public LockService(ILogger<LockService> logger, ILockStore backend)
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
            _logger.LogInformation("Initializing lock store service");
            _backend.Init(props);
            return Task.FromResult(new Empty());
        }

        public override Task<TryLockResponse> TryLock(TryLockRequest request, ServerCallContext context)
        {
            var result = _backend.TryLock(request.LockOwner, request.ResourceId, request.ExpiryInSeconds);
            return Task.FromResult(new TryLockResponse { Success = result.Success });
        }

        public override Task<UnlockResponse> Unlock(UnlockRequest request, ServerCallContext context)
        {
            var result = _backend.Unlock(request.LockOwner, request.ResourceId);
            var status = result.Success ? UnlockStatus.Success : UnlockStatus.InternalError;
            return Task.FromResult(new UnlockResponse { Status = status });
        }
}
