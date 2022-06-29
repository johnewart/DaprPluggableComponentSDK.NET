namespace DaprInMemoryComponents.NET; 

using DaprPluggableComponentSDK.NET.Components; 
using System.Text;


public class InMemoryBinding : IInputBinding, IOutputBinding {
    public InMemoryBinding() { 
    }

    public string Name() { 
        return "InMemoryBlackHole";
    }

    public InvokeResult Invoke(string operation, byte[] request, Dictionary<string, string> metadata) { 
        return new InvokeResult() { 
            data = Encoding.ASCII.GetBytes("Black hole ate your data."),
            metadata = new Dictionary<string, string>(), 
            contentType = "text/plain"
        };
    }


    public void Init(Dictionary<string, string> metadata) {
    }

    public BindingResult Read() { 
        return new BindingResult() { 
            data = Encoding.ASCII.GetBytes("Black holes are empty, what do you expect here?"),
        contentType = "text/plain",
            metadata = new Dictionary<string, string>()
        };
    }

    public void Ping() {
        // Not gonna do it
    }
}
