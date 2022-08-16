using Dapr.Client.Autogen.Grpc.v1;
using Dapr.PluggableComponents.Components;
using Dapr.Proto.Components.V1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dapr.PluggableComponents.Services;
public class HttpMiddlewareService : HttpMiddleware.HttpMiddlewareBase   {

    private readonly ILogger<HttpMiddlewareService> _logger; 
    private readonly IHttpMiddleware _backend; 

    public HttpMiddlewareService(ILogger<HttpMiddlewareService> logger, IHttpMiddleware backend) {
        this._logger = logger; 
        this._backend = backend;
    }

    public override Task<HandlerResponse> Init(MetadataRequest request, ServerCallContext context) {
        var result = this._backend.Init(new Dictionary<string, string>());
        return Task.FromResult(new HandlerResponse {
            HeaderHandler = result.HandlesHeader,
            BodyHandler = result.HandlesBody
        });
    }

    public override Task<HeaderHandlerResponse> HandleHeader(HttpRequestHeader header, ServerCallContext context) {
        var ctx = new DefaultHttpContext();
        ctx.Request.Method = header.Method;
            foreach (var k in header.Headers.Keys)
            {
                ctx.Request.Headers.Append(k, header.Headers[k]);
            }
        
        var result = _backend.HandleHeader(ctx.Request);

        var response = new HeaderHandlerResponse();
        
        if (result.request != null)
        {
            HttpRequestHeader h = new HttpRequestHeader
            {
                Method = result.request.Method,
                Uri = result.request.Path,
            };
            foreach (var k in result.request.Headers.Keys)
            {
                h.Headers[k] = result.request.Headers[k];
            }

            response.RequestHeader = h;
        }

        if (result.response != null)
        {
            var h = new HttpResponseHeader();
            
            foreach (var pair in result.response.Headers)
            {
                foreach (var v in pair.Value)
                {
                    h.Headers[pair.Key] = v;
                }
            }

            h.ResponseCode = (long)result.response.StatusCode;
            response.ResponseHeader = h; 
        }

        return Task.FromResult(response);

    }

    public override Task<BodyHandlerResponse> HandleBody(HttpRequestBody body, ServerCallContext context) {
        return Task.FromResult(new BodyHandlerResponse());
    }

    public override Task<Empty> Ping(Empty request, ServerCallContext context)
    {
        return base.Ping(request, context);
    }
    
    

}
