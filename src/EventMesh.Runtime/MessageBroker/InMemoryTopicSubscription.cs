using EventMesh.Runtime.Models;

namespace EventMesh.Runtime.MessageBroker
{
    public class InMemoryTopicSubscription
    {
        public string ClientId { get; set; }
        public ClientSession Session { get; set; }
        public int Offset { get; set; }
    }
}
