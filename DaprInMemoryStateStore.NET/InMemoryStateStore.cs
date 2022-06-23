using Dapr.Client.Autogen.Grpc.v1;
using Google.Protobuf.Collections;

namespace DaprInMemoryStateStore.NET;

using DaprPluggableComponentSDK.NET.Components;

public class InMemoryStateStore : IStateStore
{
    private Dictionary<String, StoreObject> dataStore;
    
    public InMemoryStateStore()
    {
        dataStore = new Dictionary<string, StoreObject>();
    }
    
    public StoreObject? Get(string requestKey)
    {
        if (dataStore.ContainsKey(requestKey))
        {
            return dataStore[requestKey];
        }
        else
        {
            return null; 
        }
    }

    public void Init(Dictionary<string, string> props)
    {
    }

    public List<string> Features()
    {
        return new List<string>();
    }

    public void Delete(string requestKey, int etag)
    {
        dataStore.Remove(requestKey);
    }

    public void Set(string requestKey, StoreObject storeObject)
    {
        dataStore[requestKey] = storeObject;
    }
}