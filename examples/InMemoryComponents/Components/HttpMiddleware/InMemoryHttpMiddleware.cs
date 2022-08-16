using System.Net;
using Dapr.PluggableComponents.Components;

namespace DaprInMemoryComponents.Components.HttpMiddleware;

public class InMemoryHttpMiddleware : IHttpMiddleware
{
    private Dictionary<String, String> dataStore;

    public InMemoryHttpMiddleware()
    {
        dataStore = new Dictionary<string, string>();
    }

    public MiddlewareCapabilities Init(Dictionary<string, string> props)
    {
        Console.WriteLine("HTTP Middleware init!");
        return new MiddlewareCapabilities
        {
            HandlesHeader = true,
            HandlesBody = true,
        };
    }

    public MiddlewareResult HandleHeader(HttpRequest request)
    {
        Console.WriteLine("HTTP Middleware handle header!");

        request.Headers.Add("X-Fiddled-With-By-DotNet", "OHAI!");
        var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        resp.Headers.Add("X-Fiddled-With-By-DotNet", "OHAI!");
        resp.StatusCode = HttpStatusCode.Unauthorized;
        return new MiddlewareResult
        {
            request = request,
            response = resp,
        };
    }

    public MiddlewareResult HandleBody(HttpRequest request)
    {
        Console.WriteLine("HTTP Middleware handle body!");
        var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        
        return new MiddlewareResult
        {
            
        };
    }
}
