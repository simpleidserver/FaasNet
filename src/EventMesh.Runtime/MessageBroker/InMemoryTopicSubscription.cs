using EventMesh.Runtime.Models;

namespace EventMesh.Runtime.MessageBroker
{
    public class InMemoryTopicSubscription
    {
        public ClientSession Session { get; set; }
        public int Offset { get; set; }
    }
}
