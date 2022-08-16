using Dapr.PluggableComponents.Components;

namespace DaprInMemoryComponents.Components.PubSub;

public class InMemoryPubSubComponent : IPubSubComponent
{
    private static Dictionary<String, Queue<PubSubMessage>> dataStore = new Dictionary<string, Queue<PubSubMessage>>();
    
    public InMemoryPubSubComponent()
    {
        Console.WriteLine("Creating new PubSub component!");
    }
    
    public void Init(Dictionary<string, string> props)
    {
        Console.WriteLine("Init pub-sub component!");
    }

    public List<string> Features() 
    {
        return new List<string>();
    }

    public void Publish(string topic, PubSubMessage msg)
    {
       Console.WriteLine("Publish message to topic {0}", topic);
       if (!dataStore.ContainsKey(topic)) {
           Console.WriteLine("New topic being published to: '{0}'", topic);
           dataStore[topic] = new Queue<PubSubMessage>();
       } 
       dataStore[topic].Enqueue(msg);
       Console.WriteLine("There are now {0} messages in the queue for {1}", dataStore[topic].Count, topic);
    }

    public Queue<PubSubMessage> Subscribe(string topic)
    {
        if (!dataStore.ContainsKey(topic)) {
            Console.WriteLine("New topic being subscribed to: '{0}'", topic);
            dataStore[topic] = new Queue<PubSubMessage>();
        }
        return dataStore[topic];
    }
}
