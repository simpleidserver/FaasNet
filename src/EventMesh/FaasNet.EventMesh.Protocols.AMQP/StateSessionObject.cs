using Amqp.Framing;
using FaasNet.EventMesh.Client;

namespace FaasNet.EventMesh.Protocols.AMQP
{

    public class StateSessionObject
    {
        public string ClientId { get; set; }
        public string Password { get; set; }
        public IEventMeshClientPubSession EventMeshPubSession { get; set; }
        public SubscriptionResult EventMeshSubSession { get; set; }
        public Attach Link { get; set; }
    }
}
