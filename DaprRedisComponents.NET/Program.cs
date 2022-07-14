using DaprRedisComponents.NET.Services;
using DaprPluggableComponentSDK.NET;
using DaprInMemoryStateStore.NET.Services;
using StackExchange.Redis;
using System.Net;

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
    options.Limits.MaxResponseBufferSize = 0;
});


// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
// String strHostName = string.Empty;
// Getting Ip address of local machine...
// First get the host name of local machine.
var strHostName = Dns.GetHostName();
Console.WriteLine("Local Machine's Host Name: " + strHostName);
// Then using host name, get the IP address list..
IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
IPAddress[] addr = ipEntry.AddressList;

for (int i = 0; i < addr.Length; i++)
{
    Console.WriteLine("IP Address {0}: {1} ", i, addr[i].ToString());
}

var redisHost = "127.0.0.1";
var connectOpts = new ConfigurationOptions{
                EndPoints = { redisHost },
                AbortOnConnectFail = false,
            };
Console.WriteLine("Connecting to Redis on {0}", redisHost);
builder.Services.AddGrpc();
builder.Services.AddSingleton<ConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectOpts));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<RedisStateStoreService>();
app.MapGrpcService<InMemoryPubSubService>();


app.Run();

