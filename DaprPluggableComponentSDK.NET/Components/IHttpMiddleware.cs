namespace DaprPluggableComponentSDK.NET.Components;

using Microsoft.AspNetCore.Http; 

public struct MiddlewareCapabilities {
    public bool HandlesHeader { get; set; }
    public bool HandlesBody { get; set; }
}

public struct MiddlewareResult {
    public HttpRequest? request { get; set; }
    public HttpResponse? response { get; set; }
}

public interface IHttpMiddleware {
    public MiddlewareCapabilities Init(Dictionary<string, string> parameters);
    public MiddlewareResult HandleHeader(HttpRequest request);
    public MiddlewareResult HandleBody(HttpRequest request); 
}
