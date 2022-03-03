using FaasNet.EventMesh.Runtime.Models;

namespace FaasNet.EventMesh.Runtime.MessageBroker
{
    public class InMemoryTopicSubscription
    {
        public string ClientId { get; set; }
        public ClientSession Session { get; set; }
        public int Offset { get; set; }
    }
}
