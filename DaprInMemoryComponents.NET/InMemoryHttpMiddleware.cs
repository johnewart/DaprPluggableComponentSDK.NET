namespace DaprInMemoryStateStore.NET;

using DaprPluggableComponentSDK.NET.Components;

public class InMemoryHttpMiddleware : IHttpMiddleware
{
    private Dictionary<String, String> dataStore;

    public InMemoryHttpMiddleware()
    {
        dataStore = new Dictionary<string, string>();
    }

    public MiddlewareCapabilities Init(Dictionary<string, string> props)
    {
        return new MiddlewareCapabilities
        {
            HandlesHeader = true,
            HandlesBody = false,
        };
    }

    public MiddlewareResult HandleHeader(HttpRequest request)
    {

        request.Headers.Add("X-Fiddled-With-By-DotNet", "OHAI!");

        return new MiddlewareResult
        {
            request = request,
            response = null,
        };
    }

    public MiddlewareResult HandleBody(HttpRequest request)
    {
        return new MiddlewareResult
        {
        };
    }
}
