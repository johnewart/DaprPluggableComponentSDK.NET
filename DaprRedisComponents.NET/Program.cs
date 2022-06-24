using DaprRedisComponents.NET.Services;
using DaprPluggableComponentSDK.NET;
using DaprInMemoryStateStore.NET.Services;
using StackExchange.Redis;

var socketPath = Environment.GetEnvironmentVariable(Constants.DaprSocketPathEnvironmentVariable);

Console.WriteLine("Starting .NET  in-memory state store");
Console.WriteLine(format: @"Using UNIX socket located at {0}", socketPath);

if (File.Exists(socketPath))
{
    File.Delete(socketPath);
}

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenUnixSocket(socketPath);
});


// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<ConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<RedisStateStoreService>();
app.MapGrpcService<InMemoryPubSubService>();

app.Run();
