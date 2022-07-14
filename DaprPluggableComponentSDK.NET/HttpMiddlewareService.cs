using Dapr.Proto.Components.V1;
using DaprPluggableComponentSDK.NET.Components;
using Microsoft.AspNetCore.Http;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace DaprPluggableComponentSDK.NET;
public class HttpMiddlewareService : HttpMiddleware.HttpMiddlewareBase   {

    private readonly ILogger<HttpMiddlewareService> _logger; 
    private readonly IHttpMiddleware _backend; 

    public HttpMiddlewareService(ILogger<HttpMiddlewareService> logger, IHttpMiddleware backend) {
        this._logger = logger; 
        this._backend = backend;
    }

    public override Task<HandlerResponse> Init(MetadataRequest request, ServerCallContext context) {
        this._backend.Init(new Dictionary<string, string>());
        return Task.FromResult(new HandlerResponse()); 
    }

    public override Task<HeaderHandlerResponse> HandleHeader(HttpRequestHeader header, ServerCallContext context) {
        var ctx = new DefaultHttpContext();
        ctx.Request.Method = header.Method;
            foreach (var k in header.Headers.Keys)
            {
                ctx.Request.Headers.Append(k, header.Headers[k]);
            }
        
        var result = _backend.HandleHeader(ctx.Request);

        if (result.request != null) { 
            HttpRequestHeader h  = new HttpRequestHeader {
                Method = result.request.Method, 
                Uri = result.request.Path,
            };
            foreach (var k in result.request.Headers.Keys) {
                h.Headers[k] = result.request.Headers[k];
            }
            return Task.FromResult(new HeaderHandlerResponse {
                RequestHeader = h,
            });
        }

        return Task.FromResult(new HeaderHandlerResponse());
    }

    public override Task<BodyHandlerResponse> HandleBody(HttpRequestBody body, ServerCallContext context) {
        return Task.FromResult(new BodyHandlerResponse());
    }

    public override Task<Empty> Ping(Empty request, ServerCallContext context)
    {
        return base.Ping(request, context);
    }
    
    

}
