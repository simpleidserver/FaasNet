using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Client
{
    public class RemoveTargetCommand : ICommand
    {
        public string Vpn { get; set; }
        public string ClientId { get; set; }
        public string EventId { get; set; }
        public string Target { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Vpn = context.NextString();
            ClientId = context.NextString();
            EventId = context.NextString();
            Target = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteString(ClientId);
            context.WriteString(EventId);
            context.WriteString(Target);
        }
    }
}
