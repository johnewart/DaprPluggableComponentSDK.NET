using Dapr.Client.Autogen.Grpc.v1;
using Dapr.Proto.Components.V1;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;

namespace DaprPluggableComponentSDK.NET.Components;

public struct StoreObject
{
    internal Byte[] data { get; init; }
    internal int etag { get; init; }
}

public interface IStateStore
{
    StoreObject? Get(string key);
    void Init(Dictionary<string,string> properties);
    List<string> Features();
    void Delete(string key, int etag);
    void Set(string requestKey, StoreObject storeObject);
}
