using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Subscription
{
    public class AddSubscriptionCommand : ICommand
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string EventId { get; set; }
        public string Vpn { get; set; }
        public string Topic { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            ClientId = context.NextString();
            EventId = context.NextString();
            Vpn = context.NextString();
            Topic = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(ClientId);
            context.WriteString(EventId);
            context.WriteString(Vpn);
            context.WriteString(Topic);
        }
    }
}
