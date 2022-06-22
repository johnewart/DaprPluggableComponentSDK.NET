using Dapr.Client.Autogen.Grpc.v1;
using Dapr.Proto.Components.V1;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;

namespace DaprPluggableComponentSDK.NET.Components;

public struct StoreObject
{
    internal Byte[] data { get; }
    internal int etag { get;  }
}

public interface IStateStore
{
    StoreObject Get(string requestKey);
    void Init(MapField<string,string> requestProperties);
    List<string> Features();
    void Delete(string requestKey, StateOptions requestOptions, Etag requestEtag);
    void Set(string requestKey, ByteString requestValue, Etag requestEtag);
}